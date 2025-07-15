using AutoMapper;
using MediatR;
using ResourceService.Application.Common.Interfaces;
using ResourceService.Application.Common.Models;
using ResourceService.Domain.Resource;

namespace ResourceService.Application.Resource.Queries;

public record GetAllResourcesQuery(List<long>? Ids, string? Keyword, ResourceType? Type, ResourceStatus? Status, PageQueryRequest PageQuery):IRequest<PageQueryResult<ResourceDto>>;

public class GetAllResourcesQueryHandler(
    IResourceRepository resourceRepository,
    IMapper mapper
) : IRequestHandler<GetAllResourcesQuery, PageQueryResult<ResourceDto>>
{
    public async Task<PageQueryResult<ResourceDto>> Handle(GetAllResourcesQuery request, CancellationToken cancellationToken)
    {
        var (resources, totalCount) = await resourceRepository.GetAllWithFilters(
            request.Ids,
            request.Keyword,
            request.Type,
            request.Status,
            request.PageQuery,
            cancellationToken
        );

        var dtos = mapper.Map<List<ResourceDto>>(resources);
        return new PageQueryResult<ResourceDto>(dtos, totalCount);
    }
}