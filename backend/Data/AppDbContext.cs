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
        public DbSet<Chat> Chats { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<AppNotification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();
                entity.HasIndex(u => u.Username).IsUnique();
                entity.Property(u => u.IsActive).HasDefaultValue(true);
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

                modelBuilder.Entity<Chat>(entity =>
                {
                    entity.HasOne(x => x.User1)
                        .WithMany(x => x.ChatsInitiated)
                        .HasForeignKey(x => x.User1Id)
                        .OnDelete(DeleteBehavior.Restrict);

                    entity.HasOne(x => x.User2)
                        .WithMany(x => x.ChatsReceived)
                        .HasForeignKey(x => x.User2Id)
                        .OnDelete(DeleteBehavior.Restrict);

                    entity.HasIndex(x => new { x.User1Id, x.User2Id }).IsUnique();
                });

                modelBuilder.Entity<ChatMessage>(entity =>
                {
                    entity.Property(x => x.Content).HasMaxLength(1500);

                    entity.HasOne(x => x.Chat)
                        .WithMany(x => x.Messages)
                        .HasForeignKey(x => x.ChatId)
                        .OnDelete(DeleteBehavior.Cascade);

                    entity.HasOne(x => x.Sender)
                        .WithMany(x => x.SentMessages)
                        .HasForeignKey(x => x.SenderId)
                        .OnDelete(DeleteBehavior.Restrict);

                    entity.HasIndex(x => new { x.ChatId, x.SentAtUtc });
                });

                modelBuilder.Entity<AppNotification>(entity =>
                {
                    entity.Property(x => x.Type).HasMaxLength(50);
                    entity.Property(x => x.Title).HasMaxLength(120);
                    entity.Property(x => x.Message).HasMaxLength(1000);

                    entity.HasOne(x => x.User)
                        .WithMany(x => x.Notifications)
                        .HasForeignKey(x => x.UserId)
                        .OnDelete(DeleteBehavior.Cascade);

                    entity.HasOne(x => x.RelatedUser)
                        .WithMany()
                        .HasForeignKey(x => x.RelatedUserId)
                        .OnDelete(DeleteBehavior.SetNull);

                    entity.HasOne(x => x.ChatMessage)
                        .WithMany()
                        .HasForeignKey(x => x.ChatMessageId)
                        .OnDelete(DeleteBehavior.SetNull);

                    entity.HasIndex(x => new { x.UserId, x.IsRead, x.CreatedAtUtc });
                });
        }
    }
}

