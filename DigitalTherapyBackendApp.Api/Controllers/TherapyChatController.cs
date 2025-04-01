using DigitalTherapyBackendApp.Api.Features.TherapyChat.Commands;
using DigitalTherapyBackendApp.Api.Features.TherapyChat.Payloads;
using DigitalTherapyBackendApp.Api.Features.TherapyChat.Queries;
using DigitalTherapyBackendApp.Api.Features.TherapyChat.Responses;
using DigitalTherapyBackendApp.Api.Features.TherapySessions.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DigitalTherapyBackendApp.Api.Controllers
{
    [Authorize(Roles = "Patient")]
    [ApiController]
    [Route("api/[controller]")]
    public class TherapyChatController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<TherapyChatController> _logger;

        public TherapyChatController(
            IMediator mediator,
            ILogger<TherapyChatController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Terapi asistanına mesaj gönderir
        /// </summary>
        [HttpPost("SendMessage")]
        public async Task<ActionResult<SendChatMessageResponse>> SendMessage([FromBody] SendChatMessagePayload payload)
        {
            try
            {
                var userId = GetCurrentUserId();

                var result = await _mediator.Send(new SendChatMessageCommand
                {
                    Message = payload.Message,
                    SessionId = payload.SessionId,
                    UserId = userId
                });

                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending chat message");
                return StatusCode(500, new { error = "An error occurred." });
            }
        }

        /// <summary>
        /// Kullanıcının yapay zeka terapi oturumlarını listeler
        /// </summary>
        [HttpGet("GetSessions")]
        public async Task<ActionResult<GetChatSessionsResponse>> GetSessions()
        {
            try
            {
                var userId = GetCurrentUserId();
                var query = new GetChatSessionsQuery { UserId = userId };

                var result = await _mediator.Send(query);

                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting chat sessions");
                return StatusCode(500, new { error = "An error occurred." });
            }
        }

        /// <summary>
        /// Belirli bir terapi oturumunun mesajlarını getirir
        /// </summary>
        [HttpGet("GetMessages/{sessionId}")]
        public async Task<ActionResult<GetChatMessagesResponse>> GetMessages(Guid sessionId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var query = new GetChatMessagesQuery
                {
                    UserId = userId,
                    SessionId = sessionId
                };

                var result = await _mediator.Send(query);

                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting messages for session {sessionId}");
                return StatusCode(500, new { error = "An error occurred." });
            }
        }

        /// <summary>
        /// Yeni bir yapay zeka terapi oturumu başlatır
        /// </summary>
        [HttpPost("StartSession")]
        public async Task<ActionResult<StartChatSessionResponse>> StartSession([FromBody] StartSessionPayload request)
        {
            try
            {
                bool forceNew = request?.ForceNew ?? false;
                var userId = GetCurrentUserId();
                var command = new StartChatSessionCommand { UserId = userId, ForceNew = forceNew };

                var result = await _mediator.Send(command);

                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting new chat session");
                return StatusCode(500, new { error = "An error occurred." });
            }
        }

        /// <summary>
        /// Aktif bir yapay zeka terapi oturumunu sonlandırır
        /// </summary>
        [HttpPost("EndSession/{sessionId}")]
        public async Task<ActionResult<EndChatSessionResponse>> EndSession(Guid sessionId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var command = new EndChatSessionCommand
                {
                    UserId = userId,
                    SessionId = sessionId
                };

                var result = await _mediator.Send(command);

                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error ending session {sessionId}");
                return StatusCode(500, new { error = "An error occurred." });
            }
        }

        /// <summary>
        /// Belirli bir terapi oturumunu aktifleştirir, varsa diğer aktif oturumları deaktif yapar
        /// </summary>
        [HttpPost("ActivateSession/{sessionId}")]
        public async Task<ActionResult<ActivateSessionResponse>> ActivateSession(Guid sessionId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var command = new ActivateSessionCommand
                {
                    UserId = userId,
                    SessionId = sessionId
                };

                var result = await _mediator.Send(command);

                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error activating session {sessionId}");
                return StatusCode(500, new { error = "An error occurred." });
            }
        }

        /// <summary>
        /// Kullanıcıya ait tüm terapi oturumlarını temizler
        /// </summary>
        [HttpPost("ClearAllAiSessions")]
        public async Task<ActionResult<ClearAllAiSessionsResponse>> ClearAllSessions()
        {
            try
            {
                var userId = GetCurrentUserId();
                var command = new ClearAllAiSessionsCommand { UserId = userId };

                var result = await _mediator.Send(command);

                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing all chat sessions");
                return StatusCode(500, new { error = "An error occurred." });
            }
        }

        /// <summary>
        /// Seçili oturumu tamamlar
        /// </summary>
        [HttpPost("CompleteSession/{sessionId}")]
        public async Task<ActionResult<CompleteSessionResponse>> CompleteSession(Guid sessionId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var command = new CompleteSessionCommand
                {
                    UserId = userId,
                    SessionId = sessionId
                };

                var result = await _mediator.Send(command);

                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error completing session {sessionId}");
                return StatusCode(500, new { error = "An error occurred." });
            }
        }


        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedAccessException("Invalid user ID.");
            }

            return userId;
        }
    }
}