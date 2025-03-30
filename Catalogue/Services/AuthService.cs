using Catalogue.Data;
using Catalogue.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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

        public async Task<string> GeneratePasswordResetToken(string email)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.email == email);
            if (user == null) return "User not found";

            var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray()); // Token random
            var expiration = DateTime.UtcNow.AddHours(1); // Expiră într-o oră

            var resetToken = new PasswordResetToken
            {
                UserId = user.id,
                Token = token,
                Expiration = expiration
            };

            _dbContext.PasswordResetTokens.Add(resetToken);
            await _dbContext.SaveChangesAsync();

            // Aici ar trebui să trimiți tokenul prin email (de implementat separat)
            return token;
        }

        public async Task<string> ResetPassword(string token, string newPassword)
        {
            var resetToken = await _dbContext.PasswordResetTokens.FirstOrDefaultAsync(t => t.Token == token);
            if (resetToken == null || resetToken.Expiration < DateTime.UtcNow)
                return "Invalid or expired token";

            var user = await _dbContext.Users.FindAsync(resetToken.UserId);
            if (user == null) return "User not found";

            user.SetPassword(newPassword);
            _dbContext.PasswordResetTokens.Remove(resetToken);
            await _dbContext.SaveChangesAsync();

            return "Password reset successful";
        }
    }
}
