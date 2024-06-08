using BanSach.DataAcess.Data;
using Microsoft.AspNetCore.Mvc;

namespace BanSachWeb.Components
{
    public class Book : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        public Book(ApplicationDbContext context)
        {
            _context = context;
        }
        public IViewComponentResult Invoke()
        {
            return View(_context.Products.ToList());
        }
    }
}
