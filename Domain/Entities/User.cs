using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        // Kötelező mezők string.Empty-vel a null-safety érdekében
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public UserRole UserRole { get; set; } // Enum

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Foreign Keys
        public int CountyId { get; set; }

        // Navigáció
        public virtual County County { get; set; } = null!;

        // felhasználóknak sok terméke lehet
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
