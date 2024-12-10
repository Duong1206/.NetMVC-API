namespace BanSach.Model.ViewModel
{
    public class UserListViewModel
    {
        public IEnumerable<UserViewModel> Users { get; set; } = Enumerable.Empty<UserViewModel>();
        public PagingInfo PagingInfo { get; set; } = new PagingInfo();
    }

}
