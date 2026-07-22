using Marpo.PestControl.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Marpo.PestControl.Api.Data;

public static class SeedData
{
    public static async Task InitializeDatabaseAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<PestControlContext>();

        await dbContext.Database.EnsureCreatedAsync();

        if (await dbContext.Technicians.AnyAsync() || await dbContext.ServiceAppointments.AnyAsync())
        {
            return;
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var technicians = new[]
        {
            new Technician
            {
                Name = "Ana Gomez",
                Region = "North Service Area",
                LicenseNumber = "MPC-1001"
            },
            new Technician
            {
                Name = "Marcus Reed",
                Region = "Central Service Area",
                LicenseNumber = "MPC-1002"
            },
            new Technician
            {
                Name = "Sofia Patel",
                Region = "South Service Area",
                LicenseNumber = "MPC-1003"
            }
        };

        dbContext.Technicians.AddRange(technicians);
        await dbContext.SaveChangesAsync();

        dbContext.ServiceAppointments.AddRange(
            new ServiceAppointment
            {
                CustomerName = "Green Valley Apartments",
                PropertyAddress = "125 Orchard Lane",
                PestType = "Termites",
                ScheduledDate = today.AddDays(1),
                Status = "Scheduled",
                Notes = "Inspect all first-floor units near the courtyard.",
                TechnicianId = technicians[0].Id
            },
            new ServiceAppointment
            {
                CustomerName = "Harper Family Home",
                PropertyAddress = "48 Cedar Street",
                PestType = "Ants",
                ScheduledDate = today.AddDays(2),
                Status = "Scheduled",
                Notes = "Focus on the kitchen and patio entry points.",
                TechnicianId = technicians[1].Id
            },
            new ServiceAppointment
            {
                CustomerName = "Riverside Bistro",
                PropertyAddress = "900 River Road",
                PestType = "Rodents",
                ScheduledDate = today.AddDays(3),
                Status = "Inspection Complete",
                Notes = "Follow-up visit requested after sanitation update.",
                TechnicianId = technicians[2].Id
            });

        await dbContext.SaveChangesAsync();
    }
}
