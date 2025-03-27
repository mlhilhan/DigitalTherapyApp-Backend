using DigitalTherapyBackendApp.Api.Features.EmotionalStates.Responses;
using DigitalTherapyBackendApp.Infrastructure.ExternalServices;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Api.Features.EmotionalStates.Commands
{
    public class ToggleBookmarkCommand : IRequest<ToggleBookmarkResponse>
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        public ToggleBookmarkCommand(Guid id, Guid userId)
        {
            Id = id;
            UserId = userId;
        }
    }

    public class ToggleBookmarkCommandHandler : IRequestHandler<ToggleBookmarkCommand, ToggleBookmarkResponse>
    {
        private readonly IEmotionalStateService _emotionalStateService;

        public ToggleBookmarkCommandHandler(IEmotionalStateService emotionalStateService)
        {
            _emotionalStateService = emotionalStateService;
        }

        public async Task<ToggleBookmarkResponse> Handle(ToggleBookmarkCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var existingRecord = await _emotionalStateService.GetByIdAsync(command.Id, command.UserId);

                if (existingRecord == null)
                {
                    return new ToggleBookmarkResponse
                    {
                        Success = false,
                        Message = "The specified mood record was not found.",
                        ErrorCode = "EMOTIONALSTATE_NOT_FOUND",
                        Data = null,
                        IsAdded = null
                    };
                }

                bool currentBookmarkStatus = existingRecord.IsBookmarked;

                var result = await _emotionalStateService.ToggleBookmarkAsync(command.Id, command.UserId);

                if (!result)
                {
                    return new ToggleBookmarkResponse
                    {
                        Success = false,
                        Message = "Failed to change bookmark status.",
                        ErrorCode = "TOGGLE_BOOKMARK_FAILED",
                        Data = null,
                        IsAdded = null
                    };
                }

                var updatedRecord = await _emotionalStateService.GetByIdAsync(command.Id, command.UserId);

                return new ToggleBookmarkResponse
                {
                    Success = true,
                    Message = currentBookmarkStatus
                        ? "Bookmark removed successfully."
                        : "Bookmark added successfully.",
                    IsAdded = !currentBookmarkStatus,
                    Data = new EmotionalStateData
                    {
                        Id = updatedRecord.Id,
                        MoodLevel = updatedRecord.MoodLevel,
                        Factors = updatedRecord.Factors,
                        Notes = updatedRecord.Notes,
                        Date = updatedRecord.Date,
                        IsBookmarked = updatedRecord.IsBookmarked,
                        UpdatedAt = DateTime.UtcNow
                    }
                };
            }
            catch (Exception ex)
            {
                return new ToggleBookmarkResponse
                {
                    Success = false,
                    Message = $"An error occurred while changing bookmark status: {ex.Message}",
                    ErrorCode = "TOGGLE_BOOKMARK_ERROR",
                    Data = null,
                    IsAdded = null
                };
            }
        }
    }
}