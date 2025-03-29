using Microsoft.AspNetCore.Mvc;
using Catalogue.Data;
using Microsoft.EntityFrameworkCore;


namespace Catalogue.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly MyDbContext _context;

        public TestController(MyDbContext context)
        {
            _context = context;
        }

        [HttpGet("testConnection")]
        public async Task<IActionResult> TestConnection()
        {
            try
            {
                // Testează conexiunea la baza de date printr-o interogare simplă
                var usersCount = await _context.Users.CountAsync();
                return Ok($"Conexiunea a fost realizată cu succes! Numărul de utilizatori: {usersCount}");
            }
            catch (Exception ex)
            {
                return BadRequest($"Eroare la conectare: {ex.Message}");
            }
        }
    }
}
