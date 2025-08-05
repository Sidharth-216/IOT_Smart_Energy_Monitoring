using Microsoft.EntityFrameworkCore;
using SmartEnergy.Web.Models;

namespace SmartEnergy.Web.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Device> Devices { get; set; }
        public DbSet<EnergyReading> EnergyReadings { get; set; }
        public DbSet<Alert> Alerts { get; set; }
        public DbSet<Suggestion> Suggestions { get; set; }
        public DbSet<HistoricalStat> HistoricalStats { get; set; }
    }
}
