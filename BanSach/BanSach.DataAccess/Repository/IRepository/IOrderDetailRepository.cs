using BanSach.Model;

namespace BanSach.DataAccess.Repository.IRepository
{
    public interface IOrderDetailRepository : IRepository<OrderDetail>
    {
        void Update(OrderDetail obj);

        int GetSoldCountForProduct(int productId);

    }
}
