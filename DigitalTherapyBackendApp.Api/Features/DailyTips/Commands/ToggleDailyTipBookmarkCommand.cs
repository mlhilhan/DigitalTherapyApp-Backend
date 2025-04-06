using DigitalTherapyBackendApp.Api.Features.DailyTips.Responses;
using DigitalTherapyBackendApp.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Api.Features.DailyTips.Commands
{
    public class ToggleDailyTipBookmarkCommand : IRequest<ToggleBookmarkResponse>
    {
        public int TipId { get; }

        public ToggleDailyTipBookmarkCommand(int tipId)
        {
            TipId = tipId;
        }
    }

    public class ToggleDailyTipBookmarkCommandHandler : IRequestHandler<ToggleDailyTipBookmarkCommand, ToggleBookmarkResponse>
    {
        private readonly IDailyTipRepository _dailyTipRepository;
        private readonly ILogger<ToggleDailyTipBookmarkCommandHandler> _logger;

        public ToggleDailyTipBookmarkCommandHandler(
            IDailyTipRepository dailyTipRepository,
            ILogger<ToggleDailyTipBookmarkCommandHandler> logger)
        {
            _dailyTipRepository = dailyTipRepository;
            _logger = logger;
        }

        public async Task<ToggleBookmarkResponse> Handle(
            ToggleDailyTipBookmarkCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var tip = await _dailyTipRepository.GetTipByIdAsync(request.TipId);
                if (tip == null)
                {
                    return new ToggleBookmarkResponse
                    {
                        Success = false,
                        Message = $"Tip with ID {request.TipId} not found."
                    };
                }

                var isBookmarked = await _dailyTipRepository.ToggleBookmarkAsync(request.TipId);

                return new ToggleBookmarkResponse
                {
                    Success = true,
                    Message = isBookmarked ? "Tip bookmarked successfully." : "Tip removed from bookmarks.",
                    IsBookmarked = isBookmarked
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling bookmark for tip {TipId}", request.TipId);
                return new ToggleBookmarkResponse
                {
                    Success = false,
                    Message = "An error occurred while toggling the bookmark."
                };
            }
        }
    }
}