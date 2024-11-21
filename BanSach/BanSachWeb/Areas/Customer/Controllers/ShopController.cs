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
            return View("Index", new ProductListViewModel
            {
                Products = _context.Products.Where(p => p.Name.Contains(keyword)).Skip((productPage - 1) * PageSize).Take(PageSize),
                pagingInfo = new PagingInfo
                {
                    ItemsPerPage = PageSize,
                    CurrentPage = productPage,
                    TotalItems = _context.Products.Count()
                }
            });
        }

        public IActionResult ProductCategory(int categoryId, int productPage = 1)
        {
            var products = _context.Products.Where(p => p.CategoryId == categoryId);

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

        [HttpGet]
        public IActionResult FilterProducts(string[] categories)
        {
            // Validate input
            if (categories == null || !categories.Any())
            {
                return PartialView("_ProductList", new List<Product>()); // Return an empty list if no categories are selected
            }

            // Logic to retrieve products based on selected categories
            var products = GetProductsByCategories(categories);

            // Return PartialView with the filtered product list
            return PartialView("_ProductList", products);
        }

        private List<Product> GetProductsByCategories(string[] categories)
        {
            // Filter products directly in the database based on selected categories
            var products = _context.Products
                .Where(p => categories.Contains(p.CategoryId.ToString()))
                .ToList(); // Execute the query and retrieve the filtered products

            return products;
        }

    }
}
