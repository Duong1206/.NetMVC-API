using BanSach.DataAcess.Data;
using BanSach.DataAcess.Repository.IRepository;
using BanSach.Model;
using BanSach.Model.ViewModel;
using DryIoc.ImTools;
using Microsoft.AspNetCore.Mvc;
using System.Linq;


namespace BanSachWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ShopController : Controller
    {
        private readonly ApplicationDbContext _context;
        public int PageSize = 12;
        public ShopController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int productPage = 1)
        {
            return View(new ProductListViewModel
            {
                Products = _context.Products.Skip((productPage - 1) * PageSize).Take(PageSize),
                pagingInfo = new PagingInfo
                {
                    ItemsPerPage = PageSize,
                    CurrentPage = productPage,
                    TotalItems = _context.Products.Count()
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

            keyword = string.Join(" ", keyword.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));

            var productsQuery = _context.Products
                ?.Where(p => p.Name.ToLower().Contains(keyword.ToLower()));

            return View("Index", new ProductListViewModel
            {
                Products = productsQuery?.Skip((productPage - 1) * PageSize).Take(PageSize),
                pagingInfo = new PagingInfo
                {
                    ItemsPerPage = PageSize,
                    CurrentPage = productPage,
                    TotalItems = productsQuery?.Count() ?? 0
                }
            });
        }


        public IActionResult ProductCategory(int categoryId, int productPage = 1)
        {
            var products = _context.Products?.Where(p => p.CategoryId == categoryId);

            return View("Index", new ProductListViewModel
            {
                Products = products.Skip((productPage - 1) * PageSize).Take(PageSize),
                pagingInfo = new PagingInfo
                {
                    ItemsPerPage = PageSize,
                    CurrentPage = productPage,
                    TotalItems = products.Count() // Tính tổng số sản phẩm trong Category
                }
            });
        }

        private List<Product> GetProductsByCategories(string[] categories)
        {
            // Filter products directly in the database based on selected categories
            var products = _context.Products?
                .Where(p => categories.Contains(p.CategoryId.ToString()))
                .ToList(); // Execute the query and retrieve the filtered products

            return products;
        }


        //filter category
        [HttpPost]
        public IActionResult FilterProducts([FromBody] FilterRequest request)
        {
            var filteredProducts = _context.Products
                .Where(p => request.Categories.Contains("all") || request.Categories.Contains(p.CategoryId.ToString()))
                .ToList();

            return Json(filteredProducts);
        }

        public class FilterRequest
        {
            public List<string> Categories { get; set; }
        }

    }
}
