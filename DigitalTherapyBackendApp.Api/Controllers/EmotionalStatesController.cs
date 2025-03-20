using DigitalTherapyBackendApp.Api.Features.EmotionalStates.Commands;
using DigitalTherapyBackendApp.Api.Features.EmotionalStates.Payloads;
using DigitalTherapyBackendApp.Api.Features.EmotionalStates.Queries;
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
    public class EmotionalStatesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EmotionalStatesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("GetUserEmotionalStates")]
        public async Task<IActionResult> GetUserEmotionalStates([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            var query = new GetEmotionalStatesQuery
            {
                UserId = Guid.Parse(userId),
                StartDate = startDate,
                EndDate = endDate
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("GetEmotionalStateById/{id}")]
        public async Task<IActionResult> GetEmotionalStateById(Guid id)
        {
            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            var query = new GetEmotionalStateByIdQuery
            {
                EmotionalStateId = id,
                UserId = Guid.Parse(userId)
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost("CreateEmotionalState")]
        public async Task<IActionResult> CreateEmotionalState([FromBody] CreateEmotionalStatePayload payload)
        {
            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            var command = new CreateEmotionalStateCommand
            {
                UserId = Guid.Parse(userId),
                Mood = payload.Mood,
                MoodIntensity = payload.MoodIntensity,
                Notes = payload.Notes
            };

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("UpdateEmotionalState/{id}")]
        public async Task<IActionResult> UpdateEmotionalState(Guid id, [FromBody] UpdateEmotionalStatePayload payload)
        {
            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            var command = new UpdateEmotionalStateCommand
            {
                EmotionalStateId = id,
                UserId = Guid.Parse(userId),
                Mood = payload.Mood,
                MoodIntensity = payload.MoodIntensity,
                Notes = payload.Notes
            };

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("DeleteEmotionalState/{id}")]
        public async Task<IActionResult> DeleteEmotionalState(Guid id)
        {
            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            var command = new DeleteEmotionalStateCommand
            {
                EmotionalStateId = id,
                UserId = Guid.Parse(userId)
            };

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet("GetEmotionalStateStatistics")]
        public async Task<IActionResult> GetEmotionalStateStatistics([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            var query = new GetEmotionalStateStatisticsQuery
            {
                UserId = Guid.Parse(userId),
                StartDate = startDate ?? DateTime.UtcNow.AddMonths(-1),
                EndDate = endDate ?? DateTime.UtcNow
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}