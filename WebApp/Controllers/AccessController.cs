using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class AccessController : Controller
    {
        public IActionResult Login()
        {
            ClaimsPrincipal claimUser = HttpContext.User;

            if (claimUser.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "AuctionItem");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(VMLogin model)
        {
            if (model.Username == "admin1" || model.Username == "admin2")
            {
                model.Role = "Admin";
            }
            else if (model.Username == "user1" || model.Username == "user2")
            {
                model.Role = "Regular";
            }
            else
            {
                TempData["errorMessage"] = "User not found";
                return View();
            }

            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, model.Username),
                new Claim(ClaimTypes.Role, model.Role)
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true,
                //IsPersistent = model.RememberMe,
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), properties);

            return RedirectToAction("Index", "AuctionItem");
        }
    }
}
