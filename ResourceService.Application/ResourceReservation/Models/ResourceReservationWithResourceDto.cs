namespace ResourceService.Application.ResourceReservation.Models;

public class ResourceReservationWithResourceDto
{
    public long Id { get; set; }
    public long ResourceId { get; set; }
    public Guid ReservedBy { get; set; }
    public Guid? TaskItemId { get; set; } 
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int Quantity { get; set; }
    public string? Notes { get; set; }
    public ResourceDto Resource { get; set; }
}