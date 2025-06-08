

using Domain.Security;

namespace Application.DTOs.Security
{
    public interface IAuthService
    {
        // Métodos de autenticación y autorización
        Task<AuthResponseDTO?> LoginAsync(LoginDTO loginDto);
        Task<AuthResponseDTO?> RegisterAsync(RegisterDTO registerDto);
        Task<bool> ValidateTokenAsync(string token);
        Task<User?> GetCurrentUserAsync(string token);
    }
}
