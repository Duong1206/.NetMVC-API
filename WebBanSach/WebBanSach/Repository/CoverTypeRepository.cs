using Microsoft.EntityFrameworkCore;
using WebBanSach.Data;
using WebBanSach.Models;
using WebBanSach.Repository.IRepository;

namespace WebBanSach.Repository
{
    public class CoverTypeRepository : Repository<CoverType>, ICoverTypeRepository
    {
        private readonly ApplicationDbContext _db;

        public CoverTypeRepository(ApplicationDbContext db) : base(db) 
        {
            _db = db;
            
        }
        public void Save()
        {
            _db.SaveChanges();
        }

        public void Update(CoverType coverType)
        {
            _db.CoverTypes.Update(coverType);
        }
    }
}
