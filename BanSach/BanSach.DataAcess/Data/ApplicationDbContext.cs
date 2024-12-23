﻿using BanSach.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BanSach.DataAcess.Data
{
    public class ApplicationDbContext: IdentityDbContext
    {
        // hàm tạo
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
    }
}
