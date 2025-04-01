using MediatR;
using DigitalTherapyBackendApp.Api.Features.TherapyChat.Responses;
using DigitalTherapyBackendApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using DigitalTherapyBackendApp.Domain.Entities;

namespace DigitalTherapyBackendApp.Api.Features.TherapyChat.Commands
{
    public class ClearAllAiSessionsCommand : IRequest<ClearAllAiSessionsResponse>
    {
        public Guid UserId { get; set; }
    }

    public class ClearAllAiSessionsCommandHandler : IRequestHandler<ClearAllAiSessionsCommand, ClearAllAiSessionsResponse>
    {
        private readonly AppDbContext _db;
        private readonly ILogger<ClearAllAiSessionsCommandHandler> _logger;

        public ClearAllAiSessionsCommandHandler(
            AppDbContext db,
            ILogger<ClearAllAiSessionsCommandHandler> logger)
        {
            _db = db;
            _logger = logger;
        }


        public async Task<ClearAllAiSessionsResponse> Handle(ClearAllAiSessionsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userAiSessions = await _db.TherapySessions
                    .Where(s => s.PatientId == request.UserId && s.IsAiSession == true)
                    .ToListAsync(cancellationToken);

                if (userAiSessions.Count == 0)
                {
                    return new ClearAllAiSessionsResponse
                    {
                        Success = true,
                        Message = "No session found to clear."
                    };
                }

                foreach (var session in userAiSessions)
                {
                    if (!session.EndTime.HasValue)
                    {
                        session.EndTime = DateTime.UtcNow;
                    }

                    session.IsActive = false;
                    session.IsArchived = true;
                    session.Status = SessionStatus.Archived;
                }

                await _db.SaveChangesAsync(cancellationToken);

                return new ClearAllAiSessionsResponse
                {
                    Success = true,
                    Message = "Sessions cleared successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error clearing AI sessions for user {request.UserId}");

                return new ClearAllAiSessionsResponse
                {
                    Success = false,
                    Message = "An error occurred while clearing AI Sessions."
                };
            }
        }
    }
}