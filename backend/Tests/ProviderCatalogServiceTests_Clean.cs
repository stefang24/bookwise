using Xunit;
using Moq;
using backend.Services;
using backend.Helpers;
using backend.DTOs;
using backend.Models;
using backend.Repositories;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace backend.Tests.Services.Obsolete
{
    public class ProviderCatalogServiceTestsOld
    {
        private readonly Mock<IProviderServiceRepository> _mockProviderServiceRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IWebHostEnvironment> _mockWebHostEnvironment;
        private readonly ProviderCatalogService _service;

    public ProviderCatalogServiceTestsOld()
        {
            ConfigProvider.Initialize(new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["JwtSettings:Secret"] = "super-secret-key-for-tests-123456789",
                    ["AppConfig:Defaults:ProfileImagePath"] = "/images/profiles/default.png",
                    ["AppConfig:Defaults:ServiceImagePath"] = "/images/services/default-service.svg",
                    ["AppConfig:Uploads:MaxImageFileSizeBytes"] = "5242880",
                    ["AppConfig:Uploads:AllowedImageExtensions:0"] = ".jpg",
                    ["AppConfig:Uploads:AllowedImageExtensions:1"] = ".jpeg",
                    ["AppConfig:Uploads:AllowedImageExtensions:2"] = ".png",
                    ["AppConfig:Uploads:AllowedImageExtensions:3"] = ".gif",
                    ["AppConfig:Uploads:AllowedImageExtensions:4"] = ".webp",
                    ["AppConfig:Uploads:ProfileImagesFolder"] = "images/profiles",
                    ["AppConfig:Uploads:ServiceImagesFolder"] = "images/services",
                    ["AppConfig:Uploads:ProfileImageFilePrefix"] = "profile",
                    ["AppConfig:Uploads:ServiceImageFilePrefix"] = "svc",
                    ["AppConfig:ProviderCatalog:PredefinedCategories:0"] = "Haircut",
                    ["AppConfig:ProviderCatalog:PredefinedCategories:1"] = "Barber"
                })
                .Build());

            _mockProviderServiceRepository = new Mock<IProviderServiceRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockWebHostEnvironment = new Mock<IWebHostEnvironment>();
            _mockWebHostEnvironment.SetupGet(e => e.WebRootPath).Returns(Path.GetTempPath());
            _mockProviderServiceRepository.Setup(r => r.GetActiveCategoriesAsync()).ReturnsAsync(new List<string>());

            _service = new ProviderCatalogService(
                _mockProviderServiceRepository.Object,
                _mockUserRepository.Object,
                _mockWebHostEnvironment.Object
            );
        }

        [Fact]
        public async Task CreateAsync_WithValidData_ReturnsSuccess()
        {
            int providerId = 1;
            var provider = new User { Id = providerId, Role = UserRole.Provider, FirstName = "Jane", LastName = "Doe" };
            var request = new ProviderServiceRequest
            {
                Name = "Haircut",
                Category = "Beauty",
                Description = "Professional haircut",
                PriceEur = 30,
                DurationMinutes = 45,
                ImageUrl = null
            };

            _mockUserRepository.Setup(r => r.GetByIdAsync(providerId))
                .ReturnsAsync(provider);
            var result = await _service.CreateAsync(providerId, request);
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("Haircut", result.Data.Name);
            Assert.Equal(30, result.Data.PriceEur);
        }

        [Fact]
        public async Task CreateAsync_WithTooShortDuration_ReturnsFail()
        {
            int providerId = 1;
            var provider = new User { Id = providerId, Role = UserRole.Provider };
            var request = new ProviderServiceRequest
            {
                Name = "Service",
                Category = "Beauty",
                PriceEur = 50,
                DurationMinutes = 3
            };

            _mockUserRepository.Setup(r => r.GetByIdAsync(providerId))
                .ReturnsAsync(provider);
            var result = await _service.CreateAsync(providerId, request);
            Assert.False(result.Success);
            Assert.Contains("Duration", result.Message);
        }

        [Fact]
        public async Task DeleteAsync_WithValidService_ReturnsSuccess()
        {
            int providerId = 1;
            int serviceId = 100;
            var service = new ProviderService
            {
                Id = serviceId,
                ProviderId = providerId,
                Name = "Service",
                IsActive = true,
                ImageUrl = "/default.jpg"
            };

            _mockProviderServiceRepository.Setup(r => r.GetByIdAndProviderAsync(serviceId, providerId))
                .ReturnsAsync(service);
            var result = await _service.DeleteAsync(providerId, serviceId);
            Assert.True(result.Success);
            _mockProviderServiceRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task GetMyServicesAsync_ReturnsProviderServices()
        {
            int providerId = 1;
            var provider = new User { Id = providerId, FirstName = "John", LastName = "Doe" };
            var services = new List<ProviderService>
            {
                new ProviderService { Id = 1, Name = "Service1", Provider = provider, IsActive = true },
                new ProviderService { Id = 2, Name = "Service2", Provider = provider, IsActive = true }
            };

            _mockProviderServiceRepository.Setup(r => r.GetByProviderActiveAsync(providerId))
                .ReturnsAsync(services);
            var result = await _service.GetMyServicesAsync(providerId);
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(2, result.Data.Count);
        }

        [Fact]
        public async Task SearchAsync_WithCategoryFilter_ReturnsFiltered()
        {
            var provider = new User { Id = 1, FirstName = "Jane", LastName = "Smith" };
            var services = new List<ProviderService>
            {
                new ProviderService { Id = 1, Name = "Haircut", Category = "Beauty", Provider =provider, IsActive = true },
                new ProviderService { Id = 2, Name = "Styling", Category = "Beauty", Provider = provider, IsActive = true }
            };

            _mockProviderServiceRepository.Setup(r => r.SearchActiveAsync("Beauty", null))
                .ReturnsAsync(services);
            var result = await _service.SearchAsync("Beauty", null);
            Assert.True(result.Success);
            Assert.Equal(2, result.Data.Count);
        }

        [Fact]
        public async Task GetCategoriesAsync_ReturnsPredefinedCategories()
        {
            var result = await _service.GetCategoriesAsync();
            Assert.True(result.Success);
            Assert.NotEmpty(result.Data);
            Assert.Contains("Haircut", result.Data);
        }

        [Fact]
        public async Task GetProvidersAsync_WithFilters_ReturnsFiltered()
        {
            var providers = new List<User>
            {
                new User
                {
                    Id = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    CompanyName = "John's Salon",
                    PrimaryCategory = "Beauty",
                    City = "Belgrade",
                    Address = "Main St 1",
                    ProfileImagePath = "/images/provider.jpg",
                    ProviderServices = new List<ProviderService> { new ProviderService { IsActive = true } }
                }
            };

            _mockUserRepository.Setup(r => r.GetProvidersFilteredAsync("Beauty", "Belgrade", null))
                .ReturnsAsync(providers);
            var result = await _service.GetProvidersAsync("Beauty", "Belgrade", null, "name");
            Assert.True(result.Success);
            Assert.Single(result.Data);
            Assert.Equal("John's Salon", result.Data[0].Name);
        }
    }
}
