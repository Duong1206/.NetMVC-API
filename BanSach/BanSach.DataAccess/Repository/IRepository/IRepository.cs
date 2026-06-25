using BanSach.Model;
using BanSach.Model.Dtos.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BanSach.DataAccess.Repository.IRepository
{
    // khai b�o c�c method d�ng chung
    public interface IRepository<T> where T : class
    {
        // l?y category d?a v�o Id (ph?n t? ID duy nh?t)
        T GetFirstOrDefault(Expression<Func<T, bool>> filter, string? includeProperties = null);
        Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter, string? includeProperties = null);
        
        // l?y t?t c?
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter=null, string? includeProperties = null);
        Task<IEnumerable<T>> GetAllWithFilterAsync(Expression<Func<T, bool>>? filter=null, string? includeProperties = null);
        
        // add & remove
        void Add(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entity);
        
        // async operations
        Task<List<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task<T> CreateAsync(T entity);
        Task<T?> DeleteAsync(int id);
    }
}
