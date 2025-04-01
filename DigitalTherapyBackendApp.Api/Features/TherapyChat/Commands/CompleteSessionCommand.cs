using DigitalTherapyBackendApp.Api.Features.TherapyChat.Responses;
using DigitalTherapyBackendApp.Application.Dtos;
using DigitalTherapyBackendApp.Domain.Entities;
using DigitalTherapyBackendApp.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DigitalTherapyBackendApp.Api.Features.TherapyChat.Commands
{
    public class CompleteSessionCommand : IRequest<CompleteSessionResponse>
    {
        public Guid UserId { get; set; }
        public Guid SessionId { get; set; }
    }

    public class CompleteSessionCommandHandler : IRequestHandler<CompleteSessionCommand, CompleteSessionResponse>
    {
        private readonly AppDbContext _db;
        private readonly ILogger<CompleteSessionCommandHandler> _logger;

        public CompleteSessionCommandHandler(
            AppDbContext db,
            ILogger<CompleteSessionCommandHandler> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<CompleteSessionResponse> Handle(CompleteSessionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var session = await _db.TherapySessions
                    .FirstOrDefaultAsync(s => s.Id == request.SessionId && s.PatientId == request.UserId, cancellationToken);

                if (session == null)
                {
                    return new CompleteSessionResponse
                    {
                        Success = false,
                        Message = "The specified session was not found or you do not have permission to access this session."
                    };
                }

                if (session.Status == SessionStatus.Completed)
                {
                    return new CompleteSessionResponse
                    {
                        Success = false,
                        Message = "The session has already been completed."
                    };
                }

                if (session.Status == SessionStatus.Cancelled)
                {
                    return new CompleteSessionResponse
                    {
                        Success = false,
                        Message = "A canceled session cannot be completed."
                    };
                }

                if (session.IsArchived)
                {
                    return new CompleteSessionResponse
                    {
                        Success = false,
                        Message = "An archived session cannot be completed."
                    };
                }

                session.Status = SessionStatus.Completed;
                session.IsActive = false;

                if (!session.EndTime.HasValue)
                {
                    session.EndTime = DateTime.UtcNow;
                }

                await _db.SaveChangesAsync(cancellationToken);

                var sessionDto = new ChatSessionDto
                {
                    Id = session.Id,
                    StartTime = session.StartTime,
                    EndTime = session.EndTime,
                    IsActive = session.IsActive,
                    Status = session.Status.ToString(),
                    IsArchived = session.IsArchived,
                    MessageCount = await _db.SessionMessages
                        .CountAsync(m => m.SessionId == session.Id, cancellationToken),
                    LastMessage = await _db.SessionMessages
                        .Where(m => m.SessionId == session.Id)
                        .OrderByDescending(m => m.SentAt)
                        .Select(m => m.Content)
                        .FirstOrDefaultAsync(cancellationToken)
                };

                return new CompleteSessionResponse
                {
                    Success = true,
                    Message = "The session status was changed to completed successfully.",
                    Data = sessionDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error completing session {request.SessionId} for user {request.UserId}");

                return new CompleteSessionResponse
                {
                    Success = false,
                    Message = "An unexpected error occurred: " + ex.Message,
                };
            }
        }
    }
}