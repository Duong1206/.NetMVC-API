using BanSach.Persistence.Context;
using BanSach.DataAccess.Repository.IRepository;
using BanSach.Model;

namespace BanSach.DataAccess.Repository
{
    public class CouponRepository : Repository<Coupon>, ICouponRepository
    {
        //t?o 1 bi?n 
        private readonly ApplicationDbContext _db;


        //h�m kh?i t?o
        public CouponRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;

        }
        /*    public void Save()
            {
                _db.SaveChanges();
            }*/

        public void Update(Coupon coupon)
        {
            _db.Coupons?.Update(coupon);
        }
    }
}
