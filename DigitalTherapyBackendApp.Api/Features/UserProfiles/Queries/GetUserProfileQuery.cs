using DigitalTherapyBackendApp.Api.Features.UserProfiles.Responses;
using DigitalTherapyBackendApp.Domain.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Api.Features.UserProfiles.Queries
{
    public class GetUserProfileQuery : IRequest<UserProfileResponse>
    {
        public Guid UserId { get; set; }
    }

    public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, UserProfileResponse>
    {
        private readonly IUserProfileRepository _userProfileRepository;

        public GetUserProfileQueryHandler(IUserProfileRepository userProfileRepository)
        {
            _userProfileRepository = userProfileRepository;
        }

        public async Task<UserProfileResponse> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
        {
            var userProfile = await _userProfileRepository.GetByUserIdAsync(request.UserId);

            if (userProfile == null)
                return null;

            return new UserProfileResponse
            {
                Id = userProfile.Id,
                UserId = userProfile.UserId,
                FirstName = userProfile.FirstName,
                LastName = userProfile.LastName,
                BirthDate = userProfile.BirthDate,
                Gender = userProfile.Gender,
                Bio = userProfile.Bio,
                AvatarUrl = userProfile.AvatarUrl,
                PreferredLanguage = userProfile.PreferredLanguage,
                NotificationPreferences = userProfile.NotificationPreferences
            };
        }
    }
}