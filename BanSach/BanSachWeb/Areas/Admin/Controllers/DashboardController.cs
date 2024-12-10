using BanSach.DataAcess.Data;
using Microsoft.AspNetCore.Mvc;

namespace BanSachWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
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

            var revenueToday = _dbContext.OrderHeaders?
                .Where(o => o.OrderDate.Date == today && o.OrderStatus == "Shipped") 
                .Sum(o => o.OrderTotal);

            var ordersToday = _dbContext.OrderHeaders?
                .Where(o => o.OrderDate.Date == today && o.OrderStatus == "Shipped") 
                .Count();

            var revenueMonth = _dbContext.OrderHeaders?
                .Where(o => o.OrderDate >= currentMonth && o.OrderStatus == "Shipped") 
                .Sum(o => o.OrderTotal);

            var ordersMonth = _dbContext.OrderHeaders?
                .Where(o => o.OrderDate >= currentMonth && o.OrderStatus == "Shipped")  
                .Count();

            var revenueYear = _dbContext.OrderHeaders?
                .Where(o => o.OrderDate >= currentYear && o.OrderStatus == "Shipped") 
                .Sum(o => o.OrderTotal);

            var ordersYear = _dbContext.OrderHeaders?
                .Where(o => o.OrderDate >= currentYear && o.OrderStatus == "Shipped")  
                .Count();

            var dailyOrdersData = _dbContext.OrderHeaders?
                .Where(o => o.OrderDate.Date == today && o.OrderStatus == "Shipped")  
                .GroupBy(o => o.OrderDate.Hour)
                .Select(g => new { Hour = g.Key, OrderCount = g.Count() })
                .OrderBy(g => g.Hour)
                .ToList();

            var monthlyOrdersData = _dbContext.OrderHeaders?
                .Where(o => o.OrderDate.Year == today.Year && o.OrderStatus == "Shipped") 
                .GroupBy(o => o.OrderDate.Month)
                .Select(g => new { Month = g.Key, OrderCount = g.Count() })
                .OrderBy(g => g.Month)
                .ToList();

            ViewData["RevenueToday"] = revenueToday;
            ViewData["OrdersToday"] = ordersToday;
            ViewData["RevenueMonth"] = revenueMonth;
            ViewData["OrdersMonth"] = ordersMonth;
            ViewData["RevenueYear"] = revenueYear;
            ViewData["OrdersYear"] = ordersYear;
            ViewData["DailyOrdersData"] = dailyOrdersData;
            ViewData["MonthlyOrdersData"] = monthlyOrdersData;

            return View();
        }
    }
}
