namespace BanSach.Model.ViewModel
{
    public class OrderListViewModel
    {
        public IEnumerable<OrderVM> Orders { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
