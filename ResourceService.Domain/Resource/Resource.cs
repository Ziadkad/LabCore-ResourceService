using ResourceService.Domain.Common;

namespace ResourceService.Domain.Resource;

public class Resource : BaseModel
{
    public long Id { get; private set; }
    public string Name { get; private set; }
    public ResourceType Type { get; private set; }
    public string? Description { get; private set; }
    public int? QuantityAvailable { get; private set; }
    public ResourceStatus Status { get; private set; }
    
    public string? ImageUrl { get; private set; }
    
    public List<ResourceReservation.ResourceReservation> ResourceReservations { get; private set; } = new List<ResourceReservation.ResourceReservation>(); 

    public Resource(string name, ResourceType type, string? description, int? quantityAvailable, ResourceStatus status)
    {
        Name = name;
        Type = type;
        Description = description;
        QuantityAvailable = quantityAvailable;
        Status = status;
    }

    public void Update(string name, ResourceType type, string? description, int? quantityAvailable, ResourceStatus status)
    {
        Name = name;
        Type = type;
        Description = description;
        QuantityAvailable = quantityAvailable;
        Status = status;
    }

    public void setImageUrl(string imageUrl)
    {
        ImageUrl = imageUrl;
    }

    public void setResourceReservations(List<ResourceReservation.ResourceReservation> resourceReservations)
    {
        ResourceReservations = resourceReservations;
    }
}