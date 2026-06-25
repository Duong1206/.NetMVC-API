using BanSach.Persistence.Context;
using BanSach.DataAccess.Repository.IRepository;
using BanSach.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BanSach.DataAccess.Repository
{
    public class ContactRepository : Repository<Contact>, IContactRepository
    {
        //t?o 1 bi?n 
        private readonly ApplicationDbContext _db;


        //h�m kh?i t?o
        public ContactRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;

        }
        /*    public void Save()
            {
                _db.SaveChanges();
            }*/

        public void Update(Contact contact)
        {
            _db.Contacts?.Update(contact);
        }
    }
}
