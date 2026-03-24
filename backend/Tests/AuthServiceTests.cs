using backend.DTOs;
using backend.Helpers;
using backend.Models;
using backend.Repositories;
using backend.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace backend.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly AuthService _service;

        public AuthServiceTests()
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
                    ["AppConfig:ProviderCatalog:PredefinedCategories:0"] = "Haircut"
                })
                .Build());

            _userRepositoryMock = new Mock<IUserRepository>();
            _service = new AuthService(_userRepositoryMock.Object);
        }

        [Fact]
        public async Task LoginAsync_WhenUserMissing_ReturnsFail()
        {
            _userRepositoryMock.Setup(r => r.GetByEmailAsync("missing@test.com")).ReturnsAsync((User?)null);

            var result = await _service.LoginAsync(new LoginRequest
            {
                Email = "missing@test.com",
                Password = "x"
            });

            Assert.False(result.Success);
            Assert.Contains("Invalid email or password", result.Message ?? string.Empty);
        }

        [Fact]
        public async Task LoginAsync_WhenInactiveUser_ReturnsFail()
        {
            var user = new User
            {
                Email = "user@test.com",
                Username = "user",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("pass123"),
                IsActive = false,
                Role = UserRole.User
            };

            _userRepositoryMock.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);

            var result = await _service.LoginAsync(new LoginRequest
            {
                Email = user.Email,
                Password = "pass123"
            });

            Assert.False(result.Success);
            Assert.Contains("deactivated", result.Message ?? string.Empty);
        }

        [Fact]
        public async Task LoginAsync_WhenValid_ReturnsTokenAndRole()
        {
            var user = new User
            {
                Id = 10,
                Email = "user@test.com",
                Username = "user",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("pass123"),
                IsActive = true,
                Role = UserRole.Provider
            };

            _userRepositoryMock.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);

            var result = await _service.LoginAsync(new LoginRequest
            {
                Email = user.Email,
                Password = "pass123"
            });

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.False(string.IsNullOrWhiteSpace(result.Data.Token));
            Assert.Equal("Provider", result.Data.Role);
            Assert.Equal("user", result.Data.Username);
        }

        [Fact]
        public async Task RegisterAsync_WhenEmailExists_ReturnsFail()
        {
            _userRepositoryMock.Setup(r => r.EmailExistsAsync("existing@test.com")).ReturnsAsync(true);

            var result = await _service.RegisterAsync(new RegisterRequest
            {
                Email = "existing@test.com",
                Username = "newuser",
                Password = "p",
                FirstName = "A",
                LastName = "B",
                Role = "User"
            });

            Assert.False(result.Success);
            Assert.Contains("Email is already taken", result.Message ?? string.Empty);
        }

        [Fact]
        public async Task RegisterAsync_WhenProviderWithoutCompany_ReturnsFail()
        {
            _userRepositoryMock.Setup(r => r.EmailExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
            _userRepositoryMock.Setup(r => r.UsernameExistsAsync(It.IsAny<string>())).ReturnsAsync(false);

            var result = await _service.RegisterAsync(new RegisterRequest
            {
                Email = "provider@test.com",
                Username = "provider1",
                Password = "p",
                FirstName = "A",
                LastName = "B",
                Role = "Provider",
                CompanyName = "   "
            });

            Assert.False(result.Success);
            Assert.Contains("Company name", result.Message ?? string.Empty);
        }

        [Fact]
        public async Task RegisterAsync_WhenValidUser_CreatesAndReturnsAuth()
        {
            User? created = null;
            _userRepositoryMock.Setup(r => r.EmailExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
            _userRepositoryMock.Setup(r => r.UsernameExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
            _userRepositoryMock
                .Setup(r => r.CreateAsync(It.IsAny<User>()))
                .Callback<User>(u => created = u)
                .ReturnsAsync((User u) => u);

            var result = await _service.RegisterAsync(new RegisterRequest
            {
                Email = "new@test.com",
                Username = "newuser",
                Password = "pass123",
                FirstName = "John",
                LastName = "Doe",
                Role = "User"
            });

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.NotNull(created);
            Assert.Equal(UserRole.User, created!.Role);
            Assert.True(BCrypt.Net.BCrypt.Verify("pass123", created.PasswordHash));
            Assert.Equal("User", result.Data.Role);
        }
    }
}
