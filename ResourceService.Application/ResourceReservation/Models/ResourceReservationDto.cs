namespace ResourceService.Application.ResourceReservation.Models;

public class ResourceReservationDto
{
    public long Id { get; set; }
    public long ResourceId { get; set; }
    public Guid ReservedBy { get; set; }
    public Guid? TaskItemId { get; set; } 
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? Notes { get; set; }
}