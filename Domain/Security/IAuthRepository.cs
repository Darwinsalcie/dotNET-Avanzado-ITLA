

namespace Domain.Security
{
    public interface IAuthRepository
    {
        // Métodos de autenticación y autorización
        // User? => significa que puede retornar null si no se encuentra el usuario
        Task<User?> GetUserByUsernameAsync(string username);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByIdAsync(int id);
        Task<User> CreateUserAsync(User user);
        Task<bool> UpdateUserAsync(User user);
    }
}
