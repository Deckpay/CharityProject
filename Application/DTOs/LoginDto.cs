using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Felhasználónév vagy email megadása kötelező")]
        public string EmailOrUserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Jelszó megadás kötelező")]
        public string Password { get; set; } = string.Empty;
    }
}
