using MediatR;
using Microsoft.AspNetCore.Mvc;
using DigitalTherapyBackendApp.Api.Features.Auth.Payloads;
using DigitalTherapyBackendApp.Api.Features.Auth.Commands;
using Microsoft.AspNetCore.Authorization;

namespace DigitalTherapyBackendApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController: ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserPayload payload)
        {
            var command = new CreateUserCommand(payload);
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginPayload payload)
        {
            var command = new LoginCommand(payload);
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [HttpPost("Logout")]
        [Authorize]
        public async Task<IActionResult> Logout([FromBody] LogoutPayload payload)
        {
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (string.IsNullOrEmpty(token))
                return BadRequest(new { message = "Token is required" });
            payload.Token = token;

            var command = new LogoutCommand(payload);
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [HttpPost("RefreshToken")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenPayload payload)
        {
            var command = new RefreshTokenCommand(payload);
            var result = await _mediator.Send(command);

            return Ok(result);
        }
    }
}
