using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class ErtekmentoDbContext : DbContext
    {
        // A konstruktoron keresztül kapja meg a beállításokat (pl. Connection String)
        public ErtekmentoDbContext(DbContextOptions options) : base(options)
        {
        }

        // Itt definiáljuk a táblákat (DbSet)
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<ProductCategory> ProductCategories { get; set; } = null!;
        public DbSet<County> Counties { get; set; } = null!;
        public DbSet<ChatMessage> ChatMessages { get; set; } = null!;
        public DbSet<Chat> Chats { get; set; } = null!;
        public DbSet<ProductRequest> ProductRequests { get; set; }
        public DbSet<RequesterLimitRule> RequesterLimitRules { get; set; }
        public DbSet<RequesterLimitUsage> RequesterLimitUsages { get; set; }

        // Ez a metódus felel az adatbázis finomhangolásáért (Fluent API)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // TÁBLANEVEK
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Product>().ToTable("Products");
            modelBuilder.Entity<ProductCategory>().ToTable("ProductCategory");
            modelBuilder.Entity<County>().ToTable("Counties");
            modelBuilder.Entity<ProductRequest>().ToTable("ProductRequests");
            modelBuilder.Entity<RequesterLimitRule>().ToTable("RequesterLimitRule");
            modelBuilder.Entity<RequesterLimitUsage>().ToTable("RequesterLimitUsage");
            modelBuilder.Entity<Chat>().ToTable("Chat");
            modelBuilder.Entity<ChatMessage>().ToTable("ChatMessage");

            // USER
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.UserId);

                entity.Property(u => u.Email)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(u => u.PasswordHash)
                    .HasMaxLength(255)
                    .IsRequired();

                entity.Property(u => u.UserName)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(u => u.FirstName)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(u => u.LastName)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(u => u.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(u => u.UpdatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.HasIndex(u => u.Email).IsUnique();
                entity.HasIndex(u => u.UserName).IsUnique();
            });

            // COUNTY
            modelBuilder.Entity<County>(entity =>
            {
                entity.HasKey(c => c.CountyId);

                entity.Property(c => c.CountyName)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.HasIndex(c => c.CountyName).IsUnique();
            });

            // PRODUCT CATEGORY
            modelBuilder.Entity<ProductCategory>(entity =>
            {
                entity.HasKey(pc => pc.ProductCategoryId);

                entity.Property(pc => pc.ProductCategoryName)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(pc => pc.IsActive)
                    .HasDefaultValue(true);

                entity.HasIndex(pc => pc.ProductCategoryName).IsUnique();
            });

            // PRODUCT
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.ProductId);

                entity.Property(p => p.ProductName)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(p => p.ImagePath)
                    .HasMaxLength(500);

                entity.Property(p => p.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(p => p.UpdatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.HasIndex(p => p.ProductCategoryId)
                    .HasDatabaseName("IX_Products_CategoryId");

                entity.HasIndex(p => p.SenderId)
                    .HasDatabaseName("IX_Products_SenderId");

                entity.HasIndex(p => p.CountyId)
                    .HasDatabaseName("IX_Products_CountyId");

                entity.HasOne(p => p.ProductCategory)
                    .WithMany(c => c.Products)
                    .HasForeignKey(p => p.ProductCategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.County)
                    .WithMany(c => c.Products)
                    .HasForeignKey(p => p.CountyId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(p => p.SenderId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // PRODUCT REQUEST
            modelBuilder.Entity<ProductRequest>(entity =>
            {
                entity.HasKey(pr => pr.ProductRequestId);

                entity.Property(pr => pr.RequestedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.HasIndex(pr => pr.RequesterId)
                    .HasDatabaseName("IX_ProductRequests_RequesterId");

                entity.HasIndex(pr => pr.ProductId)
                    .HasDatabaseName("IX_ProductRequests_ProductId");

                entity.HasOne<Product>()
                    .WithMany()
                    .HasForeignKey(pr => pr.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(pr => pr.RequesterId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // REQUESTER LIMIT RULE
            modelBuilder.Entity<RequesterLimitRule>(entity =>
            {
                entity.HasKey(r => r.RequesterLimitRuleId);

                entity.Property(r => r.PeriodType)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(r => r.RequesterLimitRuleDescription)
                    .HasMaxLength(500);

                entity.Property(r => r.IsActive)
                    .HasDefaultValue(true);

                entity.Property(r => r.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(r => r.UpdatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne<ProductCategory>()
                    .WithMany()
                    .HasForeignKey(r => r.RequesterLimitRuleCategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // REQUESTER LIMIT USAGE
            modelBuilder.Entity<RequesterLimitUsage>(entity =>
            {
                entity.HasKey(u => u.RequesterLimitUsageId);

                entity.Property(u => u.UsedQuantity)
                    .HasDefaultValue(0);

                entity.Property(u => u.LastResetAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.HasIndex(u => new { u.RequesterId, u.RuleId, u.PeriodStart })
                    .IsUnique()
                    .HasDatabaseName("UC_RequesterLimitUsage");

                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(u => u.RequesterId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<RequesterLimitRule>()
                    .WithMany()
                    .HasForeignKey(u => u.RuleId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // CHAT
            modelBuilder.Entity<Chat>(entity =>
            {
                entity.HasKey(c => c.ChatId);

                entity.Property(c => c.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.HasIndex(c => c.ProductRequestId)
                    .IsUnique();

                entity.HasOne<ProductRequest>()
                    .WithMany()
                    .HasForeignKey(c => c.ProductRequestId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(c => c.SenderId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(c => c.RequesterId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // CHAT MESSAGE
            modelBuilder.Entity<ChatMessage>(entity =>
            {
                entity.HasKey(cm => cm.ChatMessageId);

                entity.Property(cm => cm.IsRead)
                    .HasDefaultValue(false);

                entity.HasIndex(cm => cm.ChatId)
                    .HasDatabaseName("IX_ChatMessage_ChatId");

                entity.HasOne<Chat>()
                    .WithMany()
                    .HasForeignKey(cm => cm.ChatId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(cm => cm.SenderId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
