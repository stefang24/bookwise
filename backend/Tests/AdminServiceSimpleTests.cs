using backend.Models;
using backend.Repositories.Interfaces;
using backend.Services;
using Moq;
using Xunit;

namespace backend.Tests.Services
{
    public class AdminServiceSimpleTests
    {
        [Fact]
        public async Task GetUsersAsync_WhenEmpty_ReturnsSuccessWithEmptyList()
        {
            var repo = new Mock<IAdminRepository>();
            repo.Setup(r => r.GetAllUsersAsync()).ReturnsAsync(new List<User>());

            var service = new AdminService(repo.Object);
            var result = await service.GetUsersAsync();

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Empty(result.Data);
        }

        [Fact]
        public async Task GetStatsAsync_WithNoAppointments_ReturnsZeroTotals()
        {
            var repo = new Mock<IAdminRepository>();
            repo.Setup(r => r.GetAllUsersAsync()).ReturnsAsync(new List<User>());
            repo.Setup(r => r.GetAppointmentsInRangeAsync(It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .ReturnsAsync(new List<Appointment>());

            var service = new AdminService(repo.Object);
            var result = await service.GetStatsAsync(null, null);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(0, result.Data.TotalUsers);
            Assert.Equal(0, result.Data.TotalProviders);
            Assert.Equal(0, result.Data.TotalAppointments);
        }
    }
}
