using DigitalTherapyBackendApp.Api.Features.TherapyChat.Responses;
using DigitalTherapyBackendApp.Application.Dtos;
using DigitalTherapyBackendApp.Domain.Entities;
using DigitalTherapyBackendApp.Domain.Interfaces;
using MediatR;

namespace DigitalTherapyBackendApp.Api.Features.TherapyChat.Commands
{
    public class StartChatSessionCommand : IRequest<StartChatSessionResponse>
    {
        public Guid UserId { get; set; }
        public bool ForceNew { get; set; }
    }

    public class StartChatSessionCommandHandler : IRequestHandler<StartChatSessionCommand, StartChatSessionResponse>
    {
        private readonly ITherapySessionRepository _therapySessionRepository;
        private readonly ISessionMessageRepository _sessionMessageRepository;
        private readonly ILogger<StartChatSessionCommandHandler> _logger;
        private readonly IConfiguration _configuration;

        public StartChatSessionCommandHandler(
            ITherapySessionRepository therapySessionRepository,
            ISessionMessageRepository sessionMessageRepository,
            ILogger<StartChatSessionCommandHandler> logger,
            IConfiguration configuration)
        {
            _therapySessionRepository = therapySessionRepository;
            _sessionMessageRepository = sessionMessageRepository;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<StartChatSessionResponse> Handle(StartChatSessionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                TherapySession session;

                var activeSessions = await _therapySessionRepository.GetAiSessionsByPatientIdAsync(request.UserId);
                var currentActiveSessions = activeSessions.Where(s => s.IsActive).ToList();

                if (request.ForceNew || !currentActiveSessions.Any())
                {
                    session = await _therapySessionRepository.CreateAiSessionAsync(request.UserId);

                    foreach (var activeSession in currentActiveSessions.Where(s => s.Id != session.Id))
                    {
                        _logger.LogInformation($"Closing active session {activeSession.Id} due to new session creation");
                        await _therapySessionRepository.CloseAiSessionAsync(activeSession.Id);
                    }

                    string systemUserIdStr = _configuration["AI:SystemUserId"];
                    Guid systemUserId;

                    if (!Guid.TryParse(systemUserIdStr, out systemUserId))
                    {
                        systemUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
                    }

                    await _sessionMessageRepository.AddAiMessageAsync(
                        session.Id,
                        systemUserId,
                        "Hello! I'm your digital therapy assistant. How can I help you today?"
                    );

                    return new StartChatSessionResponse
                    {
                        Success = true,
                        Data = new ChatSessionDto
                        {
                            Id = session.Id,
                            StartTime = session.StartTime,
                            EndTime = session.EndTime,
                            IsActive = session.IsActive,
                            LastMessage = "Hello! I'm your digital therapy assistant. How can I help you today?",
                            LastMessageTime = DateTime.UtcNow,
                            MessageCount = 1
                        },
                        Message = "New session started successfully"
                    };
                }
                else
                {
                    session = currentActiveSessions.First();

                    if (currentActiveSessions.Count > 1)
                    {
                        _logger.LogWarning($"Found {currentActiveSessions.Count} active sessions for user {request.UserId}. Keeping only one active.");
                        foreach (var activeSession in currentActiveSessions.Where(s => s.Id != session.Id))
                        {
                            _logger.LogInformation($"Closing extra active session {activeSession.Id}");
                            await _therapySessionRepository.CloseAiSessionAsync(activeSession.Id);
                        }
                    }

                    var lastMessage = await _sessionMessageRepository.GetLastMessageAsync(session.Id);

                    return new StartChatSessionResponse
                    {
                        Success = true,
                        Data = new ChatSessionDto
                        {
                            Id = session.Id,
                            StartTime = session.StartTime,
                            EndTime = session.EndTime,
                            IsActive = session.IsActive,
                            LastMessage = lastMessage?.Content,
                            LastMessageTime = lastMessage?.SentAt ?? session.StartTime,
                            MessageCount = await _sessionMessageRepository.GetMessageCountAsync(session.Id)
                        },
                        Message = "Active session found"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error starting chat session for user {request.UserId}");
                return new StartChatSessionResponse
                {
                    Success = false,
                    Message = "An error occurred while starting the session."
                };
            }
        }
    }
}