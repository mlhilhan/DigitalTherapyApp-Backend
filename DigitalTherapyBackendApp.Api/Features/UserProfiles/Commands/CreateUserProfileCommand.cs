using DigitalTherapyBackendApp.Api.Features.UserProfiles.Responses;
using DigitalTherapyBackendApp.Domain.Entities;
using DigitalTherapyBackendApp.Domain.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Api.Features.UserProfiles.Commands
{
    public class CreateUserProfileCommand : IRequest<UserProfileResponse>
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Gender { get; set; }
        public string Bio { get; set; }
        public string AvatarUrl { get; set; }
        public string PreferredLanguage { get; set; }
        public string NotificationPreferences { get; set; }
    }

    public class CreateUserProfileCommandHandler : IRequestHandler<CreateUserProfileCommand, UserProfileResponse>
    {
        //private readonly IUserProfileRepository _userProfileRepository;

        //public CreateUserProfileCommandHandler(IUserProfileRepository userProfileRepository)
        //{
        //    _userProfileRepository = userProfileRepository;
        //}

        public async Task<UserProfileResponse> Handle(CreateUserProfileCommand request, CancellationToken cancellationToken)
        {
            //var existingProfile = await _userProfileRepository.GetByUserIdAsync(request.UserId);
            //if (existingProfile != null)
            //    throw new InvalidOperationException("User profile already exists.");

            //var userProfile = new UserProfile
            //{
            //    Id = Guid.NewGuid(),
            //    UserId = request.UserId,
            //    FirstName = request.FirstName,
            //    LastName = request.LastName,
            //    BirthDate = request.BirthDate,
            //    Gender = request.Gender,
            //    Bio = request.Bio,
            //    AvatarUrl = request.AvatarUrl,
            //    PreferredLanguage = request.PreferredLanguage,
            //    NotificationPreferences = request.NotificationPreferences
            //};

            //var result = await _userProfileRepository.AddAsync(userProfile);

            //return new UserProfileResponse
            //{
            //    Id = result.Id,
            //    UserId = result.UserId,
            //    FirstName = result.FirstName,
            //    LastName = result.LastName,
            //    BirthDate = result.BirthDate,
            //    Gender = result.Gender,
            //    Bio = result.Bio,
            //    AvatarUrl = result.AvatarUrl,
            //    PreferredLanguage = result.PreferredLanguage,
            //    NotificationPreferences = result.NotificationPreferences
            //};

            return new UserProfileResponse
            {
            };
        }
    }
}