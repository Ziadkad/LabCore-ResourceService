using AutoMapper;
using ResourceService.Application.ResourceReservation.Models;

namespace ResourceService.Application.ResourceReservation;

public class ResourceReservationMapper : Profile
{
    public ResourceReservationMapper()
    {
        CreateMap<Domain.ResourceReservation.ResourceReservation, ResourceReservationDto>();
        CreateMap<Domain.ResourceReservation.ResourceReservation, ResourceReservationWithResourceDto>();

    }
}