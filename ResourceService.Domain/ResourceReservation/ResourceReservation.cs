using ResourceService.Domain.Common;

namespace ResourceService.Domain.ResourceReservation;

public class ResourceReservation : BaseModel
{
    public long Id { get; private set; }
    public long ResourceId { get; private set; }
    public Resource.Resource? Resource { get; private set; }
    
    public Guid ReservedBy { get; private set; }
    public Guid? TaskItemId { get; private set; } 

    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public int? Quantity { get; private set; }
    public string? Notes { get; private set; }

    public ResourceReservation(long resourceId, Guid reservedBy, Guid? taskItemId, DateTime startTime, DateTime endTime, string? notes, int? quantity)
    {
        ResourceId = resourceId;
        ReservedBy = reservedBy;
        TaskItemId = taskItemId;
        StartTime = startTime;
        EndTime = endTime;
        Quantity = quantity;
        Notes = notes;
    }
    
    
}