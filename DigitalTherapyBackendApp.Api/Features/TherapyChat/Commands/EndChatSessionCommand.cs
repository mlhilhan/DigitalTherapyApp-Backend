using DigitalTherapyBackendApp.Api.Features.TherapyChat.Responses;
using DigitalTherapyBackendApp.Application.Interfaces;
using DigitalTherapyBackendApp.Domain.Interfaces;
using MediatR;

namespace DigitalTherapyBackendApp.Api.Features.TherapyChat.Commands
{
    public class EndChatSessionCommand : IRequest<EndChatSessionResponse>
    {
        public Guid SessionId { get; set; }
        public Guid UserId { get; set; }
    }

    public class EndChatSessionCommandHandler : IRequestHandler<EndChatSessionCommand, EndChatSessionResponse>
    {
        private readonly ITherapySessionRepository _therapySessionRepository;
        private readonly IAiService _aiService;
        private readonly ILogger<EndChatSessionCommandHandler> _logger;

        public EndChatSessionCommandHandler(
            ITherapySessionRepository therapySessionRepository,
            IAiService aiService,
            ILogger<EndChatSessionCommandHandler> logger)
        {
            _therapySessionRepository = therapySessionRepository;
            _aiService = aiService;
            _logger = logger;
        }

        public async Task<EndChatSessionResponse> Handle(EndChatSessionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Ending chat session {request.SessionId} for user {request.UserId}");

                // Oturumun bu kullanıcıya ait olduğunu doğrula
                var session = await _therapySessionRepository.GetByIdAsync(request.SessionId);
                if (session == null || session.PatientId != request.UserId)
                {
                    return new EndChatSessionResponse
                    {
                        Success = false,
                        Message = "Oturum bulunamadı veya erişim izniniz yok"
                    };
                }

                if (!session.IsAiSession)
                {
                    return new EndChatSessionResponse
                    {
                        Success = false,
                        Message = "Bu bir AI oturumu değil"
                    };
                }

                var result = await _aiService.EndChatSessionAsync(request.SessionId);
                if (!result)
                {
                    return new EndChatSessionResponse
                    {
                        Success = false,
                        Message = "Oturum sonlandırılamadı"
                    };
                }

                return new EndChatSessionResponse
                {
                    Success = true,
                    Message = "Oturum başarıyla sonlandırıldı"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error ending chat session {request.SessionId} for user {request.UserId}");
                return new EndChatSessionResponse
                {
                    Success = false,
                    Message = "Oturum sonlandırılırken bir hata oluştu"
                };
            }
        }
    }
}
