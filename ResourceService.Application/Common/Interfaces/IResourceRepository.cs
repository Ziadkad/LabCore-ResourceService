
using ResourceService.Application.Common.Models;
using ResourceService.Domain.Resource;

namespace ResourceService.Application.Common.Interfaces;

public interface IResourceRepository : IBaseRepository<Domain.Resource.Resource, long>
{
    Task<(List<Domain.Resource.Resource> Resources, int TotalCount)> GetAllWithFilters(
        List<long>? ids,
        string? keyword,
        ResourceType? type,
        ResourceStatus? status,
        PageQueryRequest pageQuery,
        CancellationToken cancellationToken = default);

    Task<Domain.Resource.Resource?> GetWithReservation(long? id, CancellationToken cancellationToken = default);
    
}