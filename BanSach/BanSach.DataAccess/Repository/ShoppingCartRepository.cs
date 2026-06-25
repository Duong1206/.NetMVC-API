using BanSach.Persistence.Context;
using BanSach.DataAccess.Repository.IRepository;
using BanSach.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BanSach.DataAccess.Repository
{
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {
        //t?o 1 bi?n 
        private readonly ApplicationDbContext _db;
    

        //h�m kh?i t?o
        public ShoppingCartRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        
        }
        int IShoppingCartRepository.DecrementCount(BanSach.Model.ShoppingCart shoppingCart, int count)
        {
            shoppingCart.Count -= count;
            return shoppingCart.Count;
        }
        int IShoppingCartRepository.IncrementCount(BanSach.Model.ShoppingCart shoppingCart, int count)
        {
            shoppingCart.Count += count;
            return shoppingCart.Count;
        }

      
    }
}
