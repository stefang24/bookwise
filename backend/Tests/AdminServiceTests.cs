using backend.Models;
using backend.Repositories;
using backend.Services;
using Moq;
using Xunit;

namespace backend.Tests.Services
{
    public class AdminServiceTests
    {
        private readonly Mock<IAdminRepository> _adminRepositoryMock;
        private readonly AdminService _service;

        public AdminServiceTests()
        {
            _adminRepositoryMock = new Mock<IAdminRepository>();
            _service = new AdminService(_adminRepositoryMock.Object);
        }

        [Fact]
        public async Task GetUsersAsync_ReturnsMappedUsers()
        {
            var users = new List<User>
            {
                new()
                {
                    Id = 1,
                    Email = "user@test.com",
                    Username = "user1",
                    FirstName = "John",
                    LastName = "Doe",
                    Role = UserRole.User,
                    IsActive = true
                },
                new()
                {
                    Id = 2,
                    Email = "provider@test.com",
                    Username = "provider1",
                    CompanyName = "Provider Co",
                    Role = UserRole.Provider,
                    IsActive = true,
                    City = "Belgrade"
                }
            };

            _adminRepositoryMock.Setup(r => r.GetAllUsersAsync()).ReturnsAsync(users);

            var result = await _service.GetUsersAsync();

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(2, result.Data.Count);
            Assert.Equal("John Doe", result.Data[0].Name);
            Assert.Equal("Provider Co", result.Data[1].Name);
        }

        [Fact]
        public async Task SetUserActivationAsync_WhenAdminTargetsSelf_ReturnsFail()
        {
            var result = await _service.SetUserActivationAsync(adminUserId: 1, targetUserId: 1, isActive: false);

            Assert.False(result.Success);
            Assert.Contains("cannot change your own", result.Message ?? string.Empty);
        }

        [Fact]
        public async Task SetUserActivationAsync_WhenTargetIsAdmin_ReturnsFail()
        {
            _adminRepositoryMock
                .Setup(r => r.GetUserByIdAsync(2))
                .ReturnsAsync(new User { Id = 2, Role = UserRole.Admin, IsActive = true });

            var result = await _service.SetUserActivationAsync(adminUserId: 1, targetUserId: 2, isActive: false);

            Assert.False(result.Success);
            Assert.Contains("Admin account cannot be changed", result.Message ?? string.Empty);
        }

        [Fact]
        public async Task SetUserActivationAsync_WhenValid_UpdatesUserAndSaves()
        {
            var user = new User { Id = 2, Role = UserRole.User, IsActive = true };
            _adminRepositoryMock.Setup(r => r.GetUserByIdAsync(2)).ReturnsAsync(user);

            var result = await _service.SetUserActivationAsync(adminUserId: 1, targetUserId: 2, isActive: false);

            Assert.True(result.Success);
            Assert.True(result.Data);
            Assert.False(user.IsActive);
            _adminRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAppointmentsAsync_ReturnsMappedAppointments()
        {
            var appointments = new List<Appointment>
            {
                new()
                {
                    Id = 11,
                    ProviderService = new ProviderService
                    {
                        Name = "Haircut",
                        Category = "Beauty",
                        PriceEur = 30,
                        ProviderId = 7,
                        Provider = new User { FirstName = "Ana", LastName = "Provider", CompanyName = "Ana Studio" }
                    },
                    ClientId = 9,
                    Client = new User { FirstName = "Petar", LastName = "Client" },
                    StartUtc = DateTime.UtcNow,
                    EndUtc = DateTime.UtcNow.AddHours(1),
                    Status = AppointmentStatus.Scheduled
                }
            };

            _adminRepositoryMock.Setup(r => r.GetAllAppointmentsAsync()).ReturnsAsync(appointments);

            var result = await _service.GetAppointmentsAsync();

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            var item = Assert.Single(result.Data);
            Assert.Equal("Haircut", item.ServiceName);
            Assert.Equal("Ana Studio", item.ProviderName);
            Assert.Equal("Petar Client", item.ClientName);
            Assert.Equal("Scheduled", item.Status);
        }

        [Fact]
        public async Task GetStatsAsync_CalculatesStats()
        {
            var users = new List<User>
            {
                new() { Role = UserRole.User },
                new() { Role = UserRole.User },
                new() { Role = UserRole.Provider }
            };

            var appointments = new List<Appointment>
            {
                new()
                {
                    Status = AppointmentStatus.Scheduled,
                    StartUtc = new DateTime(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc),
                    ProviderService = new ProviderService { Category = "Beauty" }
                },
                new()
                {
                    Status = AppointmentStatus.Completed,
                    StartUtc = new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc),
                    ProviderService = new ProviderService { Category = "Beauty" }
                },
                new()
                {
                    Status = AppointmentStatus.Cancelled,
                    StartUtc = new DateTime(2026, 1, 2, 9, 0, 0, DateTimeKind.Utc),
                    ProviderService = new ProviderService { Category = "Fitness" }
                }
            };

            _adminRepositoryMock.Setup(r => r.GetAllUsersAsync()).ReturnsAsync(users);
            _adminRepositoryMock
                .Setup(r => r.GetAppointmentsInRangeAsync(It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .ReturnsAsync(appointments);

            var result = await _service.GetStatsAsync(null, null);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(2, result.Data.TotalUsers);
            Assert.Equal(1, result.Data.TotalProviders);
            Assert.Equal(3, result.Data.TotalAppointments);
            Assert.Equal(1, result.Data.ScheduledAppointments);
            Assert.Equal(1, result.Data.CompletedAppointments);
            Assert.Equal(1, result.Data.CancelledAppointments);
            Assert.Equal(2, result.Data.BookingsByDate.Count);
            Assert.Equal("Beauty", result.Data.BookingsByCategory[0].Label);
            Assert.Equal(2, result.Data.BookingsByCategory[0].Value);
        }
    }
}
