using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;
        public string? ProductDescription { get; set; }
        public string? ImagePath { get; set; }

        public ProductStatus ProductStatus { get; set; } = ProductStatus.Active; // Enum használata status ként
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // szerveridőt utcnow ban kell tárolni
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Foreign Key
        public int SenderId { get; set; }
        public int ProductCategoryId { get; set; }
        public int CountyId { get; set; }

        // Navigáció
        public User Sender { get; set; } = null!;
        public ProductCategory ProductCategory { get; set; } = null!;
        public County County { get; set; } = null!;
    }
}
