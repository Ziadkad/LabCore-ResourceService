
using ResourceService.Domain.Resource;

public class ResourceDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public ResourceType Type { get; set; }
    public string? Description { get; set; }
    public int? QuantityAvailable { get; set; }
    public ResourceStatus Status { get; set; }

}