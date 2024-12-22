using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using BanSach.Model.ViewModel;
using BanSach.DataAcess.Data;

namespace BanSachWeb.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public UserController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> userRole, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = userRole;
            _context = context;
        }

        public IActionResult Index(int userPage = 1)
        {
            int pageSize = 15;
            int totalUsers = _userManager.Users.Count();

            var users = (from u in _userManager.Users
                         join a in _context.ApplicationUsers on u.Id equals a.Id
                         select new UserViewModel
                         {
                             Id = u.Id,
                             Name = a.Name,
                             Email = u.Email,
                             StreetAddress = a.StreetAddress,
                             City = a.City,
                             PhoneNumber = u.PhoneNumber
                         })
                         .Skip((userPage - 1) * pageSize)
                         .Take(pageSize)
                         .ToList();

            foreach (var user in users)
            {
                var userRoles = _userManager.GetRolesAsync(_userManager.FindByIdAsync(user.Id).Result).Result;
                user.Role = string.Join(", ", userRoles);
            }

            var viewModel = new UserListViewModel
            {
                Users = users,
                PagingInfo = new PagingInfo
                {
                    CurrentPage = userPage,
                    ItemsPerPage = pageSize,
                    TotalItems = totalUsers
                }
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = _context.ApplicationUsers.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            var userViewModel = new UserViewModel
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                StreetAddress = user.StreetAddress,
                City = user.City,
                Role = string.Join(", ", _userManager.GetRolesAsync(user).Result)
            };

            ViewBag.Roles = new List<string> { "Employee", "Individual" };

            return View(userViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(UserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = new List<string> { "Employee", "Individual" };
                return View(model);
            }

            var user = _context.ApplicationUsers.Find(model.Id);
            if (user == null)
            {
                return NotFound();
            }

            // Update user role
            var currentRoles = _userManager.GetRolesAsync(user).Result;
            _userManager.RemoveFromRolesAsync(user, currentRoles).Wait();
            _userManager.AddToRoleAsync(user, model.Role).Wait();

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpDelete]
        public IActionResult Delete(string id)
        {
            var user = _userManager.FindByIdAsync(id).Result;
            if (user == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            var result = _userManager.DeleteAsync(user).Result;
            if (result.Succeeded)
            {
                return Json(new { success = true, message = "User deleted successfully." });
            }
            else
            {
                return Json(new { success = false, message = "Error occurred while deleting the user." });
            }
        }
    }
}
