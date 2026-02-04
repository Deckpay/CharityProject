using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
