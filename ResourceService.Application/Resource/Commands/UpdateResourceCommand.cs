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

        resource.Update(request.Name, request.Type, request.Description, request.QuantityAvailable, request.Status);

        resourceRepository.Update(resource);

        var saved = await unitOfWork.SaveChangesAsync(cancellationToken);
        if (saved <= 0)
            throw new InternalServerException();

        return mapper.Map<ResourceDto>(resource);
    }
}