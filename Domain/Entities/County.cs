using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class County
    {
        [Key]
        public int CountyId { get; set; }
        public string CountyName { get; set; } = string.Empty;

        // Navigáció
        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<Product> Products { get; set; } = new List<Product>();
        //Ez az Inverz Navigáció. Lehetővé teszi, hogy ha van egy kategória objektumod,
        //egyetlen mozdulattal lekérd az összes oda tartozó terméket
        //pl.: Keresed a Budapesti usereket county.Users
    }
}
