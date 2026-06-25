using BanSach.Persistence.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IActionResult> Index()
        {
            var today = DateTime.Now.Date;
            var currentMonth = new DateTime(today.Year, today.Month, 1);
            var currentYear = new DateTime(today.Year, 1, 1);
            var last30Days = today.AddDays(-30);
            var last12Months = today.AddMonths(-12);

            // Revenue Statistics
            var revenueToday = _dbContext.OrderHeaders?
                .Where(o => o.OrderDate.Date == today && o.PaymentStatus == "Đã thanh toán")
                .Sum(o => (decimal?)o.OrderTotal) ?? 0;

            var ordersToday = _dbContext.OrderHeaders?
                .Where(o => o.OrderDate.Date == today)
                .Count();

            var revenueMonth = _dbContext.OrderHeaders?
                .Where(o => o.OrderDate >= currentMonth && o.PaymentStatus == "Đã thanh toán")
                .Sum(o => (decimal?)o.OrderTotal) ?? 0;

            var ordersMonth = _dbContext.OrderHeaders?
                .Where(o => o.OrderDate >= currentMonth)
                .Count();

            var revenueYear = _dbContext.OrderHeaders?
                .Where(o => o.OrderDate >= currentYear && o.PaymentStatus == "Đã thanh toán")
                .Sum(o => (decimal?)o.OrderTotal) ?? 0;

            var ordersYear = _dbContext.OrderHeaders?
                .Where(o => o.OrderDate >= currentYear)
                .Count();

            // Total counts
            var totalProducts = _dbContext.Products.Count();
            var totalReviews = _dbContext.Reviews.Count();
            var totalUsers = _dbContext.Users.Count();
            var totalOrders = _dbContext.OrderHeaders.Count();

            // Order status breakdown
            var orderStatusBreakdown = _dbContext.OrderHeaders?
                .GroupBy(o => o.OrderStatus)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToList();

            // Payment status breakdown
            var paymentStatusBreakdown = _dbContext.OrderHeaders?
                .GroupBy(o => o.PaymentStatus)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToList();

            // Top 5 products by sales
            var topProducts = await _dbContext.Products
                .AsNoTracking()
                .OrderByDescending(p => p.SoldCount)
                .Take(5)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.SoldCount,
                    p.Price50
                })
                .ToListAsync();

            // Recent orders
            var recentOrders = await _dbContext.OrderHeaders
                .AsNoTracking()
                .Include(o => o.ApplicationUser)
                .OrderByDescending(o => o.OrderDate)
                .Take(10)
                .Select(o => new
                {
                    o.Id,
                    o.OrderDate,
                    CustomerName = o.ApplicationUser.Name,
                    o.OrderTotal,
                    o.OrderStatus,
                    o.PaymentStatus
                })
                .ToListAsync();

            // New customers (last 30 days)
            var newCustomers = await _dbContext.ApplicationUsers
                .AsNoTracking()
                .Where(u => u.Id != null)
                .OrderByDescending(u => u.Id)
                .Take(5)
                .Select(u => new
                {
                    u.Name,
                    u.Email,
                    u.PhoneNumber
                })
                .ToListAsync();

            // Revenue by category
            var revenueByCategory = await _dbContext.OrderDetails
                .AsNoTracking()
                .Include(od => od.Product)
                .ThenInclude(p => p.Category)
                .GroupBy(od => od.Product.Category.Name)
                .Select(g => new
                {
                    Category = g.Key,
                    Revenue = g.Sum(od => od.Price * od.Count),
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Revenue)
                .Take(10)
                .ToListAsync();

            // Daily revenue for last 30 days
            var dailyRevenue = await _dbContext.OrderHeaders
                .AsNoTracking()
                .Where(o => o.OrderDate >= last30Days && o.PaymentStatus == "Đã thanh toán")
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Revenue = g.Sum(o => o.OrderTotal)
                })
                .OrderBy(x => x.Date)
                .ToListAsync();

            // Monthly revenue for last 12 months
            var monthlyRevenue = await _dbContext.OrderHeaders
                .AsNoTracking()
                .Where(o => o.OrderDate >= last12Months && o.PaymentStatus == "Đã thanh toán")
                .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                .Select(g => new
                {
                    YearMonth = $"{g.Key.Year}-{g.Key.Month:D2}",
                    Revenue = g.Sum(o => o.OrderTotal)
                })
                .OrderBy(x => x.YearMonth)
                .ToListAsync();

            // Passing data to the view
            ViewData["RevenueToday"] = revenueToday.ToString("N0");
            ViewData["OrdersToday"] = ordersToday;
            ViewData["RevenueMonth"] = revenueMonth.ToString("N0");
            ViewData["OrdersMonth"] = ordersMonth;
            ViewData["RevenueYear"] = revenueYear.ToString("N0");
            ViewData["OrdersYear"] = ordersYear;
            ViewData["TotalProducts"] = totalProducts;
            ViewData["TotalReviews"] = totalReviews;
            ViewData["TotalUsers"] = totalUsers;
            ViewData["TotalOrders"] = totalOrders;

            ViewBag.OrderStatusBreakdown = orderStatusBreakdown?.Cast<dynamic>().ToList() ?? new List<dynamic>();
            ViewBag.PaymentStatusBreakdown = paymentStatusBreakdown?.Cast<dynamic>().ToList() ?? new List<dynamic>();
            ViewBag.TopProducts = topProducts?.Cast<dynamic>().ToList() ?? new List<dynamic>();
            ViewBag.RecentOrders = recentOrders?.Cast<dynamic>().ToList() ?? new List<dynamic>();
            ViewBag.NewCustomers = newCustomers?.Cast<dynamic>().ToList() ?? new List<dynamic>();
            ViewBag.RevenueByCategory = revenueByCategory?.Cast<dynamic>().ToList() ?? new List<dynamic>();
            ViewBag.DailyRevenue = dailyRevenue?.Cast<dynamic>().ToList() ?? new List<dynamic>();
            ViewBag.MonthlyRevenue = monthlyRevenue?.Cast<dynamic>().ToList() ?? new List<dynamic>();

            return View();
        }
    }
}
