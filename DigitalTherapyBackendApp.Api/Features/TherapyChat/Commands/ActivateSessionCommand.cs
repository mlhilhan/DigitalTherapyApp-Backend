using DigitalTherapyBackendApp.Api.Features.TherapyChat.Responses;
using DigitalTherapyBackendApp.Application.Dtos;
using DigitalTherapyBackendApp.Domain.Interfaces;
using DigitalTherapyBackendApp.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DigitalTherapyBackendApp.Api.Features.TherapyChat.Commands
{
    public class ActivateSessionCommand : IRequest<ActivateSessionResponse>
    {
        public Guid UserId { get; set; }
        public Guid SessionId { get; set; }
    }


    public class ActivateSessionCommandHandler : IRequestHandler<ActivateSessionCommand, ActivateSessionResponse>
    {
        private readonly AppDbContext _db;
        private readonly ILogger<ActivateSessionCommandHandler> _logger;

        public ActivateSessionCommandHandler(
            AppDbContext db,
            ILogger<ActivateSessionCommandHandler> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<ActivateSessionResponse> Handle(ActivateSessionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Kullanıcıya ait tüm oturumları al
                var userSessions = await _db.TherapySessions
                    .Where(s => s.PatientId == request.UserId)
                    .ToListAsync(cancellationToken);

                // İstenen oturumu bul
                var targetSession = userSessions.FirstOrDefault(s => s.Id == request.SessionId);
                if (targetSession == null)
                {
                    return new ActivateSessionResponse
                    {
                        Success = false,
                        Message = "Belirtilen oturum bulunamadı"
                    };
                }

                // Tüm aktif oturumları deaktif yap
                foreach (var session in userSessions.Where(s => s.IsActive && s.Id != request.SessionId))
                {
                    session.IsActive = false;
                    session.EndTime = DateTime.UtcNow;
                }

                // Hedef oturumu aktifleştir
                targetSession.IsActive = true;

                // EndTime null ise eski bir oturumdur, tekrar başlatılıyor olarak işaretle
                if (targetSession.EndTime.HasValue)
                {
                    targetSession.EndTime = null;
                    targetSession.ReactivatedAt = DateTime.UtcNow;
                }

                await _db.SaveChangesAsync(cancellationToken);

                // DTO'yu oluştur ve geri dön
                var sessionDto = new ChatSessionDto
                {
                    Id = targetSession.Id,
                    StartTime = targetSession.StartTime,
                    EndTime = targetSession.EndTime,
                    IsActive = targetSession.IsActive,
                    MessageCount = await _db.SessionMessages
                        .CountAsync(m => m.SessionId == targetSession.Id, cancellationToken),
                    LastMessage = await _db.SessionMessages
                        .Where(m => m.SessionId == targetSession.Id)
                        .OrderByDescending(m => m.SentAt)
                        .Select(m => m.Content)
                        .FirstOrDefaultAsync(cancellationToken),
                    Status = targetSession.Status.ToString(),
                };

                return new ActivateSessionResponse
                {
                    Success = true,
                    Message = "Oturum başarıyla aktifleştirildi",
                    Data = sessionDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error activating session {request.SessionId} for user {request.UserId}");

                return new ActivateSessionResponse
                {
                    Success = false,
                    Message = "Oturum aktifleştirme sırasında bir hata oluştu"
                };
            }
        }
    }
}
