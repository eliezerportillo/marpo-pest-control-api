using Marpo.PestControl.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Marpo.PestControl.Api.Data;

public sealed class PestControlContext(DbContextOptions<PestControlContext> options) : DbContext(options)
{
    public DbSet<ServiceAppointment> ServiceAppointments => Set<ServiceAppointment>();
    public DbSet<Technician> Technicians => Set<Technician>();
}
