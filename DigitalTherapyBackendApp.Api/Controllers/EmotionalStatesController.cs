using DigitalTherapyBackendApp.Api.Features.EmotionalStates.Commands;
using DigitalTherapyBackendApp.Api.Features.EmotionalStates.Payloads;
using DigitalTherapyBackendApp.Api.Features.EmotionalStates.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DigitalTherapyBackendApp.Api.Controllers
{
    [Authorize(Roles = "Patient")]
    [ApiController]
    [Route("api/[controller]")]
    public class EmotionalStatesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EmotionalStatesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        private Guid GetUserId()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(userIdString, out Guid userId))
            {
                return userId;
            }

            throw new UnauthorizedAccessException("User ID could not be determined.");
        }

        [HttpGet("GetEmotionalStates")]
        public async Task<IActionResult> GetEmotionalStates([FromQuery] GetEmotionalStatesPayload payload)
        {
            var query = new GetEmotionalStatesQuery(GetUserId(), payload.StartDate, payload.EndDate);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("GetEmotionalStates/{id}")]
        public async Task<IActionResult> GetEmotionalState(Guid id)
        {
            var query = new GetEmotionalStateQuery(id, GetUserId());
            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpGet("GetBookmarked")]
        public async Task<IActionResult> GetBookmarked()
        {
            var query = new GetBookmarkedEmotionalStatesQuery(GetUserId());
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("GetStatistics")]
        public async Task<IActionResult> GetStatistics([FromQuery] GetEmotionalStateStatisticsPayload payload)
        {
            var query = new GetEmotionalStateStatisticsQuery(GetUserId(), payload.StartDate, payload.EndDate);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost("CreateEmotionalState")]
        public async Task<IActionResult> CreateEmotionalState([FromBody] CreateEmotionalStatePayload payload)
        {
            var command = new CreateEmotionalStateCommand(payload, GetUserId());
            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetEmotionalState), new { id = result.Data.Id }, result);
        }

        [HttpPut("UpdateEmotionalState/{id}")]
        public async Task<IActionResult> UpdateEmotionalState(Guid id, [FromBody] UpdateEmotionalStatePayload payload)
        {
            var command = new UpdateEmotionalStateCommand(id, payload, GetUserId());
            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                if (result.ErrorCode == "EMOTIONALSTATE_NOT_FOUND")
                {
                    return NotFound(result);
                }

                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpDelete("DeleteEmotionalState/{id}")]
        public async Task<IActionResult> DeleteEmotionalState(Guid id)
        {
            var command = new DeleteEmotionalStateCommand(id, GetUserId());
            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                if (result.ErrorCode == "EMOTIONALSTATE_NOT_FOUND")
                {
                    return NotFound(result);
                }

                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPatch("{id}/ToggleBookmark")]
        public async Task<IActionResult> ToggleBookmark(Guid id)
        {
            var command = new ToggleBookmarkCommand(id, GetUserId());
            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                if (result.ErrorCode == "EMOTIONALSTATE_NOT_FOUND")
                {
                    return NotFound(result);
                }

                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}