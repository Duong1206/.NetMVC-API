using BanSach.DataAcess.Data;
using BanSach.DataAcess.Repository.IRepository;
using BanSach.Model;
using BanSach.Model.Dtos.Category;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BanSach.DataAcess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {

        //tạo 1 biến 
        private readonly ApplicationDbContext _db;
        internal DbSet<T> DbSet;

        //hàm khởi tạo
        public Repository(ApplicationDbContext db)
        {
            _db = db;
            this.DbSet = _db.Set<T>();
        }

        

        public void Add(T entity)
        { 
          DbSet.Add(entity);
        }

        public async Task<T> CreateAsync(T entity)
        {
            await _db.Set<T>().AddAsync(entity);
            await _db.SaveChangesAsync();
            return entity;
        }

        public async Task<T?> DeleteAsync(int id)
        {
            var entity = await _db.Set<T>().FindAsync(id);
            if (entity == null)
            {
                // Nếu không tìm thấy thực thể, trả về null
                return null;
            }

            // Xóa thực thể
            _db.Set<T>().Remove(entity);

            // Lưu thay đổi vào cơ sở dữ liệu
            await _db.SaveChangesAsync();

            // Trả về thực thể đã xóa
            return entity;
        }

        // include category, covertype



        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null)
        {
            IQueryable<T> query = DbSet;
            if(filter != null)
            {
                query = query.Where(filter);

            }
            if(includeProperties != null)
            {
                foreach(var item in includeProperties.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(item);
                }
            }
            return query.ToList();
        }

        public Task<List<T>> GetAllAsync()
        {
            return _db.Set<T>().ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            var entity = await _db.Set<T>().FindAsync(id);
            return entity;
        }

        public T GetFirstOrDefault(Expression<Func<T, bool>> filter, string? includeProperties = null)
        {
            IQueryable<T> query = DbSet;
            if (includeProperties != null)
            {
                foreach (var item in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(item);
                }
            }
            query = query.Where(filter);
            return query.FirstOrDefault();
        }

        public void Remove(T entity)
        {
           DbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entity)
        {
           DbSet.RemoveRange(entity);
        }

        /*public Task<T?> UpdateAsync(int id, UpdateCategoryRequestDto entityDto)
        {
            throw new NotImplementedException();
        }*/
    }
}
