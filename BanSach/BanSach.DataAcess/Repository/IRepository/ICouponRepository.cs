using BanSach.Model;

namespace BanSach.DataAcess.Repository.IRepository
{
    public interface ICouponRepository : IRepository<Coupon>
    {
        void Update(Coupon coupon);
    }
}
