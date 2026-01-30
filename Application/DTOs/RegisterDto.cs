using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "A felhasználónév kötelező")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "At email cím kötelező")]
        [EmailAddress(ErrorMessage = "Érvénytelen email formátum")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A jelszó kötelező")]
        [MinLength(6, ErrorMessage = "A jelszónak legalább 6 karakternek kell lennie")]
        public string Password { get; set; } = string.Empty;

        [Compare("Password", ErrorMessage = "A két jelszó nem egyezik")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "A megye kiválasztása kötelező")]
        public int CountyId { get; set; }

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "A szerepkör kiválasztása kötelező")]
        public int RoleId { get; set; } // Az enum értékét int-ként kezeljük a lenyílóban
    }
}
