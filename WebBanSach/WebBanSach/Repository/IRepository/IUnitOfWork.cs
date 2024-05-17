namespace WebBanSach.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository Category { get; }
        ICoverTypeRepository covertype { get; }

        void Save();
    }
}
