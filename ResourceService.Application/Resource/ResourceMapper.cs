using AutoMapper;
using ResourceService.Application.Resource.Models;

namespace ResourceService.Application.Resource;

public class ResourceMapper : Profile
{
    public ResourceMapper()
    {
        CreateMap<Domain.Resource.Resource, ResourceDto>();
        CreateMap<Domain.Resource.Resource, ResourceWithReservationsDto>();

    }
}