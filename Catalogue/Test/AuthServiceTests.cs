using Xunit;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Catalogue.Data;
using Catalogue.Models;
using Catalogue.Services;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;

namespace Catalogue.Tests
{
    public class AuthServiceTests
    {
        private readonly DbContextOptions<MyDbContext> _options;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;

        public AuthServiceTests()
        {
            _options = new DbContextOptionsBuilder<MyDbContext>()
                .UseInMemoryDatabase(databaseName: "AuthTestDb")
                .Options;

            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext();
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(context);
        }

        [Fact]
        public async Task RegisterAsync_NewUser_ReturnsSuccessMessage()
        {
            // Arrange
            using var context = new MyDbContext(_options);
            var service = new AuthService(context, _httpContextAccessorMock.Object);

            // Act
            var result = await service.RegisterAsync(
                "test@example.com",
                "Password123",
                "John",
                "Doe",
                "Student");

            // Assert
            Assert.Equal("User registered successfully", result);
            var user = await context.Users.FirstOrDefaultAsync(u => u.email == "test@example.com");
            Assert.NotNull(user);
            Assert.True(user.VerifyPassword("Password123"));
        }

        [Fact]
        public async Task RegisterAsync_ExistingEmail_ReturnsErrorMessage()
        {
            // Arrange
            using var context = new MyDbContext(_options);
            var service = new AuthService(context, _httpContextAccessorMock.Object);

            await service.RegisterAsync(
                "test@example.com",
                "Password123",
                "John",
                "Doe",
                "Student");

            // Act
            var result = await service.RegisterAsync(
                "test@example.com",
                "Password456",
                "Jane",
                "Doe",
                "Student");

            // Assert
            Assert.Equal("Email already exists", result);
        }


        [Fact]
        public async Task LoginAsync_InvalidCredentials_ReturnsFalse()
        {
            // Arrange
            using var context = new MyDbContext(_options);
            var service = new AuthService(context, _httpContextAccessorMock.Object);

            await service.RegisterAsync(
                "test@example.com",
                "Password123",
                "John",
                "Doe",
                "Student");

            // Act
            var result = await service.LoginAsync("test@example.com", "WrongPassword");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ResetPassword_ExistingEmail_ReturnsSuccessMessage()
        {
            // Arrange
            using var context = new MyDbContext(_options);
            var service = new AuthService(context, _httpContextAccessorMock.Object);

            await service.RegisterAsync(
                "test@example.com",
                "Password123",
                "John",
                "Doe",
                "Student");

            // Act
            var result = await service.ResetPassword("test@example.com", "NewPassword123");

            // Assert
            Assert.Equal("Password reset successfully.", result);
            var user = await context.Users.FirstOrDefaultAsync(u => u.email == "test@example.com");
            Assert.True(user.VerifyPassword("NewPassword123"));
        }

        [Fact]
        public async Task ResetPassword_NonExistingEmail_ReturnsGenericMessage()
        {
            // Arrange
            using var context = new MyDbContext(_options);
            var service = new AuthService(context, _httpContextAccessorMock.Object);

            // Act
            var result = await service.ResetPassword("nonexistent@example.com", "NewPassword123");

            // Assert
            Assert.Equal("If the email exists, you will receive a password reset confirmation.", result);
        }
    }
}