

namespace Domain.Security
{
    public interface ITokenService
    {
        string GenerateToken(User user);
        bool ValidateToken(string token);
        int? GetUserIdFromToken(string token);
        string? GetRoleFromToken(string token);
    }
}
