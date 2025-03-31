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

                if (request.ForceNew ||
                    await _therapySessionRepository.GetActiveAiSessionAsync(request.UserId) == null)
                {
                    // Yeni oturum oluştur
                    session = await _therapySessionRepository.CreateAiSessionAsync(request.UserId);

                    // Eski aktif oturumları kapat
                    if (request.ForceNew)
                    {
                        var activeSessions = await _therapySessionRepository.GetAiSessionsByPatientIdAsync(request.UserId);
                        foreach (var activeSession in activeSessions.Where(s => s.IsActive && s.Id != session.Id))
                        {
                            await _therapySessionRepository.CloseAiSessionAsync(activeSession.Id);
                        }
                    }

                    // Sistem kullanıcı ID'sini al
                    string systemUserIdStr = _configuration["AI:SystemUserId"];
                    Guid systemUserId;

                    if (!Guid.TryParse(systemUserIdStr, out systemUserId))
                    {
                        systemUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
                    }

                    // Hoş geldin mesajı ekle
                    await _sessionMessageRepository.AddAiMessageAsync(
                        session.Id,
                        systemUserId,
                        "Merhaba! Ben dijital terapi asistanınız. Size nasıl yardımcı olabilirim?"
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
                            LastMessage = "Merhaba! Ben dijital terapi asistanınız. Size nasıl yardımcı olabilirim?",
                            LastMessageTime = DateTime.UtcNow,
                            MessageCount = 1
                        },
                        Message = "Yeni oturum başlatıldı"
                    };
                }
                else
                {
                    // Aktif oturumu bul
                    session = await _therapySessionRepository.GetActiveAiSessionAsync(request.UserId);

                    // Son mesajı al
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
                        Message = "Aktif oturum bulundu"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error starting chat session for user {request.UserId}");
                return new StartChatSessionResponse
                {
                    Success = false,
                    Message = "Oturum başlatılırken bir hata oluştu"
                };
            }
        }
    }
}
