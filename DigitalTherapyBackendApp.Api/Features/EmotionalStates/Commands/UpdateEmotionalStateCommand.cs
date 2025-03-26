using DigitalTherapyBackendApp.Api.Features.EmotionalStates.Payloads;
using DigitalTherapyBackendApp.Api.Features.EmotionalStates.Responses;
using DigitalTherapyBackendApp.Application.Dtos;
using DigitalTherapyBackendApp.Infrastructure.ExternalServices;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Api.Features.EmotionalStates.Commands
{
    public class UpdateEmotionalStateCommand : IRequest<UpdateEmotionalStateResponse>
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public UpdateEmotionalStatePayload Payload { get; set; }

        public UpdateEmotionalStateCommand(Guid id, UpdateEmotionalStatePayload payload, Guid userId)
        {
            Id = id;
            Payload = payload;
            UserId = userId;
        }
    }

    public class UpdateEmotionalStateCommandHandler : IRequestHandler<UpdateEmotionalStateCommand, UpdateEmotionalStateResponse>
    {
        private readonly IEmotionalStateService _emotionalStateService;

        public UpdateEmotionalStateCommandHandler(IEmotionalStateService emotionalStateService)
        {
            _emotionalStateService = emotionalStateService;
        }

        public async Task<UpdateEmotionalStateResponse> Handle(UpdateEmotionalStateCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var existingRecord = await _emotionalStateService.GetByIdAsync(command.Id, command.UserId);

                if (existingRecord == null)
                {
                    return new UpdateEmotionalStateResponse
                    {
                        Success = false,
                        Message = "The specified mood record was not found.",
                        ErrorCode = "EMOTIONALSTATE_NOT_FOUND",
                        Data = null
                    };
                }

                var dto = new UpdateEmotionalStateDto
                {
                    MoodLevel = command.Payload.MoodLevel,
                    Factors = command.Payload.Factors,
                    Notes = command.Payload.Notes,
                    Date = command.Payload.Date,
                    IsBookmarked = command.Payload.IsBookmarked
                };

                var result = await _emotionalStateService.UpdateAsync(command.Id, dto, command.UserId);

                if (!result)
                {
                    return new UpdateEmotionalStateResponse
                    {
                        Success = false,
                        Message = "Failed to update mood record.",
                        ErrorCode = "UPDATE_EMOTIONALSTATE_FAILED",
                        Data = null
                    };
                }

                var updatedRecord = await _emotionalStateService.GetByIdAsync(command.Id, command.UserId);

                return new UpdateEmotionalStateResponse
                {
                    Success = true,
                    Message = "Mood record updated successfully.",
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
                return new UpdateEmotionalStateResponse
                {
                    Success = false,
                    Message = $"An error occurred while updating the mood record: {ex.Message}",
                    ErrorCode = "UPDATE_EMOTIONALSTATE_ERROR",
                    Data = null
                };
            }
        }
    }
}