using Catalogue.Controllers;
using Catalogue.Data;
using Catalogue.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static System.Net.WebRequestMethods;

namespace Catalogue.Services
{
    public class AuthService
    {
        private readonly MyDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(MyDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> RegisterAsync(string email, string password, string firstName, string lastName, string role)
        {
            if (await _dbContext.Users.AnyAsync(u => u.email == email))
                return "Email already exists";

            var user = new User
            {
                email = email,
                first_name = firstName,
                last_name = lastName,
                role = role,
                created_at = DateTime.UtcNow
            };

            user.SetPassword(password); // Setează parola (ar trebui să fie hashuită)

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            return "User registered successfully";
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.email == email);
            if (user == null || !user.VerifyPassword(password)) // Verifică parola
                return false;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.id.ToString()),
                new Claim(ClaimTypes.Email, user.email),
                new Claim(ClaimTypes.Role, user.role)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true
            };

            await _httpContextAccessor.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return true;
        }

        public async Task LogoutAsync()
        {
            await _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public async Task<string> ResetPassword(string email, string newPassword)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.email == email);
            if (user == null)
                return "If the email exists, you will receive a password reset confirmation."; // Avoid exposing valid emails

            user.SetPassword(newPassword); // Hash and save the new password
            await _dbContext.SaveChangesAsync();

            return "Password reset successfully.";
        }

    }
}
