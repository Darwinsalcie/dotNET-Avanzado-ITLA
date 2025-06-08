using Application.DTOs.Security;
using BCrypt.Net;
using Domain.Security;


namespace Infrastructure.Security
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly ITokenService _tokenService;

        public AuthService(IAuthRepository authRepository, ITokenService tokenService)
        {
            _authRepository = authRepository;
            _tokenService = tokenService;
        }


        public async Task<AuthResponseDTO?> LoginAsync(LoginDTO loginDto)
        {

            // Buscar usuario
            var user = await _authRepository.GetUserByUsernameAsync(loginDto.Username);

            if(user is null || !user.IsActive)
                return null;

            // Verificar contraseña
            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                return null;

            //Generar Token

            var token = _tokenService.GenerateToken(user);


            return new AuthResponseDTO
            {
               Token = token,
               Username = user.Username,
               Email = user.Email,
               Role = user.Role,
               ExpiresAt = DateTime.UtcNow.AddHours(24),
            };

        }

        public async Task<AuthResponseDTO?> RegisterAsync(RegisterDTO registerDto)
        {
            //Verificar si el usuario ya existe
            var existingUser = await _authRepository.GetUserByUsernameAsync(registerDto.Username);
            if (existingUser != null)
                return null;

            //Verificar si el email ya existe el email
            var existingEmail = await _authRepository.GetUserByEmailAsync(registerDto.Email);
            if (existingEmail != null)
                return null;


            // Crear nuevo usuario
            var user = new User
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                Role = registerDto.Role,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };


            var createdUser = await _authRepository.CreateUserAsync(user);
            var token = _tokenService.GenerateToken(createdUser);

            return new AuthResponseDTO
            {
                Token = token,
                Username = createdUser.Username,
                Email = createdUser.Email,
                Role = createdUser.Role,
                ExpiresAt = DateTime.UtcNow.AddHours(24)
            };

        }
        public async Task<bool> ValidateTokenAsync(string token)
            => _tokenService.ValidateToken(token);

        public async Task<User?> GetCurrentUserAsync(string token)
        {
            var userId = _tokenService.GetUserIdFromToken(token);
            if (userId is null)
                return null;

            return await _authRepository.GetUserByIdAsync(userId.Value);
        }
    }
}
