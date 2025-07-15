using AutoMapper;
using MediatR;
using ResourceService.Application.Common.Exceptions;
using ResourceService.Application.Common.Interfaces;
using ResourceService.Application.Resource.Models;

namespace ResourceService.Application.Resource.Queries;

public record GetResourceQuery(long Id,List<Guid>? ReservedByList, Guid? TaskItemId, DateTime? StartTime, DateTime? EndTime) : IRequest<ResourceWithReservationsDto>;

public class GetResourceQueryHandler(
    IResourceRepository resourceRepository,
    IMapper mapper
)  : IRequestHandler<GetResourceQuery, ResourceWithReservationsDto>
{
    public async Task<ResourceWithReservationsDto> Handle(GetResourceQuery request, CancellationToken cancellationToken)
    {
        var resource = await resourceRepository.GetWithReservation(request.Id, cancellationToken);
        if (resource is null)
            throw new NotFoundException(nameof(Resource), request.Id);
        
        if (resource.ResourceReservations.Count > 0)
        {
            var filteredReservations = resource.ResourceReservations.AsQueryable();

            // Filter by ReservedByList
            if (request.ReservedByList is not null && request.ReservedByList.Any())
            {
                filteredReservations = filteredReservations
                    .Where(r => request.ReservedByList.Contains(r.ReservedBy));
            }

            // Filter by TaskItemId
            if (request.TaskItemId is not null)
            {
                filteredReservations = filteredReservations
                    .Where(r => r.TaskItemId == request.TaskItemId);
            }

            // Filter by StartTime (overlap logic)
            if (request.StartTime is not null)
            {
                filteredReservations = filteredReservations
                    .Where(r => r.EndTime > request.StartTime);
            }

            // Filter by EndTime (overlap logic)
            if (request.EndTime is not null)
            {
                filteredReservations = filteredReservations
                    .Where(r => r.StartTime < request.EndTime);
            }

            resource.setResourceReservations(filteredReservations.ToList());
        }
        
        return mapper.Map<ResourceWithReservationsDto>(resource);
    }
}