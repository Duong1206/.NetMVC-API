using WebBanSach.Data;
using WebBanSach.Repository.IRepository;

namespace WebBanSach.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        public ICategoryRepository Category { get; private set; }

        public ICoverTypeRepository covertype { get; private set; }
        public IProductRepository product { get; private set; }

        private readonly ApplicationDbContext _db;

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Category = new CategoryRepository(_db);
            covertype = new CoverTypeRepository(_db);
            product = new ProductRepository(_db);
        }
        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
