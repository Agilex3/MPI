using Catalogue.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catalogue.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var result = await _authService.RegisterAsync(model.Email, model.Password, model.FirstName, model.LastName, model.Role);
            if (result == "Email already exists")
                return BadRequest(new { message = result });

            return Ok(new { message = "User registered successfully" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var success = await _authService.LoginAsync(model.Email, model.Password);
            if (!success)
                return Unauthorized(new { message = "Invalid credentials" });

            return Ok(new { message = "Login successful" });
        }

        [HttpPost("logout")]
        [Authorize] // Asigură că utilizatorul trebuie să fie logat pentru a face logout
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            return Ok(new { message = "Logout successful" });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordModel model)
        {
            var token = await _authService.GeneratePasswordResetToken(model.Email);
            return Ok(new { token }); // Ar trebui să trimiți prin email în practică
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        {
            var result = await _authService.ResetPassword(model.Token, model.NewPassword);
            return Ok(new { message = result });
        }
    }

    public class RegisterModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
    }

    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class ForgotPasswordModel
    {
        public string Email { get; set; }
    }

    public class ResetPasswordModel
    {
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }
}
