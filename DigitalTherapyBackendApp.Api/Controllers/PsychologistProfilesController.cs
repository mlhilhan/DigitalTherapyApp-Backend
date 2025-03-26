using DigitalTherapyBackendApp.Api.Features.PsychologistProfiles.Commands;
using DigitalTherapyBackendApp.Api.Features.PsychologistProfiles.Queries;
using DigitalTherapyBackendApp.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PsychologistProfilesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly UserManager<User> _userManager;

        public PsychologistProfilesController(IMediator mediator, UserManager<User> userManager)
        {
            _mediator = mediator;
            _userManager = userManager;
        }

        [HttpGet("GetPsychologistProfile/{id}")]
        public async Task<IActionResult> GetPsychologistProfile(Guid id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(currentUser);

            if (currentUser.Id != id && !roles.Contains("Admin") && !roles.Contains("Institution"))
            {
                return Forbid();
            }

            var query = new GetPsychologistProfileQuery(id);
            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpGet("GetCurrentPsychologistProfile")]
        public async Task<IActionResult> GetCurrentPsychologistProfile()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var query = new GetPsychologistProfileQuery(currentUser.Id);
            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpPut("UpdatePsychologistProfile/{id}")]
        public async Task<IActionResult> UpdatePsychologistProfile(Guid id, [FromBody] UpdatePsychologistProfileCommand command)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(currentUser);

            if (currentUser.Id != id && !roles.Contains("Admin") && !roles.Contains("Institution"))
            {
                return Forbid();
            }

            command.UserId = id;
            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("UploadProfileImage/{id}/avatar")]
        public async Task<IActionResult> UploadProfileImage(Guid id, [FromForm] UploadPsychologistProfileImageCommand command)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(currentUser);

            if (currentUser.Id != id && !roles.Contains("Admin") && !roles.Contains("Institution"))
            {
                return Forbid();
            }

            command.UserId = id;
            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("GetPsychologistsByInstitution/{institutionId}")]
        [Authorize(Roles = "Institution,Admin")]
        public async Task<IActionResult> GetPsychologistsByInstitution(Guid institutionId)
        {
            var query = new GetPsychologistsByInstitutionQuery(institutionId);
            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("GetAvailablePsychologists")]
        public async Task<IActionResult> GetAvailablePsychologists()
        {
            var query = new GetAvailablePsychologistsQuery();
            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("GetPsychologistsBySpecialty/{specialty}")]
        public async Task<IActionResult> GetPsychologistsBySpecialty(string specialty)
        {
            var query = new GetPsychologistsBySpecialtyQuery(specialty);
            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPut("UpdatePsychologistSpecialties/{id}")]
        [Authorize(Roles = "Psychologist,Admin")]
        public async Task<IActionResult> UpdatePsychologistSpecialties(Guid id, [FromBody] UpdatePsychologistSpecialtiesCommand command)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(currentUser);

            if (currentUser.Id != id && !roles.Contains("Admin"))
            {
                return Forbid();
            }

            command.PsychologistId = id;
            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPut("UpdatePsychologistAvailability/{id}")]
        [Authorize(Roles = "Psychologist,Admin")]
        public async Task<IActionResult> UpdatePsychologistAvailability(Guid id, [FromBody] UpdatePsychologistAvailabilityCommand command)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(currentUser);

            if (currentUser.Id != id && !roles.Contains("Admin"))
            {
                return Forbid();
            }

            command.PsychologistId = id;
            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}