using backend.Controllers;
using backend.DTOs;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;

namespace backend.Tests.Controllers
{
    public class ProviderServicesControllerTests
    {
        private readonly Mock<IProviderCatalogService> _mockCatalogService;
        private readonly ProviderServicesController _controller;

        public ProviderServicesControllerTests()
        {
            _mockCatalogService = new Mock<IProviderCatalogService>();
            _controller = new ProviderServicesController(_mockCatalogService.Object);
        }

        private void SetupUserContext(int userId, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role)
            };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);
            var context = new DefaultHttpContext { User = principal };
            _controller.ControllerContext = new ControllerContext { HttpContext = context };
        }

        [Fact]
        public async Task GetCategories_ReturnsOkWithCategories()
        {
            // Arrange
            var categories = new List<string> { "Beauty", "Fitness", "Photography" };
            _mockCatalogService.Setup(s => s.GetCategoriesAsync())
                .ReturnsAsync(ResultResponse<List<string>>.Ok(categories));

            // Act
            var result = await _controller.Categories();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<ResultResponse<List<string>>>(okResult.Value);
            Assert.True(returnValue.Success);
            Assert.Equal(3, returnValue.Data.Count);
        }

        [Fact]
        public async Task GetProviders_WithFilters_ReturnsOkWithProviders()
        {
            // Arrange
            var providers = new List<ProviderDirectoryItemResponse>
            {
                new ProviderDirectoryItemResponse { Id = 1, Name = "John's Salon", PrimaryCategory = "Beauty", City = "Belgrade" }
            };
            _mockCatalogService.Setup(s => s.GetProvidersAsync("Beauty", "Belgrade", null, "name"))
                .ReturnsAsync(ResultResponse<List<ProviderDirectoryItemResponse>>.Ok(providers));

            // Act
            var result = await _controller.Providers("Beauty", "Belgrade", null, "name");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<ResultResponse<List<ProviderDirectoryItemResponse>>>(okResult.Value);
            Assert.True(returnValue.Success);
            Assert.Single(returnValue.Data);
        }

        [Fact]
        public async Task GetProviders_WithoutFilters_ReturnsOkWithProviders()
        {
            // Arrange
            var providers = new List<ProviderDirectoryItemResponse>
            {
                new ProviderDirectoryItemResponse { Id = 1, Name = "Provider 1" },
                new ProviderDirectoryItemResponse { Id = 2, Name = "Provider 2" }
            };
            _mockCatalogService.Setup(s => s.GetProvidersAsync(null, null, null, null))
                .ReturnsAsync(ResultResponse<List<ProviderDirectoryItemResponse>>.Ok(providers));

            // Act
            var result = await _controller.Providers(null, null, null, null);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<ResultResponse<List<ProviderDirectoryItemResponse>>>(okResult.Value);
            Assert.True(returnValue.Success);
            Assert.Equal(2, returnValue.Data.Count);
        }

        [Fact]
        public async Task Search_WithQuery_ReturnsOkWithServices()
        {
            // Arrange
            var services = new List<ProviderServiceResponse>
            {
                new ProviderServiceResponse { Id = 1, Name = "Haircut", Category = "Beauty", PriceEur = 30 }
            };
            _mockCatalogService.Setup(s => s.SearchAsync("Beauty", "hair"))
                .ReturnsAsync(ResultResponse<List<ProviderServiceResponse>>.Ok(services));

            // Act
            var result = await _controller.Search("Beauty", "hair");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<ResultResponse<List<ProviderServiceResponse>>>(okResult.Value);
            Assert.True(returnValue.Success);
            Assert.Single(returnValue.Data);
        }

        [Fact]
        public async Task GetByProvider_ReturnsOkWithProviderServices()
        {
            // Arrange
            int providerId = 1;
            var services = new List<ProviderServiceResponse>
            {
                new ProviderServiceResponse { Id = 1, Name = "Service 1", ProviderId = providerId }
            };
            _mockCatalogService.Setup(s => s.GetByProviderAsync(providerId))
                .ReturnsAsync(ResultResponse<List<ProviderServiceResponse>>.Ok(services));

            // Act
            var result = await _controller.GetByProvider(providerId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<ResultResponse<List<ProviderServiceResponse>>>(okResult.Value);
            Assert.True(returnValue.Success);
            Assert.Single(returnValue.Data);
        }

        [Fact]
        public async Task GetMy_ReturnsOkWithProviderServices()
        {
            // Arrange
            int providerId = 1;
            SetupUserContext(providerId, "Provider");

            var services = new List<ProviderServiceResponse>
            {
                new ProviderServiceResponse { Id = 1, Name = "Service 1", ProviderId = providerId }
            };
            _mockCatalogService.Setup(s => s.GetMyServicesAsync(providerId))
                .ReturnsAsync(ResultResponse<List<ProviderServiceResponse>>.Ok(services));

            // Act
            var result = await _controller.GetMy();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<ResultResponse<List<ProviderServiceResponse>>>(okResult.Value);
            Assert.True(returnValue.Success);
            Assert.Single(returnValue.Data);
        }

        [Fact]
        public async Task Create_WithValidData_ReturnsOkWithService()
        {
            // Arrange
            int providerId = 1;
            SetupUserContext(providerId, "Provider");

            var request = new ProviderServiceRequest
            {
                Name = "New Service",
                Category = "Beauty",
                PriceEur = 50,
                DurationMinutes = 60
            };

            var createdService = new ProviderServiceResponse
            {
                Id = 1,
                ProviderId = providerId,
                Name = "New Service",
                Category = "Beauty",
                PriceEur = 50,
                DurationMinutes = 60
            };

            _mockCatalogService.Setup(s => s.CreateAsync(providerId, request))
                .ReturnsAsync(ResultResponse<ProviderServiceResponse>.Ok(createdService));

            // Act
            var result = await _controller.Create(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<ResultResponse<ProviderServiceResponse>>(okResult.Value);
            Assert.True(returnValue.Success);
            Assert.Equal("New Service", returnValue.Data.Name);
        }

        [Fact]
        public async Task Update_WithValidData_ReturnsOkWithUpdatedService()
        {
            // Arrange
            int providerId = 1;
            int serviceId = 1;
            SetupUserContext(providerId, "Provider");

            var request = new ProviderServiceRequest
            {
                Name = "Updated Service",
                Category = "Beauty",
                PriceEur = 60,
                DurationMinutes = 75
            };

            var updatedService = new ProviderServiceResponse
            {
                Id = serviceId,
                ProviderId = providerId,
                Name = "Updated Service",
                PriceEur = 60,
                DurationMinutes = 75
            };

            _mockCatalogService.Setup(s => s.UpdateAsync(providerId, serviceId, request))
                .ReturnsAsync(ResultResponse<ProviderServiceResponse>.Ok(updatedService));

            // Act
            var result = await _controller.Update(serviceId, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<ResultResponse<ProviderServiceResponse>>(okResult.Value);
            Assert.True(returnValue.Success);
            Assert.Equal("Updated Service", returnValue.Data.Name);
        }

        [Fact]
        public async Task Delete_WithValidId_ReturnsOkWithTrue()
        {
            // Arrange
            int providerId = 1;
            int serviceId = 1;
            SetupUserContext(providerId, "Provider");

            _mockCatalogService.Setup(s => s.DeleteAsync(providerId, serviceId))
                .ReturnsAsync(ResultResponse<bool>.Ok(true));

            // Act
            var result = await _controller.Delete(serviceId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<ResultResponse<bool>>(okResult.Value);
            Assert.True(returnValue.Success);
            Assert.True(returnValue.Data);
        }

        [Fact]
        public async Task Delete_WithNonExistentService_ReturnsError()
        {
            // Arrange
            int providerId = 1;
            int serviceId = 999;
            SetupUserContext(providerId, "Provider");

            _mockCatalogService.Setup(s => s.DeleteAsync(providerId, serviceId))
                .ReturnsAsync(ResultResponse<bool>.Fail("Service not found"));

            // Act
            var result = await _controller.Delete(serviceId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<ResultResponse<bool>>(okResult.Value);
            Assert.False(returnValue.Success);
            Assert.Contains("Service not found", returnValue.Message);
        }

        [Fact]
        public async Task UploadImage_WithValidFile_ReturnsOkWithImagePath()
        {
            // Arrange
            int providerId = 1;
            SetupUserContext(providerId, "Provider");

            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns("test.jpg");
            mockFile.Setup(f => f.Length).Returns(100000);

            _mockCatalogService.Setup(s => s.UploadImageAsync(providerId, mockFile.Object))
                .ReturnsAsync(ResultResponse<string>.Ok("/images/services/test_123.jpg"));

            // Act
            var result = await _controller.UploadImage(mockFile.Object);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<ResultResponse<string>>(okResult.Value);
            Assert.True(returnValue.Success);
            Assert.Contains("/images/services/", returnValue.Data);
        }

        [Fact]
        public async Task Create_WithInvalidPrice_ReturnsBadRequest()
        {
            // Arrange
            int providerId = 1;
            SetupUserContext(providerId, "Provider");

            var request = new ProviderServiceRequest
            {
                Name = "Service",
                Category = "Beauty",
                PriceEur = -10, // Invalid
                DurationMinutes = 60
            };

            _mockCatalogService.Setup(s => s.CreateAsync(providerId, request))
                .ReturnsAsync(ResultResponse<ProviderServiceResponse>.Fail("Price must be non-negative"));

            // Act
            var result = await _controller.Create(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<ResultResponse<ProviderServiceResponse>>(okResult.Value);
            Assert.False(returnValue.Success);
            Assert.Contains("Price", returnValue.Message);
        }

        [Fact]
        public async Task Search_WithEmptyQuery_ReturnsEmptyList()
        {
            // Arrange
            var emptyServices = new List<ProviderServiceResponse>();
            _mockCatalogService.Setup(s => s.SearchAsync(null, null))
                .ReturnsAsync(ResultResponse<List<ProviderServiceResponse>>.Ok(emptyServices));

            // Act
            var result = await _controller.Search(null, null);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<ResultResponse<List<ProviderServiceResponse>>>(okResult.Value);
            Assert.True(returnValue.Success);
            Assert.Empty(returnValue.Data);
        }
    }
}
