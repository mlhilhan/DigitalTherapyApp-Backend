using DigitalTherapyBackendApp.Api.Features.TherapyChat.Payloads;
using DigitalTherapyBackendApp.Api.Features.TherapyChat.Responses;
using DigitalTherapyBackendApp.Application.Dtos;
using DigitalTherapyBackendApp.Application.Interfaces;
using DigitalTherapyBackendApp.Domain.Entities;
using DigitalTherapyBackendApp.Domain.Interfaces;
using DigitalTherapyBackendApp.Infrastructure.Repositories;
using MediatR;
using System;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Api.Features.TherapySessions.Commands
{
    public class SendChatMessageCommand : IRequest<SendChatMessageResponse>
    {
        public Guid UserId { get; set; }
        public string Message { get; set; }
        public Guid? SessionId { get; set; }

        public static SendChatMessageCommand FromPayload(SendChatMessagePayload payload, Guid userId, Guid? sessionId)
        {
            return new SendChatMessageCommand
            {
                UserId = userId,
                Message = payload.Message,
                SessionId = payload.SessionId
            };
        }
    }

    public class SendChatMessageCommandHandler : IRequestHandler<SendChatMessageCommand, SendChatMessageResponse>
    {
        private readonly IAiService _aiService;
        private readonly ILogger<SendChatMessageCommandHandler> _logger;
        private readonly ITherapySessionRepository _therapySessionRepository;

        public SendChatMessageCommandHandler(
            IAiService aiService,
            ILogger<SendChatMessageCommandHandler> logger,
            ITherapySessionRepository therapySessionRepository)
        {
            _aiService = aiService;
            _logger = logger;
            _therapySessionRepository = therapySessionRepository;
        }

        public async Task<SendChatMessageResponse> Handle(SendChatMessageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Mesaj kontrolü
                if (string.IsNullOrWhiteSpace(request.Message))
                {
                    return new SendChatMessageResponse
                    {
                        Success = false,
                        Message = "Mesaj boş olamaz"
                    };
                }

                // Oturum kontrolü - belirtilen bir oturum ID'si varsa kullan, yoksa aktif veya yeni oturum oluştur
                TherapySession session;
                if (request.SessionId.HasValue)
                {
                    // Belirtilen oturumu al ve kullanıcıya ait olduğunu kontrol et
                    session = await _therapySessionRepository.GetByIdAsync(request.SessionId.Value);
                    if (session == null || session.PatientId != request.UserId)
                    {
                        return new SendChatMessageResponse
                        {
                            Success = false,
                            Message = "Belirtilen oturum bulunamadı veya erişim izniniz yok"
                        };
                    }
                }
                else
                {
                    // Aktif oturumu al veya yeni oluştur
                    session = await _therapySessionRepository.GetActiveAiSessionAsync(request.UserId)
                           ?? await _therapySessionRepository.CreateAiSessionAsync(request.UserId);
                }

                // AI yanıtını al
                string aiResponse = await _aiService.GetChatResponseAsync(request.UserId, request.Message);

                return new SendChatMessageResponse
                {
                    Success = true,
                    Data = new ChatResponseDto
                    {
                        Message = aiResponse,
                        Timestamp = DateTime.UtcNow,
                        SessionId = session.Id
                    },
                    Message = "Mesaj başarıyla gönderildi"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing chat message for user {request.UserId}");
                return new SendChatMessageResponse
                {
                    Success = false,
                    Message = "Mesaj işlenirken bir hata oluştu"
                };
            }
        }
    }
}