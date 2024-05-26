using System.Linq.Expressions;

namespace WebBanSach.Repository
{
    public interface IRepository<T> where T : class
    {
        T GetFirstOrDefault(Expression<Func<T, bool>> filer,string? includeProperties=null);
        IEnumerable<T> GetAll(string? includeProperties = null);
        void Add(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);

    }
}
