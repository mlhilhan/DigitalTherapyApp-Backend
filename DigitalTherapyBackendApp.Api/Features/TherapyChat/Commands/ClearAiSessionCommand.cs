using DigitalTherapyBackendApp.Api.Features.TherapyChat.Responses;
using DigitalTherapyBackendApp.Domain.Entities;
using DigitalTherapyBackendApp.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Api.Features.TherapyChat.Commands
{
    public class ClearAiSessionCommand : IRequest<ClearAiSessionResponse>
    {
        public Guid UserId { get; set; }
        public Guid SessionId { get; set; }
    }

    public class ClearAiSessionCommandHandler : IRequestHandler<ClearAiSessionCommand, ClearAiSessionResponse>
    {
        private readonly AppDbContext _db;
        private readonly ILogger<ClearAiSessionCommandHandler> _logger;

        public ClearAiSessionCommandHandler(
            AppDbContext db,
            ILogger<ClearAiSessionCommandHandler> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<ClearAiSessionResponse> Handle(ClearAiSessionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var session = await _db.TherapySessions
                    .FirstOrDefaultAsync(s => s.Id == request.SessionId && s.PatientId == request.UserId, cancellationToken);

                if (session == null)
                {
                    return new ClearAiSessionResponse
                    {
                        Success = false,
                        Message = "The specified session was not found or you do not have permission to access this session."
                    };
                }

                if (session.IsArchived)
                {
                    return new ClearAiSessionResponse
                    {
                        Success = false,
                        Message = "The session has already been deleted."
                    };
                }

                session.IsArchived = true;
                session.Status = SessionStatus.Archived;

                if (session.IsActive)
                {
                    session.IsActive = false;
                    session.EndTime = DateTime.UtcNow;
                }

                await _db.SaveChangesAsync(cancellationToken);

                return new ClearAiSessionResponse
                {
                    Success = true,
                    Message = "The session was deleted successfully.",
                    SessionId = session.Id
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error archiving session {request.SessionId} for user {request.UserId}");

                return new ClearAiSessionResponse
                {
                    Success = false,
                    Message = "An error occurred while archiving the session."
                };
            }
        }
    }
}