using BanSach.Model;

namespace BanSach.DataAcess.Repository.IRepository
{
    public interface IContactRepository : IRepository<Contact>
    {
        void Update(Contact contact);

    }
}
