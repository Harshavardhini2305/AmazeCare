using AmazeCare.MVC.Helpers;
using AmazeCare.MVC.Models;
using AmazeCare.MVC.Services;
using Microsoft.AspNetCore.Mvc;



namespace AmazeCare.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApiService _apiService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(ApiService apiService,
            ILogger<AccountController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        // ── GET: /Account/Login ───────────────────────────────
        [HttpGet]
        public IActionResult Login()
        {
            // If already logged in, redirect to correct dashboard
            if (SessionHelper.IsLoggedIn(HttpContext.Session))
                return RedirectToDashboard();

            return View(new LoginViewModel());
        }

        // ── POST: /Account/Login ──────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _logger.LogInformation("Login attempt: {Email} as {Role}",
                model.Email, model.Role);

            var (success, data, message) = await _apiService.LoginAsync(
                model.Email, model.Password, model.Role);

            if (!success || data == null)
            {
                ViewBag.Error = message;
                return View(model);
            }

            // Save to session
            SessionHelper.SetUserSession(
                HttpContext.Session,
                data.Token,
                data.Role,
                data.UserId,
                data.FullName);

            _logger.LogInformation("Login success: {FullName} as {Role}",
                data.FullName, data.Role);

            TempData["Success"] = $"Welcome back, {data.FullName}!";
            return RedirectToDashboard();
        }

        // ── GET: /Account/Register ────────────────────────────
        [HttpGet]
        public IActionResult Register()
        {
            if (SessionHelper.IsLoggedIn(HttpContext.Session))
                return RedirectToDashboard();

            return View(new RegisterViewModel());
        }

        // ── POST: /Account/Register ───────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _logger.LogInformation("Registration attempt: {Email}", model.Email);

            var (success, data, message) = await _apiService
                .RegisterPatientAsync(model);

            if (!success || data == null)
            {
                ViewBag.Error = message;
                return View(model);
            }

            // Save to session after registration
            SessionHelper.SetUserSession(
                HttpContext.Session,
                data.Token,
                data.Role,
                data.UserId,
                data.FullName);

            _logger.LogInformation("Registration success: {FullName}",
                data.FullName);

            TempData["Success"] = $"Welcome to AmazeCare, {data.FullName}!";
            return RedirectToAction("Dashboard", "Patient");
        }

        // ── GET: /Account/Logout ──────────────────────────────
        public IActionResult Logout()
        {
            var userName = SessionHelper.GetUserName(HttpContext.Session);
            SessionHelper.ClearSession(HttpContext.Session);

            _logger.LogInformation("Logout: {UserName}", userName);

            TempData["Success"] = "You have been logged out successfully.";
            return RedirectToAction("Index", "Home");
        }

        // ── Helper: Redirect based on role ───────────────────
        private IActionResult RedirectToDashboard()
        {
            var role = SessionHelper.GetRole(HttpContext.Session);
            return role switch
            {
                "Patient" => RedirectToAction("Dashboard", "Patient"),
                "Doctor" => RedirectToAction("Dashboard", "Doctor"),
                "Admin" => RedirectToAction("Dashboard", "Admin"),
                _ => RedirectToAction("Index", "Home")
            };
        }
    }
}

