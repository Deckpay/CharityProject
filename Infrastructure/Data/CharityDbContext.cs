using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class CharityDbContext : DbContext
    {
        // A konstruktoron keresztül kapja meg a beállításokat (pl. Connection String)
        public CharityDbContext(DbContextOptions options) : base(options)
        {
        }

        // Itt definiáljuk a táblákat (DbSet)
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<ProductCategory> ProductCategories { get; set; } = null!;
        public DbSet<County> Counties { get; set; } = null!;

        // Ez a metódus felel az adatbázis finomhangolásáért (Fluent API)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Felhasználó és Megye kapcsolata (1:N)
            modelBuilder.Entity<User>()
                .HasOne(u => u.County)
                .WithMany(c => c.Users)
                .HasForeignKey(u => u.CountyId)
                .OnDelete(DeleteBehavior.Restrict);

            // 2. Termék és Donor (User) kapcsolata (1:N)
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Donor)
                .WithMany(u => u.Products)
                .HasForeignKey(p => p.DonorId)
                .OnDelete(DeleteBehavior.Cascade); // Ha törlünk egy usert, törlődjenek a termékei

            // 3. Termék és Kategória kapcsolata (1:N)
            modelBuilder.Entity<Product>()
                .HasOne(p => p.ProductCategory)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.ProductCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // 4. Termék és Megye kapcsolata (1:N)
            modelBuilder.Entity<Product>()
                .HasOne(p => p.County)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CountyId)
                .OnDelete(DeleteBehavior.Restrict);

            // Enum konverzió (Opcionális: Ha szövegként akarnád tárolni az adatbázisban)
            // Itt most hagyjuk alapértelmezett INT-en, ahogy beszéltük.

            // Ha az adatbázisban pl. "ProductCategory" a tábla neve (egyes számban):
            modelBuilder.Entity<ProductCategory>().ToTable("ProductCategory");
            modelBuilder.Entity<Product>().ToTable("Products"); // Vagy ami a valódi neve az SQL-ben
        }
    }
}
