using DigitalTherapyBackendApp.Api.Controllers;
using DigitalTherapyBackendApp.Api.Features.EmotionalStates.Commands;
using DigitalTherapyBackendApp.Api.Features.EmotionalStates.Queries;
using DigitalTherapyBackendApp.Api.Features.PatientProfiles.Queries;
using DigitalTherapyBackendApp.Api.Features.PsychologistProfiles.Queries;
using DigitalTherapyBackendApp.Application.Dtos;
using DigitalTherapyBackendApp.Domain.Entities;
using DigitalTherapyBackendApp.Infrastructure.ExternalServices;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DigitalTherapyBackendApp.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EmotionalStatesController : ControllerBase
    {
        private readonly IEmotionalStateService _emotionalStateService;
        private readonly IMediator _mediator;
        private readonly UserManager<User> _userManager;

        public EmotionalStatesController(IEmotionalStateService emotionalStateService, IMediator mediator, UserManager<User> userManager)
        {
            _emotionalStateService = emotionalStateService;
            _mediator = mediator;
            _userManager = userManager;
        }

        private Guid GetUserId()
        {
            try
            {
                var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (Guid.TryParse(userIdString, out var userId))
                {
                    return userId;
                }
                throw new Exception("Invalid GUID format.");
            } catch (Exception ex) 
            {
                throw new Exception("Error: " + ex.Message);
            }
        }

        //[HttpGet("GetEmotionalStates")]
        //public async Task<ActionResult<IEnumerable<EmotionalStateDto>>> GetEmotionalStates([FromQuery] GetEmotionalStatesQuery query)
        //{
        //    var userId = query.UserId;

        //    if (query.StartDate.HasValue && query.EndDate.HasValue)
        //    {
        //        return Ok(await _emotionalStateService.GetByDateRangeAsync(userId, query.StartDate.Value, query.EndDate.Value));
        //    }

        //    return Ok(await _emotionalStateService.GetAllByUserIdAsync(userId));
        //}

        [HttpGet("GetEmotionalStates")]
        public async Task<IActionResult> GetEmotionalStates()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var query = new GetEmotionalStatesQuery(currentUser.Id);
            var result = await _mediator.Send(query);

            return Ok(result);
        }


        [HttpGet("GetEmotionalStates")]
        public async Task<IActionResult> GetEmotionalStates(Guid userId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            userId = currentUser.Id;

            var query = new GetEmotionalStatesQuery(userId, startDate, endDate);
            var result = await _mediator.Send(query);

            return Ok(result);
        }


        [HttpGet("GetEmotionalState/{id}")]
        public async Task<ActionResult<EmotionalStateDto>> GetEmotionalState(Guid id)
        {
            var userId = GetUserId();
            var emotionalState = await _emotionalStateService.GetByIdAsync(id, userId);

            if (emotionalState == null)
            {
                return NotFound();
            }

            return Ok(emotionalState);
        }

        [HttpGet("GetBookmarked")]
        public async Task<ActionResult<IEnumerable<EmotionalStateDto>>> GetBookmarked()
        {
            var userId = GetUserId();
            return Ok(await _emotionalStateService.GetBookmarkedAsync(userId));
        }

        [HttpGet("GetStatistics")]
        public async Task<ActionResult<EmotionalStateStatisticsDto>> GetStatistics([FromQuery] GetEmotionalStateStatisticsQuery query)
        {
            var userId = GetUserId();
            return Ok(await _emotionalStateService.GetStatisticsAsync(userId, query.StartDate, query.EndDate));
        }

        [HttpPost("CreateEmotionalState")]
        public async Task<ActionResult<int>> CreateEmotionalState([FromBody] CreateEmotionalStateCommand command)
        {
            var userId = command.Payload.UserId;
            var dto = new CreateEmotionalStateDto
            {
                MoodLevel = command.Payload.MoodLevel,
                Factors = command.Payload.Factors,
                Notes = command.Payload.Notes,
                Date = command.Payload.Date,
                IsBookmarked = command.Payload.IsBookmarked
            };

            var id = await _emotionalStateService.CreateAsync(dto, userId);
            return CreatedAtAction(nameof(GetEmotionalState), new { id = id }, id);
        }

        [HttpPut("UpdateEmotionalState/{id}")]
        public async Task<IActionResult> UpdateEmotionalState(Guid id, [FromBody] UpdateEmotionalStateCommand command)
        {
            var userId = command.UserId;
            var dto = new UpdateEmotionalStateDto
            {
                MoodLevel = command.Payload.MoodLevel,
                Factors = command.Payload.Factors,
                Notes = command.Payload.Notes,
                Date = command.Payload.Date,
                IsBookmarked = command.Payload.IsBookmarked
            };

            var result = await _emotionalStateService.UpdateAsync(id, dto, userId);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("DeleteEmotionalState/{id}")]
        public async Task<IActionResult> DeleteEmotionalState(Guid id)
        {
            var userId = GetUserId();
            var result = await _emotionalStateService.DeleteAsync(id, userId);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPatch("{id}/ToggleBookmark")]
        public async Task<IActionResult> ToggleBookmark(Guid id)
        {
            var userId = GetUserId();
            var result = await _emotionalStateService.ToggleBookmarkAsync(id, userId);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}