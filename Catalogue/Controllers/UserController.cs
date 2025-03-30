using Catalogue.Data;
using Catalogue.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Catalogue.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserApiController : ControllerBase
    {
        private readonly MyDbContext _context;

        public UserApiController(MyDbContext context)
        {
            _context = context;
        }

        // GET: api/UserApi
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }

        // GET: api/UserApi/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound(new { message = "User not found" });

            return Ok(user);
        }

        // POST: api/UserApi
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] User user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            user.created_at = DateTime.Now;
            user.SetPassword(user.password); // sau doar user.password = ... dacă nu folosești hashing
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User created successfully", user });
        }

        // PUT: api/UserApi/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] User updatedUser)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound(new { message = "User not found" });

            user.first_name = updatedUser.first_name;
            user.last_name = updatedUser.last_name;
            user.email = updatedUser.email;
            user.role = updatedUser.role;

            if (!string.IsNullOrWhiteSpace(updatedUser.password))
            {
                user.SetPassword(updatedUser.password);
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "User updated successfully", user });
        }

        // DELETE: api/UserApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound(new { message = "User not found" });

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User deleted successfully" });
        }
    }
}
