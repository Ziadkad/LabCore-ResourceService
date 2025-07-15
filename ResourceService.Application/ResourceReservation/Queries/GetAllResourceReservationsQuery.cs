using AutoMapper;
using MediatR;
using ResourceService.Application.Common.Interfaces;
using ResourceService.Application.Common.Models;
using ResourceService.Application.ResourceReservation.Models;

namespace ResourceService.Application.ResourceReservation.Queries;

public record GetAllResourceReservationsQuery(List<long>? Ids, long? ResourceId, Guid? ReservedBy, Guid? TaskItemId, DateTime? StartTime,  DateTime? EndTime, PageQueryRequest pageQueryRequest):IRequest<PageQueryResult<ResourceReservationWithResourceDto>>;

public class GetAllResourceReservationsQueryHandler(IResourceReservationRepository resourceReservationRepository, IMapper mapper) : IRequestHandler<GetAllResourceReservationsQuery, PageQueryResult<ResourceReservationWithResourceDto>>
{

    public async Task<PageQueryResult<ResourceReservationWithResourceDto>> Handle(GetAllResourceReservationsQuery request, CancellationToken cancellationToken)
    {
        var (reservations, totalCount) = await resourceReservationRepository.GetAllWithFiltersAsync(
            request.Ids,
            request.ResourceId,
            request.ReservedBy,
            request.TaskItemId,
            request.StartTime,
            request.EndTime,
            request.pageQueryRequest,
            cancellationToken);

        List<ResourceReservationWithResourceDto> reservationsDto = mapper.Map<List<ResourceReservationWithResourceDto>>(reservations);
        return new PageQueryResult<ResourceReservationWithResourceDto>(reservationsDto, totalCount);
    }
}
