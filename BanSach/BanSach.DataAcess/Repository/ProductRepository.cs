using BanSach.DataAcess.Data;
using BanSach.DataAcess.Repository.IRepository;
using BanSach.Model;

namespace BanSach.DataAcess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _db;

        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Product product)
        {
            var obj = _db.Products?.SingleOrDefault(u => u.Id == product.Id);
            if (obj != null)
            {
                obj.Name = product.Name;
                obj.ISBN = product.ISBN;
                obj.Description = product.Description;
                obj.Price50 = product.Price50;
                obj.Price100 = product.Price100;
                obj.Author = product.Author;
                obj.Quantity = product.Quantity;
                obj.CoverTypeId = product.CoverTypeId;
                obj.CategoryId = product.CategoryId;

                if (!string.IsNullOrEmpty(product.ImageUrl))
                {
                    obj.ImageUrl = product.ImageUrl;
                }
            }
            _db.Products?.Update(product);
        }
    }
}
