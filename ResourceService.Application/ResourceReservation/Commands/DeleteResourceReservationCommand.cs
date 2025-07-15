using MediatR;
using ResourceService.Application.Common.Exceptions;
using ResourceService.Application.Common.Interfaces;
using ResourceService.Application.Common.Models;
using Shared.Models;

namespace ResourceService.Application.ResourceReservation.Commands;

public record DeleteResourceReservationCommand(long Id) : IRequest;


public class DeleteResourceReservationCommandHandler(IResourceReservationRepository resourceReservationRepository,
    IResourceRepository resourceRepository,
    IUserContext userContext,
    IReservationProducer reservationProducer,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteResourceReservationCommand>
{
    public async Task Handle(DeleteResourceReservationCommand request, CancellationToken cancellationToken)
    {
        Domain.ResourceReservation.ResourceReservation? resourceReservation = await resourceReservationRepository.FindAsync(request.Id, cancellationToken);
        if (resourceReservation == null)
        {
            throw new NotFoundException(nameof(resourceReservation), request.Id);
        }
        
        if (resourceReservation.ReservedBy != userContext.GetCurrentUserId() || userContext.GetUserRole() == UserRole.ResourceManager)
        {
            throw new ForbiddenException();
        }

        if (resourceReservation.EndTime < DateTime.UtcNow)
        {
            throw new BadRequestException("Cannot delete a reservation that has already ended.");
        }
        
        if (resourceReservation.Quantity != null)
        {
            Domain.Resource.Resource? resource =
                await resourceRepository.FindAsync(resourceReservation.ResourceId, cancellationToken);
            if (resource is null || resource.IsArchived)
                throw new NotFoundException(nameof(Resource), resourceReservation.ResourceId);
            
            var updatedQuantity = resource.QuantityAvailable.Value + resourceReservation.Quantity.Value;
            resource.Update(resource.Name, resource.Type, resource.Description, updatedQuantity, resource.Status);
        }

        var id = resourceReservation.Id;
        var taskItem = resourceReservation.TaskItemId;
        
        resourceReservationRepository.Remove(resourceReservation);
        var result = await unitOfWork.SaveChangesAsync(cancellationToken);
        if (result <= 0)
            throw new InternalServerException();
        
        if (taskItem is not null)
        {
            var message = new ResourceReservationMessage()
            {
                ResourceId = id,
                TaskItemId = taskItem.Value,
            };
            await reservationProducer.SendDeleteAsync(message);
        }
    }
}