using DigitalTherapyBackendApp.Api.Features.TherapyChat.Payloads;
using DigitalTherapyBackendApp.Api.Features.TherapyChat.Responses;
using DigitalTherapyBackendApp.Application.Dtos;
using DigitalTherapyBackendApp.Application.Interfaces;
using DigitalTherapyBackendApp.Domain.Entities;
using DigitalTherapyBackendApp.Domain.Interfaces;
using DigitalTherapyBackendApp.Infrastructure.Repositories;
using MediatR;
using System;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Api.Features.TherapySessions.Commands
{
    public class SendChatMessageCommand : IRequest<SendChatMessageResponse>
    {
        public Guid UserId { get; set; }
        public string Message { get; set; }
        public Guid? SessionId { get; set; }

        public static SendChatMessageCommand FromPayload(SendChatMessagePayload payload, Guid userId, Guid? sessionId)
        {
            return new SendChatMessageCommand
            {
                UserId = userId,
                Message = payload.Message,
                SessionId = payload.SessionId
            };
        }
    }

    public class SendChatMessageCommandHandler : IRequestHandler<SendChatMessageCommand, SendChatMessageResponse>
    {
        private readonly IAiService _aiService;
        private readonly ILogger<SendChatMessageCommandHandler> _logger;
        private readonly ITherapySessionRepository _therapySessionRepository;
        private readonly ISessionMessageRepository _sessionMessageRepository;
        private readonly IConfiguration _configuration;

        public SendChatMessageCommandHandler(
            IAiService aiService,
            ILogger<SendChatMessageCommandHandler> logger,
            ITherapySessionRepository therapySessionRepository,
            ISessionMessageRepository sessionMessageRepository,
            IConfiguration configuration)
        {
            _aiService = aiService;
            _logger = logger;
            _therapySessionRepository = therapySessionRepository;
            _sessionMessageRepository = sessionMessageRepository;
            _configuration = configuration;
        }

        public async Task<SendChatMessageResponse> Handle(SendChatMessageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Message))
                {
                    return new SendChatMessageResponse
                    {
                        Success = false,
                        Message = "Message cannot be empty."
                    };
                }

                TherapySession session;
                if (request.SessionId.HasValue)
                {
                    session = await _therapySessionRepository.GetByIdAsync(request.SessionId.Value);
                    if (session == null || session.PatientId != request.UserId)
                    {
                        return new SendChatMessageResponse
                        {
                            Success = false,
                            Message = "The specified session could not be found or you do not have permission to access it."
                        };
                    }

                    if (!session.IsActive)
                    {
                        var activeSessions = await _therapySessionRepository.GetActiveSessionsAsync(request.UserId);
                        foreach (var activeSession in activeSessions)
                        {
                            if (activeSession.Id != session.Id)
                            {
                                activeSession.IsActive = false;
                                activeSession.EndTime = DateTime.UtcNow;
                                await _therapySessionRepository.UpdateAsync(activeSession);
                            }
                        }

                        session.IsActive = true;
                        if (session.EndTime.HasValue)
                        {
                            session.EndTime = null;
                        }

                        await _therapySessionRepository.UpdateAsync(session);
                        _logger.LogInformation($"Session {session.Id} activated automatically when sending a message");
                    }
                }
                else
                {
                    session = await _therapySessionRepository.GetActiveAiSessionAsync(request.UserId)
                           ?? await _therapySessionRepository.CreateAiSessionAsync(request.UserId);
                }

                string aiResponse = await _aiService.GetChatResponseAsync(request.UserId, request.Message);

                return new SendChatMessageResponse
                {
                    Success = true,
                    Data = new ChatResponseDto
                    {
                        Message = aiResponse,
                        Timestamp = DateTime.UtcNow,
                        SessionId = session.Id
                    },
                    Message = "Message sent successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing chat message for user {request.UserId}");
                return new SendChatMessageResponse
                {
                    Success = false,
                    Message = "An error occurred while processing the message."
                };
            }
        }
    }
}