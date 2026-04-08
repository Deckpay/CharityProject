using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class ChangePasswordDto
    {
        [Required]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required]
        [MinLength(6, ErrorMessage = "A jelszónak legalább 6 karakter hosszúnak kell lennie.")]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        [MinLength(6, ErrorMessage = "A jelszónak legalább 6 karakter hosszúnak kell lennie.")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}
