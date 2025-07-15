using Microsoft.EntityFrameworkCore;
using ResourceService.Application.Common.Interfaces;
using ResourceService.Application.Common.Models;
using ResourceService.Domain.Resource;
using ResourceService.Infrastructure.Data;

namespace ResourceService.Infrastructure.Repositories;

public class ResourceRepository(AppDbContext dbContext) 
    : BaseRepository<Resource, long>(dbContext), IResourceRepository
{
    public async Task<(List<Resource> Resources, int TotalCount)> GetAllWithFilters(
        List<long>? ids,
        string? keyword,
        ResourceType? type,
        ResourceStatus? status,
        PageQueryRequest pageQuery,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsQueryable();

        if (ids?.Any() == true)
            query = query.Where(r => ids.Contains(r.Id));

        if (!string.IsNullOrWhiteSpace(keyword))
            query = query.Where(r => r.Name.Contains(keyword));

        if (type.HasValue)
            query = query.Where(r => r.Type == type.Value);

        if (status.HasValue)
            query = query.Where(r => r.Status == status.Value);

        query = query.Where(r => !r.IsArchived);

        var totalCount = await query.CountAsync(cancellationToken);

        var resources = await query
            .Skip((pageQuery.Page - 1) * pageQuery.PageSize)
            .Take(pageQuery.PageSize)
            .ToListAsync(cancellationToken);

        return (resources, totalCount);
    }



    public async Task<Resource?> GetWithReservation(long? id, CancellationToken cancellationToken = default)
    {
        return await DbSet.Include(r=>r.ResourceReservations).FirstOrDefaultAsync(r => r.Id == id && !r.IsArchived, cancellationToken);
    }
}