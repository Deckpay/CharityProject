using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class ProductCategory
    {
        [Key]
        public int ProductCategoryId { get; set; }
        public string ProductCategoryName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        // Navigáció
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
