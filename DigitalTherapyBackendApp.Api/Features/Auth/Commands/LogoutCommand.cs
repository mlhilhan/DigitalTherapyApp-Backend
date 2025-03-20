using DigitalTherapyBackendApp.Api.Features.Auth.Payloads;
using DigitalTherapyBackendApp.Api.Features.Auth.Responses;
using DigitalTherapyBackendApp.Application.Interfaces;
using MediatR;

namespace DigitalTherapyBackendApp.Api.Features.Auth.Commands
{
    public class LogoutCommand : IRequest<LogoutResponse>
    {
        public LogoutPayload Payload { get; }

        public LogoutCommand(LogoutPayload payload)
        {
            Payload = payload;
        }
    }


    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, LogoutResponse>
    {
        private readonly ITokenBlacklistService _tokenBlacklistService;

        public LogoutCommandHandler(ITokenBlacklistService tokenBlacklistService)
        {
            _tokenBlacklistService = tokenBlacklistService;
        }

        public async Task<LogoutResponse> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Token add black list
                await _tokenBlacklistService.BlacklistTokenAsync(request.Payload.Token);

                return new LogoutResponse
                {
                    Success = true,
                    Message = "The logout process was completed successfully."
                };
            }
            catch (Exception ex)
            {
                return new LogoutResponse
                {
                    Success = false,
                    Message = $"An error occurred during the logout process: {ex.Message}"
                };
            }
        }
    }
}
