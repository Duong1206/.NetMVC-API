using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BanSachWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult LoginByGoogle(string returnUrl = null)
        {
            var redirectUrl = Url.Action("GoogleResponse", "Account", new { returnUrl });
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        // X? l? ph?n h?i t? Google
        [HttpGet]
        public async Task<IActionResult> GoogleResponse(string returnUrl = null)
        {
            var result = await HttpContext.AuthenticateAsync();
            if (result?.Principal != null)
            {
                // Thêm thông tin ngý?i dùng vào Claims
                var claims = result.Principal.Claims.Select(claim => new Claim(claim.Type, claim.Value)).ToList();
                var identity = new ClaimsIdentity(claims, "GoogleKeys");
                var claimsPrincipal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(claimsPrincipal);
                return LocalRedirect(returnUrl ?? "/Customer/Home"); 
            }

            return RedirectToAction("LoginByGoogle");
        }
    }
}
