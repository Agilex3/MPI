using Catalogue.Data;
using Catalogue.Models;
using Microsoft.IdentityModel.Tokens;
using System.Data.Entity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Catalogue.Services
{
    public class AuthService
    {
        private readonly MyDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public AuthService(MyDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
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

            user.SetPassword(password); // Fără hashing

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            return GenerateJwtToken(user);
        }

        public async Task<string> LoginAsync(string email, string password)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.email == email);
            if (user == null || !user.VerifyPassword(password)) // Comparare directă
                return "Invalid credentials";

            return GenerateJwtToken(user);
        }


        private string GenerateJwtToken(User user)
        {
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.id.ToString()),
            new Claim(ClaimTypes.Email, user.email),
            new Claim(ClaimTypes.Role, user.role)
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(token);
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

            // Aici trimiți tokenul prin email (de implementat separat)
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
