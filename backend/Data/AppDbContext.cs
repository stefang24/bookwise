using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<ProviderService> ProviderServices { get; set; }
        public DbSet<ProviderWorkingHour> ProviderWorkingHours { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();
                entity.HasIndex(u => u.Username).IsUnique();
            });

            modelBuilder.Entity<ProviderService>(entity =>
            {
                entity.Property(x => x.PriceEur).HasPrecision(10, 2);
                entity.HasOne(x => x.Provider)
                      .WithMany(x => x.ProviderServices)
                      .HasForeignKey(x => x.ProviderId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ProviderWorkingHour>(entity =>
            {
                entity.HasIndex(x => new { x.ProviderId, x.DayOfWeek }).IsUnique();
                entity.HasOne(x => x.Provider)
                      .WithMany(x => x.WorkingHours)
                      .HasForeignKey(x => x.ProviderId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.HasOne(x => x.ProviderService)
                      .WithMany(x => x.Appointments)
                      .HasForeignKey(x => x.ProviderServiceId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.Client)
                      .WithMany(x => x.ClientAppointments)
                      .HasForeignKey(x => x.ClientId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
