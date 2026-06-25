using BanSach.Model;

namespace BanSach.DataAccess.Repository.IRepository
{
    public interface IContactRepository : IRepository<Contact>
    {
        void Update(Contact contact);

    }
}
