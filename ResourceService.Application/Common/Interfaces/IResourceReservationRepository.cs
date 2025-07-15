
using ResourceService.Application.Common.Models;

namespace ResourceService.Application.Common.Interfaces;

public interface IResourceReservationRepository : IBaseRepository<Domain.ResourceReservation.ResourceReservation,long>
{
    Task<bool> IsResourceAvailableAsync(long resourceId, DateTime startTime, DateTime endTime,
        CancellationToken cancellationToken = default);
    Task<(List<Domain.ResourceReservation.ResourceReservation> Reservations, int TotalCount)> GetAllWithFiltersAsync(
        List<long>? ids,
        long? resourceId,
        Guid? reservedBy,
        Guid? taskItemId,
        DateTime? startTime,
        DateTime? endTime,
        PageQueryRequest pageQuery,
        CancellationToken cancellationToken = default);
}