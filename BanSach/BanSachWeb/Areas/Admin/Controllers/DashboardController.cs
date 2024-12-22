using BanSach.DataAcess.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BanSachWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Employee")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public DashboardController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var today = DateTime.Now.Date;
            var currentMonth = new DateTime(today.Year, today.Month, 1);
            var currentYear = new DateTime(today.Year, 1, 1);

            // Revenue and order count for today
            var revenueToday = _dbContext.OrderHeaders?
                .Where(o => o.OrderDate.Date == today && o.OrderStatus == "Shipped")
                .Sum(o => (decimal?)o.OrderTotal) ?? 0;

            var ordersToday = _dbContext.OrderHeaders?
                .Where(o => o.OrderDate.Date == today && o.OrderStatus == "Shipped")
                .Count();

            // Revenue and order count for this month
            var revenueMonth = _dbContext.OrderHeaders?
                .Where(o => o.OrderDate >= currentMonth && o.OrderStatus == "Shipped")
                .Sum(o => (decimal?)o.OrderTotal) ?? 0;

            var ordersMonth = _dbContext.OrderHeaders?
                .Where(o => o.OrderDate >= currentMonth && o.OrderStatus == "Shipped")
                .Count();

            // Revenue and order count for this year
            var revenueYear = _dbContext.OrderHeaders?
                .Where(o => o.OrderDate >= currentYear && o.OrderStatus == "Shipped")
                .Sum(o => (decimal?)o.OrderTotal) ?? 0;

            var ordersYear = _dbContext.OrderHeaders?
                .Where(o => o.OrderDate >= currentYear && o.OrderStatus == "Shipped")
                .Count();

            // Total Products, Comments, and Users
            var totalProducts = _dbContext.Products.Count();
            var totalComments = _dbContext.Reviews.Count();
            var totalUsers = _dbContext.Users.Count();

            // Passing data to the view
            ViewData["RevenueToday"] = revenueToday;
            ViewData["OrdersToday"] = ordersToday;
            ViewData["RevenueMonth"] = revenueMonth;
            ViewData["OrdersMonth"] = ordersMonth;
            ViewData["RevenueYear"] = revenueYear;
            ViewData["OrdersYear"] = ordersYear;
            ViewData["TotalProducts"] = totalProducts;
            ViewData["TotalComments"] = totalComments;
            ViewData["TotalUsers"] = totalUsers;

            return View();
        }
    }
}
