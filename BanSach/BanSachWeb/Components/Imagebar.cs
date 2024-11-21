using BanSach.DataAcess.Data;
using Microsoft.AspNetCore.Mvc;

namespace BanSachWeb.Components
{
    public class Imagebar : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        public Imagebar(ApplicationDbContext context)
        {
            _context = context;
        }
        public IViewComponentResult Invoke()
        {
            return View("index", _context.Categories?.ToList());
        }
    }
}
