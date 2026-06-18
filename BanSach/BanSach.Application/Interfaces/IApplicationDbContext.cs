using BanSach.Model;
using Microsoft.EntityFrameworkCore;

namespace BanSach.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Category> Categories { get; }
    DbSet<Product> Products { get; }
    DbSet<OrderHeader> OrderHeaders { get; }
    DbSet<OrderDetail> OrderDetails { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
