using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class County
    {
        [Key]
        public int CountyId { get; set; }
        public string CountyName { get; set; } = string.Empty;

        // Navigáció
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
