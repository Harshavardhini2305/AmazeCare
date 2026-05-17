// ─────────────────────────────────────────────────────────────
// AmazeCare.MVC/Controllers/HomeController.cs
// ─────────────────────────────────────────────────────────────

using AmazeCare.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace AmazeCare.MVC.Controllers
{
    public class HomeController : Controller
    {
        // GET: /
        public IActionResult Index()
        {
            // If logged in redirect to dashboard
            if (SessionHelper.IsLoggedIn(HttpContext.Session))
            {
                var role = SessionHelper.GetRole(HttpContext.Session);
                return role switch
                {
                    "Patient" => RedirectToAction("Dashboard", "Patient"),
                    "Doctor" => RedirectToAction("Dashboard", "Doctor"),
                    "Admin" => RedirectToAction("Dashboard", "Admin"),
                    _ => View()
                };
            }
            return View();
        }

        // Error page
        public IActionResult Error()
        {
            return View();
        }
    }
}
