using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{

    public class ErtekmentoDbContext : DbContext
    {
        public ErtekmentoDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<ProductCategory> ProductCategories { get; set; } = null!;
        public DbSet<County> Counties { get; set; } = null!;
        public DbSet<ChatMessage> ChatMessages { get; set; } = null!;
        public DbSet<Chat> Chats { get; set; } = null!;
        public DbSet<ProductRequest> ProductRequests { get; set; } = null!;
        public DbSet<RequesterLimitRule> RequesterLimitRules { get; set; } = null!;
        public DbSet<RequesterLimitUsage> RequesterLimitUsages { get; set; } = null!;

        // Ez a metódus felel az adatbázis finomhangolásáért (Fluent API)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 2. Termék és Sender (User) kapcsolata (1:N)
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Sender)
                .WithMany(u => u.Products)
                .HasForeignKey(p => p.SenderId)
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

            // 5. chat uzenetek kapcsolata
            modelBuilder.Entity<ChatMessage>(entity => {
                entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict); // nem törlunk uzenetet ha felfuggesztik a felhasználót
            });

            modelBuilder.Entity<ProductCategory>().ToTable("ProductCategory");
            modelBuilder.Entity<RequesterLimitRule>().ToTable("RequesterLimitRule");
            modelBuilder.Entity<RequesterLimitUsage>().ToTable("RequesterLimitUsage");
        }
    }
}
