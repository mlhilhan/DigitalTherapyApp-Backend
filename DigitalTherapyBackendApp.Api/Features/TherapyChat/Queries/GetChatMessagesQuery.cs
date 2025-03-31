using DigitalTherapyBackendApp.Api.Features.TherapyChat.Responses;
using DigitalTherapyBackendApp.Application.Dtos;
using DigitalTherapyBackendApp.Domain.Interfaces;
using MediatR;

namespace DigitalTherapyBackendApp.Api.Features.TherapyChat.Queries
{
    public class GetChatMessagesQuery : IRequest<GetChatMessagesResponse>
    {
        public Guid SessionId { get; set; }
        public Guid UserId { get; set; }
    }

    public class GetChatMessagesQueryHandler : IRequestHandler<GetChatMessagesQuery, GetChatMessagesResponse>
    {
        private readonly ITherapySessionRepository _therapySessionRepository;
        private readonly ISessionMessageRepository _sessionMessageRepository;
        private readonly ILogger<GetChatMessagesQueryHandler> _logger;

        public GetChatMessagesQueryHandler(
            ITherapySessionRepository therapySessionRepository,
            ISessionMessageRepository sessionMessageRepository,
            ILogger<GetChatMessagesQueryHandler> logger)
        {
            _therapySessionRepository = therapySessionRepository;
            _sessionMessageRepository = sessionMessageRepository;
            _logger = logger;
        }

        public async Task<GetChatMessagesResponse> Handle(GetChatMessagesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Oturumun bu kullanıcıya ait olduğunu doğrula
                var session = await _therapySessionRepository.GetByIdAsync(request.SessionId);
                if (session == null || session.PatientId != request.UserId)
                {
                    return new GetChatMessagesResponse
                    {
                        Success = false,
                        Message = "Oturum bulunamadı veya erişim izniniz yok"
                    };
                }

                _logger.LogInformation($"Getting chat messages for session {request.SessionId}");

                var messages = await _sessionMessageRepository.GetBySessionIdAsync(request.SessionId);

                var messageDtos = messages.Select(m => new ChatMessageDto
                {
                    Id = m.Id,
                    SenderId = m.SenderId,
                    Content = m.Content,
                    Timestamp = m.SentAt,
                    IsAiGenerated = m.IsAiGenerated
                }).ToList();

                return new GetChatMessagesResponse
                {
                    Success = true,
                    Data = messageDtos,
                    Message = "Chat mesajları başarıyla getirildi"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting chat messages for session {request.SessionId}");
                return new GetChatMessagesResponse
                {
                    Success = false,
                    Message = "Chat mesajları getirilirken bir hata oluştu"
                };
            }
        }
    }
}
