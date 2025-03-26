using DigitalTherapyBackendApp.Api.Features.EmotionalStates.Responses;
using DigitalTherapyBackendApp.Infrastructure.ExternalServices;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Api.Features.EmotionalStates.Commands
{
    public class DeleteEmotionalStateCommand : IRequest<DeleteEmotionalStateResponse>
    {
        public Guid Id { get; }
        public Guid UserId { get; }

        public DeleteEmotionalStateCommand(Guid id, Guid userId)
        {
            Id = id;
            UserId = userId;
        }
    }


    public class DeleteEmotionalStateCommandHandler : IRequestHandler<DeleteEmotionalStateCommand, DeleteEmotionalStateResponse>
    {
        private readonly IEmotionalStateService _emotionalStateService;

        public DeleteEmotionalStateCommandHandler(IEmotionalStateService emotionalStateService)
        {
            _emotionalStateService = emotionalStateService;
        }

        public async Task<DeleteEmotionalStateResponse> Handle(DeleteEmotionalStateCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var existingRecord = await _emotionalStateService.GetByIdAsync(command.Id, command.UserId);

                if (existingRecord == null)
                {
                    return new DeleteEmotionalStateResponse
                    {
                        Success = false,
                        Message = "The specified mood record was not found.",
                        ErrorCode = "EMOTIONALSTATE_NOT_FOUND",
                        Data = null
                    };
                }

                var recordData = new EmotionalStateData
                {
                    Id = existingRecord.Id,
                    MoodLevel = existingRecord.MoodLevel,
                    Factors = existingRecord.Factors,
                    Notes = existingRecord.Notes,
                    Date = existingRecord.Date,
                    IsBookmarked = existingRecord.IsBookmarked,
                    IsDeleted = true
                };

                var result = await _emotionalStateService.DeleteAsync(command.Id, command.UserId);

                if (!result)
                {
                    return new DeleteEmotionalStateResponse
                    {
                        Success = false,
                        Message = "Failed to delete mood record.",
                        ErrorCode = "DELETE_EMOTIONALSTATE_FAILED",
                        Data = null
                    };
                }

                return new DeleteEmotionalStateResponse
                {
                    Success = true,
                    Message = "Mood record deleted successfully.",
                    Data = recordData
                };
            }
            catch (Exception ex)
            {
                return new DeleteEmotionalStateResponse
                {
                    Success = false,
                    Message = $"An error occurred while deleting the mood record: {ex.Message}",
                    ErrorCode = "DELETE_EMOTIONALSTATE_ERROR",
                    Data = null
                };
            }
        }
    }
}
   