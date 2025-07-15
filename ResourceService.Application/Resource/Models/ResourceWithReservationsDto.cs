using ResourceService.Application.ResourceReservation.Models;
using ResourceService.Domain.Resource;

namespace ResourceService.Application.Resource.Models;

public class ResourceWithReservationsDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public ResourceType Type { get; set; }
    public string? Description { get; set; }
    public int? QuantityAvailable { get; set; }
    public ResourceStatus Status { get; set; }
    public  List<ResourceReservationDto> ResourceReservations {get; set;}
}