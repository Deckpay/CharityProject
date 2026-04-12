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

        public ProductStatus ProductStatus { get; set; } = ProductStatus.Active;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public int SenderId { get; set; }
        public int ProductCategoryId { get; set; }
        public int CountyId { get; set; }

        // Navigáció
        public User Sender { get; set; } = null!;
        public ProductCategory ProductCategory { get; set; } = null!;
        public County County { get; set; } = null!;
    }
}
