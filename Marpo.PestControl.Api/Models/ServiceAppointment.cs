namespace Marpo.PestControl.Api.Models;

public sealed class ServiceAppointment
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string PropertyAddress { get; set; } = string.Empty;
    public string PestType { get; set; } = string.Empty;
    public DateOnly ScheduledDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public int TechnicianId { get; set; }
    public Technician? Technician { get; set; }
}
