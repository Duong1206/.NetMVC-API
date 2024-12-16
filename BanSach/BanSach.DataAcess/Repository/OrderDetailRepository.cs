using BanSach.DataAcess.Data;
using BanSach.DataAcess.Repository.IRepository;
using BanSach.Model;
using BanSach.Utility;

namespace BanSach.DataAcess.Repository
{
    public class OrderDetailRepository: Repository<OrderDetail>, IOrderDetailRepository
    {
        //tạo 1 biến 
        private readonly ApplicationDbContext _db;
    

        //hàm khởi tạo
        public OrderDetailRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        
        }
    /*    public void Save()
        {
            _db.SaveChanges();
        }*/

        public void Update(OrderDetail obj)
        {
            _db.OrderDetails?.Update(obj);
        }
        public int GetSoldCountForProduct(int productId)
        {
            return _db.OrderDetails
        .Where(od => od.ProductId == productId &&
                     (od.OrderHeader.OrderStatus == SD.StatusShipped ||
                      od.OrderHeader.OrderStatus == SD.StatusInProcess ||
                      od.OrderHeader.OrderStatus == SD.StatisPending ||
                      od.OrderHeader.OrderStatus == SD.StatusApproved))
        .Sum(od => od.Count);
        }

    }
}
