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
    public class AuthServiceSimpleTests
    {
        public AuthServiceSimpleTests()
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
        }

        [Fact]
        public async Task RegisterAsync_ProviderWithCompany_SetsProviderFields()
        {
            var repo = new Mock<IUserRepository>();
            User? created = null;

            repo.Setup(r => r.EmailExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
            repo.Setup(r => r.UsernameExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
            repo.Setup(r => r.CreateAsync(It.IsAny<User>()))
                .Callback<User>(u => created = u)
                .ReturnsAsync((User u) => u);

            var service = new AuthService(repo.Object);

            var result = await service.RegisterAsync(new RegisterRequest
            {
                Email = "p@test.com",
                Username = "provider",
                Password = "pass123",
                FirstName = "Ignored",
                LastName = "Ignored",
                Role = "Provider",
                CompanyName = "Studio X",
                City = "Novi Sad"
            });

            Assert.True(result.Success);
            Assert.NotNull(created);
            Assert.Equal(UserRole.Provider, created!.Role);
            Assert.Equal("Studio X", created.CompanyName);
            Assert.Equal("Studio X", created.FirstName);
            Assert.Equal(string.Empty, created.LastName);
            Assert.Equal("Novi Sad", created.City);
        }

        [Fact]
        public async Task LoginAsync_WrongPassword_ReturnsFail()
        {
            var repo = new Mock<IUserRepository>();
            repo.Setup(r => r.GetByEmailAsync("user@test.com")).ReturnsAsync(new User
            {
                Email = "user@test.com",
                Username = "user",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("correct"),
                Role = UserRole.User,
                IsActive = true
            });

            var service = new AuthService(repo.Object);

            var result = await service.LoginAsync(new LoginRequest
            {
                Email = "user@test.com",
                Password = "wrong"
            });

            Assert.False(result.Success);
            Assert.Contains("Invalid email or password", result.Message ?? string.Empty);
        }
    }
}
