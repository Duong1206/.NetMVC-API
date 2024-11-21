using BanSach.DataAcess.Data;
using Microsoft.AspNetCore.Mvc;

namespace BanSachWeb.Components
{
    public class Filter : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        public Filter(ApplicationDbContext context)
        {
            _context = context;
        }
        public IViewComponentResult Invoke()
        {
            return View("Index",_context.Categories?.ToList());
        }
    }
}
