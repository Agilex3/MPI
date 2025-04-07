using Catalogue.Services;
using Catalogue.Data;
using Catalogue.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Xunit;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Catalogue.Tests
{
    public class AuthServiceTests
    {
        private readonly Mock<MyDbContext> _dbContextMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly Mock<HttpContext> _httpContextMock;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            // Setup mocks
            _dbContextMock = new Mock<MyDbContext>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _httpContextMock = new Mock<HttpContext>();

            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(_httpContextMock.Object);

            // Initialize service with mocked dependencies
            _authService = new AuthService(_dbContextMock.Object, _httpContextAccessorMock.Object);
        }

        [Fact]
        public async Task RegisterAsync_EmailExists_ReturnsErrorMessage()
        {
            // Arrange
            var usersMock = new Mock<DbSet<User>>();
            usersMock.Setup(m => m.AnyAsync(u => u.email == "test@example.com", default))
                .ReturnsAsync(true);
            _dbContextMock.Setup(db => db.Users).Returns(usersMock.Object);

            // Act
            var result = await _authService.RegisterAsync("test@example.com", "parola123", "Ion", "Popescu", "student");

            // Assert
            Assert.Equal("Email already exists", result);
        }

        [Fact]
        public async Task RegisterAsync_NewUser_ReturnsSuccessMessage()
        {
            // Arrange
            var usersMock = new Mock<DbSet<User>>();
            usersMock.Setup(m => m.AnyAsync(u => u.email == "test@example.com", default))
                .ReturnsAsync(false);
            _dbContextMock.Setup(db => db.Users).Returns(usersMock.Object);

            // Act
            var result = await _authService.RegisterAsync("test@example.com", "parola123", "Ion", "Popescu", "student");

            // Assert
            Assert.Equal("User registered successfully", result);
            _dbContextMock.Verify(db => db.SaveChangesAsync(default), Times.Once());
        }

        [Fact]
        public async Task LoginAsync_InvalidCredentials_ReturnsFalse()
        {
            // Arrange
            var usersMock = new Mock<DbSet<User>>();
            var user = new User { id = 1, email = "test@example.com" };
            user.SetPassword("parola123");

            usersMock.Setup(m => m.FirstOrDefaultAsync(u => u.email == "test@example.com", default))
                .ReturnsAsync(user);
            _dbContextMock.Setup(db => db.Users).Returns(usersMock.Object);

            // Act
            var result = await _authService.LoginAsync("test@example.com", "wrongpassword");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task LoginAsync_ValidCredentials_ReturnsTrueAndSignsIn()
        {
            // Arrange
            var usersMock = new Mock<DbSet<User>>();
            var user = new User { id = 1, email = "test@example.com", role = "student" };
            user.SetPassword("parola123");

            usersMock.Setup(m => m.FirstOrDefaultAsync(u => u.email == "test@example.com", default))
                .ReturnsAsync(user);
            _dbContextMock.Setup(db => db.Users).Returns(usersMock.Object);

            _httpContextMock.Setup(m => m.SignInAsync(
                It.IsAny<string>(),
                It.IsAny<ClaimsPrincipal>(),
                It.IsAny<AuthenticationProperties>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _authService.LoginAsync("test@example.com", "parola123");

            // Assert
            Assert.True(result);
            _httpContextMock.Verify(m => m.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                It.Is<ClaimsPrincipal>(cp =>
                    cp.FindFirst(ClaimTypes.Email) != null &&
                    cp.FindFirst(ClaimTypes.Email).Value == "test@example.com"),
                It.IsAny<AuthenticationProperties>()),
                Times.Once());
        }

        [Fact]
        public async Task LogoutAsync_CallsSignOut()
        {
            // Arrange
            _httpContextMock.Setup(m => m.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme))
                .Returns(Task.CompletedTask);

            // Act
            await _authService.LogoutAsync();

            // Assert
            _httpContextMock.Verify(m => m.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme),
                Times.Once());
        }

        [Fact]
        public async Task ResetPassword_UserNotFound_ReturnsGenericMessage()
        {
            // Arrange
            var usersMock = new Mock<DbSet<User>>();
            usersMock.Setup(m => m.FirstOrDefaultAsync(u => u.email == "test@example.com", default))
                .ReturnsAsync((User)null);
            _dbContextMock.Setup(db => db.Users).Returns(usersMock.Object);

            // Act
            var result = await _authService.ResetPassword("test@example.com", "newpassword123");

            // Assert
            Assert.Equal("If the email exists, you will receive a password reset confirmation.", result);
        }

        [Fact]
        public async Task ResetPassword_UserExists_ResetsPasswordSuccessfully()
        {
            // Arrange
            var usersMock = new Mock<DbSet<User>>();
            var user = new User { id = 1, email = "test@example.com" };
            usersMock.Setup(m => m.FirstOrDefaultAsync(u => u.email == "test@example.com", default))
                .ReturnsAsync(user);
            _dbContextMock.Setup(db => db.Users).Returns(usersMock.Object);

            // Act
            var result = await _authService.ResetPassword("test@example.com", "newpassword123");

            // Assert
            Assert.Equal("Password reset successfully.", result);
            _dbContextMock.Verify(db => db.SaveChangesAsync(default), Times.Once());
        }
    }
}