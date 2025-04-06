using DigitalTherapyBackendApp.Api.Features.DailyTips.Commands;
using DigitalTherapyBackendApp.Api.Features.DailyTips.Queries;
using DigitalTherapyBackendApp.Api.Features.EmotionalStates.Commands;
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
    public class DailyTipsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly UserManager<User> _userManager;

        public DailyTipsController(IMediator mediator, UserManager<User> userManager)
        {
            _mediator = mediator;
            _userManager = userManager;
        }

        [HttpGet("GetCategories")]
        public async Task<IActionResult> GetCategories([FromQuery] string languageCode = "en")
        {
            var query = new GetDailyTipCategoriesQuery(languageCode);
            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("GetAllTips")]
        public async Task<IActionResult> GetAllTips([FromQuery] string languageCode = "en")
        {
            var query = new GetAllDailyTipsQuery(languageCode);
            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("GetTipsByCategory/{categoryKey}")]
        public async Task<IActionResult> GetTipsByCategory(string categoryKey, [FromQuery] string languageCode = "en")
        {
            var query = new GetDailyTipsByCategoryQuery(categoryKey, languageCode);
            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("GetTipById/{id}")]
        public async Task<IActionResult> GetTipById(int id, [FromQuery] string languageCode = "en")
        {
            var query = new GetDailyTipByIdQuery(id, languageCode);
            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpGet("GetTipOfTheDay")]
        public async Task<IActionResult> GetTipOfTheDay([FromQuery] string languageCode = "en")
        {
            var query = new GetTipOfTheDayQuery(languageCode);
            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("GetBookmarkedTips")]
        [Authorize]
        public async Task<IActionResult> GetBookmarkedTips([FromQuery] string languageCode = "en")
        {
            var query = new GetBookmarkedTipsQuery(languageCode);
            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("ToggleBookmark/{id}")]
        [Authorize]
        public async Task<IActionResult> ToggleBookmark(int id)
        {
            var command = new ToggleDailyTipBookmarkCommand(id);
            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("CreateTip")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateTip([FromBody] CreateDailyTipCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetTipById), new { id = result.Data.Id }, result);
        }

        [HttpPost("CreateCategory")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateDailyTipCategoryCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPut("UpdateTip/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateTip(int id, [FromBody] UpdateDailyTipCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("ID mismatch");
            }

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPut("UpdateCategory/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateDailyTipCategoryCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("ID mismatch");
            }

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpDelete("DeleteTip/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTip(int id)
        {
            var command = new DeleteDailyTipCommand(id);
            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpDelete("DeleteCategory/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var command = new DeleteDailyTipCategoryCommand(id);
            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}