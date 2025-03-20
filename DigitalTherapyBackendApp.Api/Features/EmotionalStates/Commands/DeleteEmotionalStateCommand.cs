using DigitalTherapyBackendApp.Domain.Interfaces;
using MediatR;
using System;

namespace DigitalTherapyBackendApp.Api.Features.EmotionalStates.Commands
{
    public class DeleteEmotionalStateCommand : IRequest<bool>
    {
        public Guid EmotionalStateId { get; set; }
        public Guid UserId { get; set; }
    }


    public class DeleteEmotionalStateCommandHandler : IRequestHandler<DeleteEmotionalStateCommand, bool>
    {
        private readonly IEmotionalStateRepository _emotionalStateRepository;

        public DeleteEmotionalStateCommandHandler(IEmotionalStateRepository emotionalStateRepository)
        {
            _emotionalStateRepository = emotionalStateRepository;
        }

        public async Task<bool> Handle(DeleteEmotionalStateCommand request, CancellationToken cancellationToken)
        {
            var emotionalState = await _emotionalStateRepository.GetByIdAsync(request.EmotionalStateId);

            if (emotionalState == null || emotionalState.UserId != request.UserId)
                throw new UnauthorizedAccessException("You do not have permission to delete this emotional state record.");

            await _emotionalStateRepository.DeleteAsync(request.EmotionalStateId);

            return true;
        }
    }
}