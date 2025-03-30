using Catalogue.Data;
using Catalogue.Models;
using System;

namespace Catalogue.Services
{
    public class UserService
    {
        private readonly MyDbContext _context;

        public UserService(MyDbContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return _context.Users.ToList();
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task CreateUserAsync(User user)
        {
            user.created_at = DateTime.Now;
            user.SetPassword(user.password);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(User updatedUser)
        {
            var user = await _context.Users.FindAsync(updatedUser.id);
            if (user == null) return;

            user.first_name = updatedUser.first_name;
            user.last_name = updatedUser.last_name;
            user.email = updatedUser.email;
            user.role = updatedUser.role;

            if (!string.IsNullOrEmpty(updatedUser.password))
                user.SetPassword(updatedUser.password);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}

