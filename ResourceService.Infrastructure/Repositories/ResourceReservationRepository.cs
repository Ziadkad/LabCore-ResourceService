using Microsoft.EntityFrameworkCore;
using ResourceService.Application.Common.Interfaces;
using ResourceService.Application.Common.Models;
using ResourceService.Domain.ResourceReservation;
using ResourceService.Infrastructure.Data;

namespace ResourceService.Infrastructure.Repositories;

public class ResourceReservationRepository(AppDbContext dbContext) : BaseRepository<ResourceReservation,long>(dbContext), IResourceReservationRepository
{
    public async Task<bool> IsResourceAvailableAsync(long resourceId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken = default)
    {
        // Return true if NO existing reservations overlap with the given interval
        return !await DbSet.AnyAsync(r =>
                r.ResourceId == resourceId &&
                r.StartTime < endTime &&  // Existing reservation starts before the new end
                r.EndTime > startTime,    // Existing reservation ends after the new start
            cancellationToken);
    }

    public async Task<(List<ResourceReservation> Reservations, int TotalCount)> GetAllWithFiltersAsync(
        List<long>? ids,
        long? resourceId,
        Guid? reservedBy,
        Guid? taskItemId,
        DateTime? startTime,
        DateTime? endTime,
        PageQueryRequest pageQuery,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .Include(r => r.Resource)
            .AsQueryable();

        query = query.Where(r => r.Resource != null && !r.Resource.IsArchived);
        if (ids?.Any() == true)
            query = query.Where(r => ids.Contains(r.Id));

        if (resourceId != 0)
            query = query.Where(r => r.ResourceId == resourceId);

        if (reservedBy != Guid.Empty)
            query = query.Where(r => r.ReservedBy == reservedBy);

        if (taskItemId != Guid.Empty)
            query = query.Where(r => r.TaskItemId == taskItemId);

        if (startTime.HasValue)
            query = query.Where(r => r.StartTime >= startTime.Value);

        if (endTime.HasValue)
            query = query.Where(r => r.EndTime <= endTime.Value);

        var total = await query.CountAsync(cancellationToken);

        var data = await query
            .OrderByDescending(r => r.StartTime)
            .Skip((pageQuery.Page - 1) * pageQuery.PageSize)
            .Take(pageQuery.PageSize)
            .ToListAsync(cancellationToken);

        return (data, total);
    }
}