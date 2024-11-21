using BanSach.DataAcess.Data;
using BanSach.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BanSach.DataAcess
{
    public class DataSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public DataSeeder(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeedAsync()
        {
            if (await _context.Users.AnyAsync())
            {
                return; // Database has been seeded
            }

            // Seed roles
            const string roleName = "Admin";
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }

            // Seed users
            var user = new IdentityUser { UserName = "admin@bansach.com", Email = "admin@bansach.com" };
            var result = await _userManager.CreateAsync(user, "Admin@123");

            if (result.Succeeded)
            {
                // Assign the user to the Admin role
                await _userManager.AddToRoleAsync(user, roleName);
            }

            // Add more seeding logic for other entities as needed

            await _context.SaveChangesAsync();
        }
    }
}
