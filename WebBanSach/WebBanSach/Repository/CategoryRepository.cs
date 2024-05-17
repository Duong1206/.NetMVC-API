using Microsoft.EntityFrameworkCore;
using WebBanSach.Data;
using WebBanSach.Models;
using WebBanSach.Repository.IRepository;

namespace WebBanSach.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _db;

        public CategoryRepository(ApplicationDbContext db) : base(db) 
        {
            _db = db;
            
        }
        public void Save()
        {
            _db.SaveChanges();
        }

        public void Update(Category category)
        {
            _db.Categories.Update(category);
        }
    }
}
