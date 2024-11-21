namespace BanSach.Model.ViewModel
{
    public class ProductListViewModel
    {
        public IEnumerable<Product> Products { get; set; } = Enumerable.Empty<Product>();
        public PagingInfo pagingInfo { get; set; } = new PagingInfo();
    }
}
