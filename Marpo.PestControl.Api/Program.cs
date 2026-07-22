using Marpo.PestControl.Api.Data;
using Marpo.PestControl.Api.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PestControlContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("PestControlDatabase")));

var app = builder.Build();

await app.Services.InitializeDatabaseAsync();

app.UseHttpsRedirection();

app.MapGet("/", () => Results.Ok(new
{
    Company = "Marpo Pest Control",
    Message = "Pest control service API is running."
}));

app.MapGet("/api/technicians", async (PestControlContext dbContext) =>
    await dbContext.Technicians
        .AsNoTracking()
        .OrderBy(technician => technician.Name)
        .ToListAsync());

app.MapGet("/api/appointments", async (PestControlContext dbContext) =>
    await dbContext.ServiceAppointments
        .AsNoTracking()
        .Include(appointment => appointment.Technician)
        .OrderBy(appointment => appointment.ScheduledDate)
        .ThenBy(appointment => appointment.CustomerName)
        .Select(appointment => appointment.ToResponse())
        .ToListAsync());

app.MapGet("/api/appointments/{id:int}", async (int id, PestControlContext dbContext) =>
{
    var appointment = await dbContext.ServiceAppointments
        .AsNoTracking()
        .Include(serviceAppointment => serviceAppointment.Technician)
        .Where(serviceAppointment => serviceAppointment.Id == id)
        .Select(serviceAppointment => serviceAppointment.ToResponse())
        .SingleOrDefaultAsync();

    return appointment is null ? Results.NotFound() : Results.Ok(appointment);
});

app.MapPost("/api/appointments", async (CreateServiceAppointmentRequest request, PestControlContext dbContext) =>
{
    if (string.IsNullOrWhiteSpace(request.CustomerName) ||
        string.IsNullOrWhiteSpace(request.PropertyAddress) ||
        string.IsNullOrWhiteSpace(request.PestType))
    {
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            ["appointment"] = ["CustomerName, PropertyAddress, and PestType are required."]
        });
    }

    var technician = await dbContext.Technicians.FindAsync(request.TechnicianId);
    if (technician is null)
    {
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            ["technicianId"] = ["Assigned technician was not found."]
        });
    }

    var appointment = new ServiceAppointment
    {
        CustomerName = request.CustomerName.Trim(),
        PropertyAddress = request.PropertyAddress.Trim(),
        PestType = request.PestType.Trim(),
        ScheduledDate = request.ScheduledDate,
        Status = string.IsNullOrWhiteSpace(request.Status) ? "Scheduled" : request.Status.Trim(),
        Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim(),
        TechnicianId = request.TechnicianId
    };

    dbContext.ServiceAppointments.Add(appointment);
    await dbContext.SaveChangesAsync();

    await dbContext.Entry(appointment)
        .Reference(serviceAppointment => serviceAppointment.Technician)
        .LoadAsync();

    return Results.Created($"/api/appointments/{appointment.Id}", appointment.ToResponse());
});

app.Run();

static class MappingExtensions
{
    public static object ToResponse(this ServiceAppointment appointment) => new
    {
        appointment.Id,
        appointment.CustomerName,
        appointment.PropertyAddress,
        appointment.PestType,
        appointment.ScheduledDate,
        appointment.Status,
        appointment.Notes,
        Technician = appointment.Technician is null
            ? null
            : new
            {
                appointment.Technician.Id,
                appointment.Technician.Name,
                appointment.Technician.Region,
                appointment.Technician.LicenseNumber
            }
    };
}

internal sealed record CreateServiceAppointmentRequest(
    string CustomerName,
    string PropertyAddress,
    string PestType,
    DateOnly ScheduledDate,
    int TechnicianId,
    string? Status,
    string? Notes);
