using DigitalTherapyBackendApp.Api.Features.UserProfiles.Responses;
using DigitalTherapyBackendApp.Domain.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Api.Features.UserProfiles.Commands
{
    public class UpdateUserProfileCommand : IRequest<UserProfileResponse>
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateOnly? BirthDate { get; set; }
        public string Gender { get; set; }
        public string Bio { get; set; }
        public string AvatarUrl { get; set; }
        public string PreferredLanguage { get; set; }
        public string NotificationPreferences { get; set; }
    }

    public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, UserProfileResponse>
    {
        //private readonly IUserProfileRepository _userProfileRepository;

        //public UpdateUserProfileCommandHandler(IUserProfileRepository userProfileRepository)
        //{
        //    _userProfileRepository = userProfileRepository;
        //}

        public async Task<UserProfileResponse> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
        {
            //var userProfile = await _userProfileRepository.GetByUserIdAsync(request.UserId);

            //if (userProfile == null)
            //    throw new InvalidOperationException("User profile not found. Create a profile first.");

            //userProfile.FirstName = request.FirstName;
            //userProfile.LastName = request.LastName;
            //userProfile.BirthDate = request.BirthDate;
            //userProfile.Gender = request.Gender;
            //userProfile.Bio = request.Bio;
            //userProfile.AvatarUrl = request.AvatarUrl;
            //userProfile.PreferredLanguage = request.PreferredLanguage;
            //userProfile.NotificationPreferences = request.NotificationPreferences;

            //await _userProfileRepository.UpdateAsync(userProfile);

            //return new UserProfileResponse
            //{
            //    Id = userProfile.Id,
            //    UserId = userProfile.UserId,
            //    FirstName = userProfile.FirstName,
            //    LastName = userProfile.LastName,
            //    BirthDate = userProfile.BirthDate,
            //    Gender = userProfile.Gender,
            //    Bio = userProfile.Bio,
            //    AvatarUrl = userProfile.AvatarUrl,
            //    PreferredLanguage = userProfile.PreferredLanguage,
            //    NotificationPreferences = userProfile.NotificationPreferences
            //};

            return new UserProfileResponse
            {
            };
        }
    }
}