﻿using BanSach.DataAcess.Data;
using BanSach.DataAcess.Repository.IRepository;
using BanSach.Model;

namespace BanSach.DataAcess.Repository
{
    public class CoverTypeRepository : Repository<CoverType>, ICoverTypeRepository
    {
        //tạo 1 biến 
        private readonly ApplicationDbContext _db;
    

        //hàm khởi tạo
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
