using BanSach.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BanSach.DataAccess.Data
{
    public class ApplicationDbContext: IdentityDbContext
    {
        // h�m t?o
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Category>? Categories{ get; set; }
        public DbSet<CoverType>? CoverTypes{ get; set; }
        public DbSet<Product>? Products{ get; set; }
        public DbSet<ApplicationUser>? ApplicationUsers { get; set; }
        public DbSet<Company>? companies { get; set; }
        public DbSet<ShoppingCart>? ShoppingCarts { get; set; }
        public DbSet<OrderHeader>? OrderHeaders { get; set; }
        public DbSet<OrderDetail>? OrderDetails { get; set; }
        public DbSet<Contact>? Contacts { get; set; }
        public DbSet<Coupon>? Coupons { get; set; }
        public DbSet<Review>? Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Performance Indexes for Category
            modelBuilder.Entity<Category>()
                .HasIndex(c => c.DisplayOrder)
                .HasDatabaseName("IX_Category_DisplayOrder");

            modelBuilder.Entity<Category>()
                .HasIndex(c => c.CreatedDate)
                .HasDatabaseName("IX_Category_CreatedDate");

            // Performance Indexes for Product
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

            // Composite index for product filtering by category and popularity
            modelBuilder.Entity<Product>()
                .HasIndex(p => new { p.CategoryId, p.SoldCount })
                .HasDatabaseName("IX_Product_CategoryId_SoldCount");

            // Performance Indexes for OrderHeader
            modelBuilder.Entity<OrderHeader>()
                .HasIndex(o => o.OrderDate)
                .HasDatabaseName("IX_OrderHeader_OrderDate");

            modelBuilder.Entity<OrderHeader>()
                .HasIndex(o => o.PaymentStatus)
                .HasDatabaseName("IX_OrderHeader_PaymentStatus");

            modelBuilder.Entity<OrderHeader>()
                .HasIndex(o => o.ApplicationUserId)
                .HasDatabaseName("IX_OrderHeader_ApplicationUserId");

            // Performance Indexes for OrderDetail
            modelBuilder.Entity<OrderDetail>()
                .HasIndex(od => od.OrderId)
                .HasDatabaseName("IX_OrderDetail_OrderId");

            modelBuilder.Entity<OrderDetail>()
                .HasIndex(od => od.ProductId)
                .HasDatabaseName("IX_OrderDetail_ProductId");

            // Performance Indexes for ShoppingCart
            modelBuilder.Entity<ShoppingCart>()
                .HasIndex(sc => sc.ApplicationUserId)
                .HasDatabaseName("IX_ShoppingCart_ApplicationUserId");

            // Performance Indexes for Review
            modelBuilder.Entity<Review>()
                .HasIndex(r => r.ProductId)
                .HasDatabaseName("IX_Review_ProductId");
        }
    }
}
