

using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Security
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        public string Username { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "La contraseña es requerida")]
        public string Password { get; set; } = string.Empty;
        //public bool RememberMe { get; set; } = false; // Indica si se debe recordar al usuario
    }
}
