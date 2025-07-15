using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using ResourceService.Application.Common.Exceptions;
using ResourceService.Application.Common.Interfaces;
using ResourceService.Application.ResourceReservation.Models;
using ResourceService.Domain.Resource;
using Shared.Models;

namespace ResourceService.Application.ResourceReservation.Commands;

public record CreateResourceReservationCommand(long ResourceId, Guid? TaskItemId, DateTime StartTime, DateTime EndTime, string? Notes, int? Quantity):IRequest<ResourceReservationDto>;

public class CreateResourceReservationCommandValidator : AbstractValidator<CreateResourceReservationCommand>
{
    public CreateResourceReservationCommandValidator()
    {
        RuleFor(x => x.ResourceId)
            .GreaterThan(0).WithMessage("ResourceId must be greater than 0.");

        RuleFor(x => x.StartTime)
            .LessThan(x => x.EndTime).WithMessage("StartTime must be before EndTime.");

        RuleFor(x => x.StartTime)
            .GreaterThanOrEqualTo(DateTime.UtcNow).WithMessage("StartTime must be in the future.");

        RuleFor(x => x.EndTime)
            .GreaterThanOrEqualTo(x => x.StartTime).WithMessage("EndTime must be after StartTime.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .When(x => x.Quantity.HasValue)
            .WithMessage("Quantity must be greater than 0 if specified.");
    }
}

public class CreateResourceReservationCommandHandler(
    IResourceReservationRepository resourceReservationRepository,
    IResourceRepository resourceRepository,
    IUserContext userContext,
    IUnitOfWork unitOfWork,
    IReservationProducer reservationProducer,
    ILogger<CreateResourceReservationCommandHandler> logger,
    IMapper mapper)
    : IRequestHandler<CreateResourceReservationCommand, ResourceReservationDto>
{
    public async Task<ResourceReservationDto> Handle(CreateResourceReservationCommand request, CancellationToken cancellationToken)
    {
        // Retrieve and validate the resource
        var resource = await resourceRepository.FindAsync(request.ResourceId, cancellationToken);
        if (resource is null || resource.IsArchived)
            throw new NotFoundException(nameof(Resource), request.ResourceId);

        if (resource.Status != ResourceStatus.Available)
            throw new BadRequestException("The resource is not available.");

        // Case 1: Resource is quantity-based
        if (resource.QuantityAvailable is not null)
        {
            if (request.Quantity is null)
                throw new BadRequestException("This resource requires a quantity to be specified.");

            if (request.Quantity > resource.QuantityAvailable)
                throw new BadRequestException($"Not enough quantity of '{resource.Name}' available.");

            var updatedQuantity = resource.QuantityAvailable.Value - request.Quantity.Value;

            // Update resource with new quantity
            resource.Update(resource.Name, resource.Type, resource.Description, updatedQuantity, resource.Status);
        }
        else // Case 2: Resource is time-slot-based
        {
            var isAvailable = await resourceReservationRepository.IsResourceAvailableAsync(
                request.ResourceId, request.StartTime, request.EndTime, cancellationToken);

            if (!isAvailable)
                throw new BadRequestException($"The resource is not available between {request.StartTime} and {request.EndTime}.");
        }

        // Create reservation entity
        var reservation = new Domain.ResourceReservation.ResourceReservation(
            request.ResourceId,
            userContext.GetCurrentUserId(),
            request.TaskItemId,
            request.StartTime,
            request.EndTime,
            request.Notes,
            request.Quantity
        );

        await resourceReservationRepository.AddAsync(reservation, cancellationToken);

        var result = await unitOfWork.SaveChangesAsync(cancellationToken);
        if (result <= 0)
            throw new InternalServerException();
        
        if (reservation.TaskItemId is not null && reservation.TaskItemId != Guid.Empty)
        {
            var message = new ResourceReservationMessage()
            {
                ResourceId = reservation.Id,
                TaskItemId = request.TaskItemId.Value,
            };
            await reservationProducer.SendAsync(message);
        }

        return mapper.Map<ResourceReservationDto>(reservation);
    }
}
