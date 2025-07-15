using MediatR;
using ResourceService.Application.Common.Exceptions;
using ResourceService.Application.Common.Interfaces;

namespace ResourceService.Application.Resource.Commands;

public record DeleteResourceCommand(long Id) : IRequest;

public class DeleteResourceCommandHandler(IResourceRepository resourceRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteResourceCommand>
{
    public async Task Handle(DeleteResourceCommand request, CancellationToken cancellationToken)
    {
        var resource = await resourceRepository.FindAsync(request.Id, cancellationToken);

        if (resource is null || resource.IsArchived)
            throw new NotFoundException(nameof(Resource), request.Id);

        resource.SetArchived();

        var saved = await unitOfWork.SaveChangesAsync(cancellationToken);
        if (saved <= 0)
            throw new InternalServerException();

    }
}