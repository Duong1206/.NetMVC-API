using BanSach.Application.Interfaces;
using BanSach.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BanSach.Persistence.Context;

public class ApplicationDbContext : IdentityDbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<CoverType> CoverTypes => Set<CoverType>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ApplicationUser> ApplicationUsers => Set<ApplicationUser>();
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<ShoppingCart> ShoppingCarts => Set<ShoppingCart>();
    public DbSet<OrderHeader> OrderHeaders => Set<OrderHeader>();
    public DbSet<OrderDetail> OrderDetails => Set<OrderDetail>();
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<Coupon> Coupons => Set<Coupon>();
    public DbSet<Review> Reviews => Set<Review>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Category Indexes
        modelBuilder.Entity<Category>()
            .HasIndex(c => c.DisplayOrder)
            .HasDatabaseName("IX_Category_DisplayOrder");

        modelBuilder.Entity<Category>()
            .HasIndex(c => c.CreatedDate)
            .HasDatabaseName("IX_Category_CreatedDate");

        // Product Indexes
        modelBuilder.Entity<Product>()
            .HasIndex(p => p.CategoryId)
            .HasDatabaseName("IX_Product_CategoryId");

        modelBuilder.Entity<Product>()
            .HasIndex(p => p.SoldCount)
            .HasDatabaseName("IX_Product_SoldCount");

        modelBuilder.Entity<Product>()
            .HasIndex(p => p.Quantity)
            .HasDatabaseName("IX_Product_Quantity");

        modelBuilder.Entity<Product>()
            .HasIndex(p => p.Name)
            .HasDatabaseName("IX_Product_Name");

        // OrderHeader Indexes
        modelBuilder.Entity<OrderHeader>()
            .HasIndex(o => o.OrderDate)
            .HasDatabaseName("IX_OrderHeader_OrderDate");

        modelBuilder.Entity<OrderHeader>()
            .HasIndex(o => o.PaymentStatus)
            .HasDatabaseName("IX_OrderHeader_PaymentStatus");

        modelBuilder.Entity<OrderHeader>()
            .HasIndex(o => o.ApplicationUserId)
            .HasDatabaseName("IX_OrderHeader_ApplicationUserId");

        // OrderDetail Indexes
        modelBuilder.Entity<OrderDetail>()
            .HasIndex(od => od.OrderId)
            .HasDatabaseName("IX_OrderDetail_OrderId");

        modelBuilder.Entity<OrderDetail>()
            .HasIndex(od => od.ProductId)
            .HasDatabaseName("IX_OrderDetail_ProductId");

        // Review Indexes
        modelBuilder.Entity<Review>()
            .HasIndex(r => r.ProductId)
            .HasDatabaseName("IX_Review_ProductId");

        modelBuilder.Entity<Review>()
            .HasIndex(r => r.ApplicationUserId)
            .HasDatabaseName("IX_Review_ApplicationUserId");

        // Configure Relationships
        modelBuilder.Entity<Product>()
            .HasMany(p => p.Reviews)
            .WithOne(r => r.Product)
            .HasForeignKey(r => r.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Review>()
            .HasOne(r => r.ApplicationUser)
            .WithMany()
            .HasForeignKey(r => r.ApplicationUserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
