using DigitalTherapyBackendApp.Api.Features.DailyTips.Responses;
using DigitalTherapyBackendApp.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Api.Features.DailyTips.Commands
{
    public class DeleteDailyTipCategoryCommand : IRequest<DeleteDailyTipCategoryResponse>
    {
        public int Id { get; }

        public DeleteDailyTipCategoryCommand(int id)
        {
            Id = id;
        }
    }

    public class DeleteDailyTipCategoryCommandHandler : IRequestHandler<DeleteDailyTipCategoryCommand, DeleteDailyTipCategoryResponse>
    {
        private readonly IDailyTipRepository _dailyTipRepository;
        private readonly ILogger<DeleteDailyTipCategoryCommandHandler> _logger;

        public DeleteDailyTipCategoryCommandHandler(
            IDailyTipRepository dailyTipRepository,
            ILogger<DeleteDailyTipCategoryCommandHandler> logger)
        {
            _dailyTipRepository = dailyTipRepository;
            _logger = logger;
        }

        public async Task<DeleteDailyTipCategoryResponse> Handle(
            DeleteDailyTipCategoryCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var category = await _dailyTipRepository.GetCategoryByIdAsync(request.Id);
                if (category == null)
                {
                    return new DeleteDailyTipCategoryResponse
                    {
                        Success = false,
                        Message = $"Category with ID {request.Id} not found."
                    };
                }

                var tips = await _dailyTipRepository.GetTipsByCategoryAsync(category.CategoryKey);
                if (tips.Count > 0)
                {
                    return new DeleteDailyTipCategoryResponse
                    {
                        Success = false,
                        Message = $"Cannot delete category. It has {tips.Count} associated tip(s). Delete the tips first."
                    };
                }

                bool success = await _dailyTipRepository.DeleteCategoryAsync(request.Id);
                if (!success)
                {
                    return new DeleteDailyTipCategoryResponse
                    {
                        Success = false,
                        Message = "Failed to delete the category."
                    };
                }

                return new DeleteDailyTipCategoryResponse
                {
                    Success = true,
                    Message = "Category deleted successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category with ID {CategoryId}", request.Id);
                return new DeleteDailyTipCategoryResponse
                {
                    Success = false,
                    Message = "An error occurred while deleting the category."
                };
            }
        }
    }
}