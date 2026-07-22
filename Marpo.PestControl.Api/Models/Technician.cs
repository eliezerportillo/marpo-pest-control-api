namespace Marpo.PestControl.Api.Models;

public sealed class Technician
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
}
