using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BanSach.DataAcess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository Category { get; }
        ICoverTypeRepository coverType { get; }
        IProductRepository Product { get; }
        ICompanyRepository Company { get; }
        void Save();
    }
}
