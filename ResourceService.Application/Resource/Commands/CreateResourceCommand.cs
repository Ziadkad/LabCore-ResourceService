using AutoMapper;
using FluentValidation;
using MediatR;
using ResourceService.Application.Common.Exceptions;
using ResourceService.Application.Common.Interfaces;
using ResourceService.Application.Common.Models;
using ResourceService.Domain.Resource;

namespace ResourceService.Application.Resource.Commands;

public record CreateResourceCommand(string Name, ResourceType Type, string? Description, int? QuantityAvailable, ResourceStatus Status) : IRequest<ResourceDto>;

public class CreateResourceCommandValidator : AbstractValidator<CreateResourceCommand>
{
    public CreateResourceCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Invalid resource type.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid resource status.");

        RuleFor(x => x.QuantityAvailable)
            .GreaterThanOrEqualTo(0)
            .When(x => x.QuantityAvailable.HasValue)
            .WithMessage("Quantity must be a non-negative number.");
    }
}

public class CreateResourceCommandHandler(IResourceRepository resourceRepository, IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<CreateResourceCommand, ResourceDto>
{
    public async Task<ResourceDto> Handle(CreateResourceCommand request, CancellationToken cancellationToken)
    {
        int? quantityAvailable = null;
        if (request.Type is ResourceType.Consumable)
        {
            if (request.QuantityAvailable is null)
            {
                throw new BadRequestException("QuantityAvailable is required if it's a Consumable.");
            }
           var (resources,count) = await resourceRepository.GetAllWithFilters(null,request.Name,null, null, new PageQueryRequest(), cancellationToken);
           if (count != 0)
           {
               throw new BadRequestException($"A ressource with this Name already exists. ID {resources[0].Id}, it's better if you add to the quantity");
           }
           quantityAvailable = request.QuantityAvailable;
        }
        
        Domain.Resource.Resource resource = new Domain.Resource.Resource(request.Name, request.Type, request.Description, quantityAvailable, request.Status);
        await resourceRepository.AddAsync(resource, cancellationToken);
        var isSaved = await unitOfWork.SaveChangesAsync(cancellationToken);
        if (isSaved <= 0)
        {
            throw new InternalServerException();
        }
        
        return mapper.Map<ResourceDto>(resource);
    }
}