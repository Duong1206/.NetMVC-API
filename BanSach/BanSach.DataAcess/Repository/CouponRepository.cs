using BanSach.DataAcess.Data;
using BanSach.DataAcess.Repository.IRepository;
using BanSach.Model;

namespace BanSach.DataAcess.Repository
{
    public class CouponRepository : Repository<Coupon>, ICouponRepository
    {
        //tạo 1 biến 
        private readonly ApplicationDbContext _db;


        //hàm khởi tạo
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
