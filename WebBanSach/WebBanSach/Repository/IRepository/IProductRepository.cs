using WebBanSach.Models;

namespace WebBanSach.Repository.IRepository
{
    public interface IProductRepository : IRepository<Product>
    {
        void Update(Product product);
    }
}
