using Domain.Security;
using Infraestructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Security
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AppDbContext _context;

        public AuthRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<User?> GetUserByUsernameAsync(string username)
        => _context.Users.FirstOrDefaultAsync(u => u.Username == username.ToLower() && u.IsActive);
        public Task<User?> GetUserByEmailAsync(string email)
            => _context.Users.FirstOrDefaultAsync(u => u.Email == email.ToLower() && u.IsActive);

        public Task<User?> GetUserByIdAsync(int id)
            => _context.Users.FirstOrDefaultAsync(u => u.Id == id && u.IsActive);

        public async Task<User> CreateUserAsync(User user)
        {
            user.Username = user.Username.ToLower();
            user.Email = user.Email.ToLower();
            user.IsActive = true; // Set the user as active by default
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public Task<bool> UpdateUserAsync(User user)
        {
            try
            {
                user.Username = user.Username.ToLower();
                user.Email = user.Email.ToLower();
                _context.Users.Update(user);
                return _context.SaveChangesAsync().ContinueWith(t => t.Result > 0);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }
    }
}
