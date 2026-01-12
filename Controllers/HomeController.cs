using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using CMCS.Models;

/*
    References used in this controller:
    - Microsoft (2024) Routing in ASP.NET Core. https://learn.microsoft.com/en-us/aspnet/core/fundamentals/routing
    - Microsoft (2024) Dependency injection in ASP.NET Core. https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection
    - Microsoft (2024) TempData in ASP.NET Core. (used for messages)
    - Microsoft (2024) Session state in ASP.NET Core. (used for role simulation)
*/

namespace CMCS.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home/Index - Main landing page with role selection
        // Reference: Microsoft (2024) Routing in ASP.NET Core
        public IActionResult Index()
        {
            return View();
        }

        // POST: Home/Login - Sets session role for demo authentication
        // Reference: Microsoft (2024) Session state in ASP.NET Core
        [HttpPost]
        public IActionResult Login(string role)
        {
            if (!string.IsNullOrEmpty(role))
            {
                HttpContext.Session.SetString("UserRole", role);
                TempData["Success"] = $"Logged in as {role}";
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Home/Logout - Clears session
        // Reference: Microsoft (2024) Session state in ASP.NET Core
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["Success"] = "Logged out successfully";
            return RedirectToAction(nameof(Index));
        }

        // GET: Home/Error - Error handling page
        // Reference: Microsoft (2024) Handle errors in ASP.NET Core
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}