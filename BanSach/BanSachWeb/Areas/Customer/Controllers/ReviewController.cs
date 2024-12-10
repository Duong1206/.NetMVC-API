using BanSach.DataAcess.Data;
using BanSach.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BanSachWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ReviewController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReviewController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var reviews = _context.Reviews?.ToList();
            return View(reviews);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Create(Review review)
        {
            if (ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userName = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;

                if (userId != null)
                {
                    review.ApplicationUserId = userId;
                    review.ApplicationUser = new ApplicationUser { Name = userName };
                    review.CreatedAt = DateTime.Now;

                    _context.Reviews?.Add(review);
                    _context.SaveChanges();

                    TempData["Success"] = "Review added successfully.";
                }
                else
                {
                    TempData["Error"] = "You must be logged in to leave a review.";
                }

                return RedirectToAction("Details", "Home", new { id = review.ProductId });
            }

            TempData["Error"] = "Failed to add review. Please try again.";
            return RedirectToAction("Details", "Home", new { id = review.ProductId });
        }
        [Authorize]
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var review = _context.Reviews?.FirstOrDefault(r => r.Id == id);
            if (review == null)
            {
                TempData["Error"] = "Review not found.";
                return RedirectToAction("Index");
            }

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (review.ApplicationUserId == userId || User.IsInRole("Admin"))
            {
                _context.Reviews?.Remove(review);
                _context.SaveChanges();

                TempData["Success"] = "Review deleted successfully.";
            }
            else
            {
                TempData["Error"] = "You do not have permission to delete this review.";
            }

            return RedirectToAction("Details", "Home", new { id = review.ProductId });
        }
    }
}
