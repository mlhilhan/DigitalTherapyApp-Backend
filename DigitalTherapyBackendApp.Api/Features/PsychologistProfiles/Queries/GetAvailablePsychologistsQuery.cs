using DigitalTherapyBackendApp.Api.Features.PsychologistProfiles.Responses;
using DigitalTherapyBackendApp.Application.Dtos;
using DigitalTherapyBackendApp.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Api.Features.PsychologistProfiles.Queries
{
    public class GetAvailablePsychologistsQuery : IRequest<GetAvailablePsychologistsResponse>
    {
    }

    public class GetAvailablePsychologistsQueryHandler : IRequestHandler<GetAvailablePsychologistsQuery, GetAvailablePsychologistsResponse>
    {
        private readonly IPsychologistProfileRepository _psychologistProfileRepository;
        private readonly ILogger<GetAvailablePsychologistsQueryHandler> _logger;
        private readonly string _baseUrl;

        public GetAvailablePsychologistsQueryHandler(
            IPsychologistProfileRepository psychologistProfileRepository,
            ILogger<GetAvailablePsychologistsQueryHandler> logger,
            IConfiguration configuration)
        {
            _psychologistProfileRepository = psychologistProfileRepository;
            _logger = logger;
            _baseUrl = configuration["AppSettings:BaseUrl"];
        }

        public async Task<GetAvailablePsychologistsResponse> Handle(GetAvailablePsychologistsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var psychologists = await _psychologistProfileRepository.GetAvailablePsychologistsAsync();
                if (psychologists == null || !psychologists.Any())
                {
                    _logger.LogInformation("No available psychologists found");
                    return new GetAvailablePsychologistsResponse
                    {
                        Success = true,
                        Data = new List<PsychologistProfileDto>(),
                        Message = "No available psychologist found."
                    };
                }

                // Profil verilerini DTO'lara dönüştür
                var psychologistDtos = psychologists.Select(p => new PsychologistProfileDto
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    BirthDate = p.BirthDate,
                    Gender = p.Gender,
                    Bio = p.Bio,
                    AvatarUrl = !string.IsNullOrEmpty(p.AvatarUrl) ? $"{_baseUrl}{p.AvatarUrl}" : null,
                    PreferredLanguage = p.PreferredLanguage,
                    Email = p.User?.Email,
                    PhoneNumber = p.User?.PhoneNumber,
                    InstitutionId = p.InstitutionId,
                    InstitutionName = p.Institution?.Name,
                    Education = p.Education,
                    Certifications = p.Certifications,
                    Experience = p.Experience,
                    LicenseNumber = p.LicenseNumber,
                    IsAvailable = p.IsAvailable,
                    Specialties = p.Specialties?.Select(s => s.Name).ToList() ?? new List<string>(),
                    AvailabilitySlots = p.AvailabilitySlots?.Select(a => new AvailabilitySlotDto
                    {
                        Id = a.Id,
                        DayOfWeek = a.DayOfWeek,
                        StartTime = a.StartTime,
                        EndTime = a.EndTime
                    }).ToList() ?? new List<AvailabilitySlotDto>()
                }).ToList();

                return new GetAvailablePsychologistsResponse
                {
                    Success = true,
                    Data = psychologistDtos,
                    Message = "Available psychologists were brought in successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving available psychologists");
                return new GetAvailablePsychologistsResponse
                {
                    Success = false,
                    Message = "An error occurred while fetching available psychologists: " + ex.Message
                };
            }
        }
    }
}