using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class ProductCategory
    {
        [Key]
        public int ProductCategoryId { get; set; }
        public string ProductCategoryName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        // Navigáció - Egy kategóriába sok termék tartozhat
        public ICollection<Product> Products { get; set; } = new List<Product>();

        //Ez az Inverz Navigáció. Lehetővé teszi, hogy ha van egy kategória objektumod,
        //egyetlen mozdulattal lekérd az összes oda tartozó terméket (pl. category.Products)


        // string? (Nullable): Azt jelenti, hogy a név lehet "semmi" (null) is. De gondolj bele:
        // egy megyének vagy egy terméknek lehet "null" a neve? Nem. Ha null lenne,
        // elszállna a programod egy hibaüzenettel később.

        //string.Empty + nincs kérdőjel: Ezzel azt mondod a fordítónak:
        //"Ez a mező kötelező, soha nem lehet null!". A string.Empty csak egy
        //biztonsági kezdőérték, hogy ne kapj figyelmeztetést a
        //Visual Studio-ban("Non-nullable property must contain a non-null value").
    }
}
