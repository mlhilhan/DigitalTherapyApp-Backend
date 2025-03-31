using DigitalTherapyBackendApp.Api.Features.TherapyChat.Responses;
using DigitalTherapyBackendApp.Domain.Interfaces;
using DigitalTherapyBackendApp.Application.Dtos;
using MediatR;

namespace DigitalTherapyBackendApp.Api.Features.TherapyChat.Queries
{
    public class GetChatSessionsQuery : IRequest<GetChatSessionsResponse>
    {
        public Guid UserId { get; set; }
    }

    public class GetChatSessionsQueryHandler : IRequestHandler<GetChatSessionsQuery, GetChatSessionsResponse>
    {
        private readonly ITherapySessionRepository _therapySessionRepository;
        private readonly ISessionMessageRepository _sessionMessageRepository;
        private readonly ILogger<GetChatSessionsQueryHandler> _logger;

        public GetChatSessionsQueryHandler(
            ITherapySessionRepository therapySessionRepository,
            ISessionMessageRepository sessionMessageRepository,
            ILogger<GetChatSessionsQueryHandler> logger)
        {
            _therapySessionRepository = therapySessionRepository;
            _sessionMessageRepository = sessionMessageRepository;
            _logger = logger;
        }

        public async Task<GetChatSessionsResponse> Handle(GetChatSessionsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Getting chat sessions for user {request.UserId}");

                var sessions = await _therapySessionRepository.GetAiSessionsByPatientIdAsync(request.UserId);

                var sessionDtos = new List<ChatSessionDto>();
                foreach (var session in sessions)
                {
                    var lastMessage = await _sessionMessageRepository.GetLastMessageAsync(session.Id);

                    sessionDtos.Add(new ChatSessionDto
                    {
                        Id = session.Id,
                        StartTime = session.StartTime,
                        EndTime = session.EndTime,
                        IsActive = session.IsActive,
                        LastMessage = lastMessage?.Content.Length > 50
                            ? lastMessage.Content.Substring(0, 47) + "..."
                            : lastMessage?.Content,
                        LastMessageTime = lastMessage?.SentAt ?? session.StartTime,
                        MessageCount = await _sessionMessageRepository.GetMessageCountAsync(session.Id)
                    });
                }

                return new GetChatSessionsResponse
                {
                    Success = true,
                    Data = sessionDtos,
                    Message = "Chat sessions were successfully retrieved."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting chat sessions for user {request.UserId}");
                return new GetChatSessionsResponse
                {
                    Success = false,
                    Message = "An error occurred while fetching chat sessions."
                };
            }
        }
    }
}
