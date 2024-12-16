using BanSach.DataAcess.Data;
using BanSach.DataAcess.Repository.IRepository;
using BanSach.Model;
using BanSach.Model.ViewModel;
using DryIoc.ImTools;
using Microsoft.AspNetCore.Mvc;



namespace BanSachWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ShopController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        public int PageSize = 12;
        public ShopController(ApplicationDbContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index(int productPage = 1)
        {
            var availableProducts = _context.Products.ToList()
                .GroupBy(p => p.Name)
                .Select(g => g.FirstOrDefault(p => p.Quantity > 0) ?? g.First())
                .ToList();

            foreach (var product in availableProducts)
            {
                product.SoldCount = _unitOfWork.OrderDetail.GetSoldCountForProduct(product.Id);
            }


            var productsOnPage = availableProducts
                .Skip((productPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            return View(new ProductListViewModel
            {
                Products = productsOnPage,
                pagingInfo = new PagingInfo
                {
                    ItemsPerPage = PageSize,
                    CurrentPage = productPage,
                    TotalItems = availableProducts.Count
                }
            });
        }

        [HttpPost]
        public IActionResult Search(string keyword, int productPage = 1)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return RedirectToAction("Index");
            }

            keyword = RemoveVietnameseTone(string.Join(" ", keyword.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)));

            var productsQuery = _context.Products
                .AsEnumerable() // Chuyển đổi dữ liệu sang client-side
                .Where(p => RemoveVietnameseTone(p.Name.ToLower()).Contains(keyword.ToLower()));

            return View("Index", new ProductListViewModel
            {
                Products = productsQuery.Skip((productPage - 1) * PageSize).Take(PageSize),
                pagingInfo = new PagingInfo
                {
                    ItemsPerPage = PageSize,
                    CurrentPage = productPage,
                    TotalItems = productsQuery.Count()
                }
            });
        }

        private string RemoveVietnameseTone(string text)
        {
            string[] VietnameseSigns = new string[]
            {
        "aAeEoOuUiIdDyY",
        "áàạảãâấầậẩẫăắằặẳẵ",
        "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
        "éèẹẻẽêếềệểễ",
        "ÉÈẸẺẼÊẾỀỆỂỄ",
        "óòọỏõôốồộổỗơớờợởỡ",
        "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
        "úùụủũưứừựửữ",
        "ÚÙỤỦŨƯỨỪỰỬỮ",
        "íìịỉĩ",
        "ÍÌỊỈĨ",
        "đ",
        "Đ",
        "ýỳỵỷỹ",
        "ÝỲỴỶỸ"
            };

            for (int i = 1; i < VietnameseSigns.Length; i++)
            {
                for (int j = 0; j < VietnameseSigns[i].Length; j++)
                {
                    text = text.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);
                }
            }
            return text;
        }





        public IActionResult ProductCategory(int categoryId, int productPage = 1)
        {
            var products = _context.Products?
                .Where(p => p.CategoryId == categoryId && p.Quantity > 0);

            return View("Index", new ProductListViewModel
            {
                Products = products.Skip((productPage - 1) * PageSize).Take(PageSize),
                pagingInfo = new PagingInfo
                {
                    ItemsPerPage = PageSize,
                    CurrentPage = productPage,
                    TotalItems = products.Count()
                }
            });
        }


        private List<Product> GetProductsByCategories(string[] categories)
        {
            var products = _context.Products?
                .Where(p => categories.Contains(p.CategoryId.ToString()))
                .ToList(); 

            return products;
        }



        [HttpPost]
        public IActionResult FilterProducts([FromBody] FilterRequest request)
        {

            var filteredProducts = _context.Products
                .Where(p => (request.Categories.Contains("all") || request.Categories.Contains(p.CategoryId.ToString())) && p.Quantity > 0)
                .GroupBy(p => p.Name)
                .Select(g => g.First()) 
                .ToList();

            return Json(filteredProducts);
        }



        public class FilterRequest
        {
            public List<string> Categories { get; set; }
        }

    }
}
