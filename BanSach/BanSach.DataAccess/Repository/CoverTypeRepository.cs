using BanSach.Persistence.Context;
using BanSach.DataAccess.Repository.IRepository;
using BanSach.Model;

namespace BanSach.DataAccess.Repository
{
    public class CoverTypeRepository : Repository<CoverType>, ICoverTypeRepository
    {
        //t?o 1 bi?n 
        private readonly ApplicationDbContext _db;
    

        //h�m kh?i t?o
        public CoverTypeRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        
        }
      /*   public void Save()
        {
            _db.SaveChanges();
        }*/

        public void Update(CoverType coverType)
        {
           _db.CoverTypes?.Update(coverType);
        }
    }
}
