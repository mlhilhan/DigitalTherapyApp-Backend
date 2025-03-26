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
    public class CreateEmotionalStateCommand : IRequest<CreateEmotionalStateResponse>
    {
        public CreateEmotionalStatePayload Payload { get; set; }
        public Guid UserId { get; set; }

        public CreateEmotionalStateCommand(CreateEmotionalStatePayload payload, Guid userId)
        {
            Payload = payload;
            UserId = userId;
        }
    }

    public class CreateEmotionalStateCommandHandler : IRequestHandler<CreateEmotionalStateCommand, CreateEmotionalStateResponse>
    {
        private readonly IEmotionalStateService _emotionalStateService;

        public CreateEmotionalStateCommandHandler(IEmotionalStateService emotionalStateService)
        {
            _emotionalStateService = emotionalStateService;
        }

        public async Task<CreateEmotionalStateResponse> Handle(CreateEmotionalStateCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var dto = new CreateEmotionalStateDto
                {
                    MoodLevel = command.Payload.MoodLevel,
                    Factors = command.Payload.Factors,
                    Notes = command.Payload.Notes,
                    Date = command.Payload.Date,
                    IsBookmarked = command.Payload.IsBookmarked
                };

                var newId = await _emotionalStateService.CreateAsync(dto, command.UserId);

                var createdRecord = await _emotionalStateService.GetByIdAsync(newId, command.UserId);

                if (createdRecord == null)
                {
                    return new CreateEmotionalStateResponse
                    {
                        Success = true,
                        Message = "Mood record created successfully, but unable to retrieve details.",
                        Data = new EmotionalStateData
                        {
                            Id = newId
                        }
                    };
                }

                return new CreateEmotionalStateResponse
                {
                    Success = true,
                    Message = "Mood record created successfully.",
                    Data = new EmotionalStateData
                    {
                        Id = createdRecord.Id,
                        MoodLevel = createdRecord.MoodLevel,
                        Factors = createdRecord.Factors,
                        Notes = createdRecord.Notes,
                        Date = createdRecord.Date,
                        IsBookmarked = createdRecord.IsBookmarked,
                        CreatedAt = DateTime.UtcNow
                    }
                };
            }
            catch (Exception ex)
            {
                return new CreateEmotionalStateResponse
                {
                    Success = false,
                    Message = $"An error occurred while creating the mood record: {ex.Message}",
                    ErrorCode = "CREATE_EMOTIONALSTATE_ERROR",
                    Data = null
                };
            }
        }
    }
}