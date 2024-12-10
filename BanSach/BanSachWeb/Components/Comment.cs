using BanSach.DataAcess.Data;
using Microsoft.AspNetCore.Mvc;

namespace BanSachWeb.Components
{
    public class Comment : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        public Comment(ApplicationDbContext context)
        {
            _context = context;
        }
        public IViewComponentResult Invoke()
        {
            return View("Index", _context.Reviews?.ToList());
        }
    }
}
