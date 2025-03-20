using DigitalTherapyBackendApp.Api.Features.UserProfiles.Commands;
using DigitalTherapyBackendApp.Api.Features.UserProfiles.Payloads;
using DigitalTherapyBackendApp.Api.Features.UserProfiles.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserProfilesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserProfilesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("GetUserProfile")]
        public async Task<IActionResult> GetUserProfile()
        {
            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            var query = new GetUserProfileQuery
            {
                UserId = Guid.Parse(userId)
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost("CreateUserProfile")]
        public async Task<IActionResult> CreateUserProfile([FromBody] CreateUserProfilePayload payload)
        {
            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            var command = new CreateUserProfileCommand
            {
                UserId = Guid.Parse(userId),
                FirstName = payload.FirstName,
                LastName = payload.LastName,
                BirthDate = payload.BirthDate,
                Gender = payload.Gender,
                Bio = payload.Bio,
                AvatarUrl = payload.AvatarUrl,
                PreferredLanguage = payload.PreferredLanguage,
                NotificationPreferences = payload.NotificationPreferences
            };

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("UpdateUserProfile")]
        public async Task<IActionResult> UpdateUserProfile([FromBody] UpdateUserProfilePayload payload)
        {
            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            var command = new UpdateUserProfileCommand
            {
                UserId = Guid.Parse(userId),
                FirstName = payload.FirstName,
                LastName = payload.LastName,
                BirthDate = payload.BirthDate,
                Gender = payload.Gender,
                Bio = payload.Bio,
                AvatarUrl = payload.AvatarUrl,
                PreferredLanguage = payload.PreferredLanguage,
                NotificationPreferences = payload.NotificationPreferences
            };

            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}