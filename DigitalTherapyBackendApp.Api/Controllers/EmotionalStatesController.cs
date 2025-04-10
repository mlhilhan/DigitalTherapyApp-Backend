using DigitalTherapyBackendApp.Api.Attributes;
using DigitalTherapyBackendApp.Api.Features.EmotionalStates.Commands;
using DigitalTherapyBackendApp.Api.Features.EmotionalStates.Payloads;
using DigitalTherapyBackendApp.Api.Features.EmotionalStates.Queries;
using DigitalTherapyBackendApp.Application.Interfaces;
using DigitalTherapyBackendApp.Domain.Interfaces;
using DigitalTherapyBackendApp.Infrastructure.ExternalServices;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DigitalTherapyBackendApp.Api.Controllers
{
    [Authorize(Roles = "Patient")]
    [ApiController]
    [Route("api/[controller]")]
    public class EmotionalStatesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ISubscriptionService _subscriptionService;
        private readonly IEmotionalStateService _emotionalStateService;

        public EmotionalStatesController(IMediator mediator, ISubscriptionService subscriptionService, IEmotionalStateService emotionalStateService)
        {
            _mediator = mediator;
            _subscriptionService = subscriptionService;
            _emotionalStateService = emotionalStateService;
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
        [SubscriptionFeature("advanced_mood_views", 2)]
        public async Task<IActionResult> GetStatistics([FromQuery] GetEmotionalStateStatisticsPayload payload)
        {
            var query = new GetEmotionalStateStatisticsQuery(GetUserId(), payload.StartDate, payload.EndDate);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost("CreateEmotionalState")]
        [SubscriptionFeature("mood_entry", 1, false)]
        public async Task<IActionResult> CreateEmotionalState([FromBody] CreateEmotionalStatePayload payload)
        {
            var userId = GetUserId();
            DateTime selectedDate = payload.Date.Date;

            int existingEntryCount = await _emotionalStateService.GetActiveEntryCountForDateAsync(userId, selectedDate);

            var subscription = await _subscriptionService.GetUserActiveSubscriptionAsync(userId);
            int limit = 1;

            if (subscription?.Subscription?.PlanId != "free" &&
                subscription?.Subscription?.PlanId != null)
            {
                limit = 99999;
            }

            if (existingEntryCount >= limit)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "You have reached your daily limit for this feature.",
                    ErrorCode = "DAILY_LIMIT_REACHED"
                });
            }

            var command = new CreateEmotionalStateCommand(payload, userId);
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
            var emotionalState = await _mediator.Send(new GetEmotionalStateQuery(id, GetUserId()));
            if (!emotionalState.Success)
            {
                return NotFound(emotionalState);
            }

            var command = new DeleteEmotionalStateCommand(id, GetUserId());
            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            if (emotionalState.Data != null)
            {
                DateTime entryDate = emotionalState.Data.Date;
                await _subscriptionService.MarkFeatureUsageAsDeletedAsync(GetUserId(), "mood_entry", entryDate);
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

        [HttpGet("CheckFeatureAccess")]
        public async Task<IActionResult> CheckFeatureAccess([FromQuery] string featureName)
        {
            bool hasAccess = await _subscriptionService.CheckUserFeatureAccessAsync(GetUserId(), featureName);

            return Ok(new
            {
                Success = true,
                Data = new
                {
                    FeatureName = featureName,
                    HasAccess = hasAccess
                }
            });
        }

        [HttpGet("GetFeatureLimit")]
        public async Task<IActionResult> GetFeatureLimit([FromQuery] string featureName, [FromQuery] string limitType = "daily")
        {
            var limitResult = await _subscriptionService.CheckFeatureLimitAsync(GetUserId(), featureName, limitType);

            return Ok(new
            {
                Success = true,
                Data = new
                {
                    FeatureName = featureName,
                    LimitType = limitType,
                    CurrentUsage = limitResult.CurrentUsage,
                    Limit = limitResult.Limit,
                    RemainingUses = limitResult.Limit == -1 ? -1 : limitResult.Limit - limitResult.CurrentUsage,
                    ResetTime = limitResult.ResetTime
                }
            });
        }
    }
}