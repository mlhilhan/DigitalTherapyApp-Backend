using DigitalTherapyBackendApp.Api.Features.Subscriptions.Commands;
using DigitalTherapyBackendApp.Api.Features.Subscriptions.Queries;
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
    public class SubscriptionsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly UserManager<User> _userManager;

        public SubscriptionsController(IMediator mediator, UserManager<User> userManager)
        {
            _mediator = mediator;
            _userManager = userManager;
        }

        [HttpGet("GetSubscriptionPlans")]
        public async Task<IActionResult> GetSubscriptionPlans([FromQuery] string countryCode, [FromQuery] string languageCode)
        {
            var query = new GetSubscriptionPlansQuery(countryCode, languageCode);
            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("GetSubscriptionPlan/{id}")]
        public async Task<IActionResult> GetSubscriptionPlan(int id, [FromQuery] string countryCode, [FromQuery] string languageCode)
        {
            var query = new GetSubscriptionPlanQuery(id, countryCode, languageCode);
            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpGet("GetCurrentUserSubscription")]
        public async Task<IActionResult> GetCurrentUserSubscription()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var query = new GetUserSubscriptionQuery(currentUser.Id);
            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpPost("SubscribeToPlan")]
        public async Task<IActionResult> SubscribeToPlan([FromBody] SubscribeToPlanCommand command)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            command.UserId = currentUser.Id;

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("CancelSubscription")]
        public async Task<IActionResult> CancelSubscription()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var command = new CancelSubscriptionCommand(currentUser.Id);

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("ToggleAutoRenew")]
        public async Task<IActionResult> ToggleAutoRenew([FromBody] ToggleAutoRenewCommand command)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            command.UserId = currentUser.Id;

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("GetBillingHistory")]
        public async Task<IActionResult> GetBillingHistory()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var query = new GetBillingHistoryQuery(currentUser.Id);
            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("CreatePaymentForm")]
        public async Task<IActionResult> CreatePaymentForm([FromBody] CreatePaymentFormCommand command)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            command.UserId = currentUser.Id;

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("PaymentWebhook")]
        [AllowAnonymous]
        public async Task<IActionResult> PaymentWebhook([FromBody] ProcessPaymentWebhookCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("CheckFeatureAccess")]
        public async Task<IActionResult> CheckFeatureAccess([FromQuery] string featureName)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var query = new CheckFeatureAccessQuery(currentUser.Id, featureName);
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        [HttpGet("GetFeatureLimit")]
        public async Task<IActionResult> GetFeatureLimit([FromQuery] string featureName)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var query = new GetFeatureLimitQuery(currentUser.Id, featureName);
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        // Admin endpoints
        [HttpGet("GetAllUserSubscriptions")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUserSubscriptions()
        {
            var query = new GetAllUserSubscriptionsQuery();
            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("AddSubscriptionPlan")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddSubscriptionPlan([FromBody] AddSubscriptionPlanCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPut("UpdateSubscriptionPlan/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateSubscriptionPlan(int id, [FromBody] UpdateSubscriptionPlanCommand command)
        {
            command.Id = id;
            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("AddSubscriptionTranslation")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddSubscriptionTranslation([FromBody] AddSubscriptionTranslationCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("AddSubscriptionPrice")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddSubscriptionPrice([FromBody] AddSubscriptionPriceCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}