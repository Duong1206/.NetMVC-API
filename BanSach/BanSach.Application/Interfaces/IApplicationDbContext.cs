using BanSach.Model;
using Microsoft.EntityFrameworkCore;

namespace BanSach.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Category> Categories { get; }
    DbSet<CoverType> CoverTypes { get; }
    DbSet<Product> Products { get; }
    DbSet<Review> Reviews { get; }
    DbSet<OrderHeader> OrderHeaders { get; }
    DbSet<OrderDetail> OrderDetails { get; }
    DbSet<ApplicationUser> ApplicationUsers { get; }
    DbSet<Company> Companies { get; }
    DbSet<ShoppingCart> ShoppingCarts { get; }
    DbSet<Contact> Contacts { get; }
    DbSet<Coupon> Coupons { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

