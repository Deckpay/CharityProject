using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class ProductDto
    {
        public int ProductId { get; set; }

        [Required(ErrorMessage = "A termék neve kötelező")]
        public string ProductName { get; set; } = string.Empty;

        [Required(ErrorMessage = "A termék leírása kötelező")]
        public string? ProductDescription { get; set; } = string.Empty;

        public string? ImagePath { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public ProductStatus ProductStatus { get; set; }

        public int DonorId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Kategória választása kötelező")]
        public int ProductCategoryId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Megye választása kötelező")]
        public int CountyId { get; set; }
    }
}
