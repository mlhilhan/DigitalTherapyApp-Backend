using DigitalTherapyBackendApp.Api.Features.PsychologistProfiles.Payloads;
using DigitalTherapyBackendApp.Api.Features.PsychologistProfiles.Responses;
using DigitalTherapyBackendApp.Domain.Entities;
using DigitalTherapyBackendApp.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Api.Features.PsychologistProfiles.Commands
{
    public class UpdatePsychologistAvailabilityCommand : IRequest<UpdatePsychologistAvailabilityResponse>
    {
        [JsonIgnore]
        public Guid PsychologistId { get; set; }

        public bool IsAvailable { get; set; }

        public List<AvailabilitySlotInfo> AvailabilitySlots { get; set; } = new List<AvailabilitySlotInfo>();

        public class AvailabilitySlotInfo
        {
            public DayOfWeek DayOfWeek { get; set; }
            public TimeSpan StartTime { get; set; }
            public TimeSpan EndTime { get; set; }
        }

        public static UpdatePsychologistAvailabilityCommand FromPayload(UpdatePsychologistAvailabilityPayload payload, Guid psychologistId)
        {
            return new UpdatePsychologistAvailabilityCommand
            {
                PsychologistId = psychologistId,
                IsAvailable = payload.IsAvailable,
                AvailabilitySlots = payload.AvailabilitySlots?.Select(s => new AvailabilitySlotInfo
                {
                    DayOfWeek = s.DayOfWeek,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime
                }).ToList() ?? new List<AvailabilitySlotInfo>()
            };
        }
    }

    public class UpdatePsychologistAvailabilityCommandHandler : IRequestHandler<UpdatePsychologistAvailabilityCommand, UpdatePsychologistAvailabilityResponse>
    {
        private readonly IPsychologistProfileRepository _psychologistProfileRepository;
        private readonly ILogger<UpdatePsychologistAvailabilityCommandHandler> _logger;

        public UpdatePsychologistAvailabilityCommandHandler(
            IPsychologistProfileRepository psychologistProfileRepository,
            ILogger<UpdatePsychologistAvailabilityCommandHandler> logger)
        {
            _psychologistProfileRepository = psychologistProfileRepository;
            _logger = logger;
        }

        public async Task<UpdatePsychologistAvailabilityResponse> Handle(UpdatePsychologistAvailabilityCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var psychologist = await _psychologistProfileRepository.GetByIdAsync(request.PsychologistId);
                if (psychologist == null)
                {
                    _logger.LogWarning("Psychologist profile not found with ID: {PsychologistId}", request.PsychologistId);
                    return new UpdatePsychologistAvailabilityResponse
                    {
                        Success = false,
                        Message = "No psychologist profile found."
                    };
                }

                psychologist.IsAvailable = request.IsAvailable;

                var availabilitySlots = request.AvailabilitySlots.Select(slot => new PsychologistAvailabilitySlot
                {
                    PsychologistProfileId = psychologist.Id,
                    DayOfWeek = slot.DayOfWeek,
                    StartTime = slot.StartTime,
                    EndTime = slot.EndTime
                }).ToList();

                await _psychologistProfileRepository.UpdateAvailabilityAsync(
                    request.PsychologistId,
                    request.IsAvailable,
                    availabilitySlots);

                var responseData = new UpdatePsychologistAvailabilityResponse.AvailabilityData
                {
                    IsAvailable = request.IsAvailable,
                    Slots = availabilitySlots.Select(slot => new UpdatePsychologistAvailabilityResponse.AvailabilitySlotDto
                    {
                        Id = slot.Id,
                        DayOfWeek = slot.DayOfWeek,
                        StartTime = slot.StartTime,
                        EndTime = slot.EndTime
                    }).ToList()
                };

                return new UpdatePsychologistAvailabilityResponse
                {
                    Success = true,
                    Message = "Psychologist availability has been updated successfully.",
                    Data = responseData
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating availability for psychologist: {PsychologistId}", request.PsychologistId);
                return new UpdatePsychologistAvailabilityResponse
                {
                    Success = false,
                    Message = "An error occurred while updating availability."
                };
            }
        }
    }
}