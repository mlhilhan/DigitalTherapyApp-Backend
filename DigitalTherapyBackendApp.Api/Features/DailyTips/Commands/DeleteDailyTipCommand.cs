using DigitalTherapyBackendApp.Api.Features.DailyTips.Responses;
using DigitalTherapyBackendApp.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Api.Features.DailyTips.Commands
{
    public class DeleteDailyTipCommand : IRequest<DeleteDailyTipResponse>
    {
        public int Id { get; }

        public DeleteDailyTipCommand(int id)
        {
            Id = id;
        }
    }

    public class DeleteDailyTipCommandHandler : IRequestHandler<DeleteDailyTipCommand, DeleteDailyTipResponse>
    {
        private readonly IDailyTipRepository _dailyTipRepository;
        private readonly ILogger<DeleteDailyTipCommandHandler> _logger;

        public DeleteDailyTipCommandHandler(
            IDailyTipRepository dailyTipRepository,
            ILogger<DeleteDailyTipCommandHandler> logger)
        {
            _dailyTipRepository = dailyTipRepository;
            _logger = logger;
        }

        public async Task<DeleteDailyTipResponse> Handle(
            DeleteDailyTipCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var tip = await _dailyTipRepository.GetTipByIdAsync(request.Id);
                if (tip == null)
                {
                    return new DeleteDailyTipResponse
                    {
                        Success = false,
                        Message = $"Tip with ID {request.Id} not found."
                    };
                }

                bool success = await _dailyTipRepository.DeleteTipAsync(request.Id);
                if (!success)
                {
                    return new DeleteDailyTipResponse
                    {
                        Success = false,
                        Message = "Failed to delete the tip."
                    };
                }

                return new DeleteDailyTipResponse
                {
                    Success = true,
                    Message = "Daily tip deleted successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting daily tip with ID {TipId}", request.Id);
                return new DeleteDailyTipResponse
                {
                    Success = false,
                    Message = "An error occurred while deleting the daily tip."
                };
            }
        }
    }
}