using DigitalTherapyBackendApp.Api.Features.PatientProfiles.Commands;
using DigitalTherapyBackendApp.Api.Features.PatientProfiles.Queries;
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
    public class PatientProfilesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly UserManager<User> _userManager;

        public PatientProfilesController(IMediator mediator, UserManager<User> userManager)
        {
            _mediator = mediator;
            _userManager = userManager;
        }

        [HttpGet("GetPatientProfile/{id}")]
        public async Task<IActionResult> GetPatientProfile(Guid id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(currentUser);

            if (currentUser.Id != id && !roles.Contains("Admin") && !roles.Contains("Psychologist") && !roles.Contains("Institution"))
            {
                return Forbid();
            }

            var query = new GetPatientProfileQuery(id);
            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpGet("GetCurrentPatientProfile")]
        public async Task<IActionResult> GetCurrentPatientProfile()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var query = new GetPatientProfileQuery(currentUser.Id);
            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpPut("UpdatePatientProfile/{id}")]
        public async Task<IActionResult> UpdatePatientProfile(Guid id, [FromBody] UpdatePatientProfileCommand command)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser.Id != id)
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
        public async Task<IActionResult> UploadProfileImage(Guid id, [FromForm] UploadPatientProfileImageCommand command)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser.Id != id)
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

        [HttpGet("GetPatientsByPsychologist/{psychologistId}")]
        [Authorize(Roles = "Psychologist,Admin")]
        public async Task<IActionResult> GetPatientsByPsychologist(Guid psychologistId)
        {
            var query = new GetPatientsByPsychologistQuery(psychologistId);
            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("GetPatientsByInstitution/{institutionId}")]
        [Authorize(Roles = "Institution,Admin")]
        public async Task<IActionResult> GetPatientsByInstitution(Guid institutionId)
        {
            var query = new GetPatientsByInstitutionQuery(institutionId);
            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}