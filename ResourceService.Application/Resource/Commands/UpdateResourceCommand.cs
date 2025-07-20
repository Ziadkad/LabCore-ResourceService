using AutoMapper;
using MediatR;
using ResourceService.Application.Common.Exceptions;
using ResourceService.Application.Common.Interfaces;
using ResourceService.Domain.Resource;

namespace ResourceService.Application.Resource.Commands;

public record UpdateResourceCommand(
    long Id,
    string Name,
    ResourceType Type,
    string? Description,
    int? QuantityAvailable,
    ResourceStatus Status
) : IRequest<ResourceDto>;



public class UpdateResourceCommandHandler(
    IResourceRepository resourceRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper)
    : IRequestHandler<UpdateResourceCommand, ResourceDto>
{
    public async Task<ResourceDto> Handle(UpdateResourceCommand request, CancellationToken cancellationToken)
    {
        var resource = await resourceRepository.FindAsync(request.Id, cancellationToken);

        if (resource is null || resource.IsArchived)
            throw new NotFoundException(nameof(Resource), request.Id);

        
        int? quantityAvailable = null;
        if (request.Type is ResourceType.Consumable)
        {
            if (request.QuantityAvailable is null)
            {
                throw new BadRequestException("QuantityAvailable is required if it's a Consumable.");
            }
            quantityAvailable = request.QuantityAvailable;
        }

        
        resource.Update(request.Name, request.Type, request.Description, quantityAvailable, request.Status);

        resourceRepository.Update(resource);

        var saved = await unitOfWork.SaveChangesAsync(cancellationToken);
        if (saved <= 0)
            throw new InternalServerException();

        return mapper.Map<ResourceDto>(resource);
    }
}