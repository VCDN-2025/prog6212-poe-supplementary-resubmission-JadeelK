using Microsoft.AspNetCore.Mvc;
using CMCS.Models;
using CMCS.Services;

/*
    References used in this controller:
    - Microsoft (2024) Routing in ASP.NET Core. https://learn.microsoft.com/en-us/aspnet/core/fundamentals/routing
    - Microsoft (2024) Dependency injection in ASP.NET Core. https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection
    - Microsoft (2024) Model validation in ASP.NET Core MVC. https://learn.microsoft.com/en-us/aspnet/core/mvc/models/validation
    - Microsoft (2024) TempData in ASP.NET Core. (used for messages)
    - Microsoft (2024) LINQ (Language Integrated Query). https://learn.microsoft.com/en-us/dotnet/csharp/linq/ (used for reporting queries)
*/

namespace CMCS.Controllers
{
    public class HRController : Controller
    {
        private readonly InMemoryDataService _dataService;

        public HRController(InMemoryDataService dataService)
        {
            _dataService = dataService;
        }

        // GET: HR/Index - HR dashboard with claim statistics
        // Reference: Microsoft (2024) LINQ (Language Integrated Query)
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserRole") != "HR")
                return Unauthorized();

            var allClaims = _dataService.GetAllClaims();
            ViewBag.PendingClaims = allClaims.Count(c => c.Status == "Submitted" || c.Status == "Verified");
            ViewBag.ApprovedClaims = allClaims.Count(c => c.Status == "Approved");
            ViewBag.TotalPayout = allClaims.Where(c => c.Status == "Approved").Sum(c => c.TotalAmount);
            ViewBag.AllClaims = allClaims;

            return View();
        }

        // GET: HR/ManageUsers - List all lecturers
        // Reference: Microsoft (2024) Routing in ASP.NET Core
        public IActionResult ManageUsers()
        {
            if (HttpContext.Session.GetString("UserRole") != "HR")
                return Unauthorized();

            var lecturers = _dataService.GetAllLecturers();
            return View(lecturers);
        }

        // GET: HR/CreateUser - Form to add new lecturer
        // Reference: Microsoft (2024) Model validation in ASP.NET Core MVC
        public IActionResult CreateUser()
        {
            if (HttpContext.Session.GetString("UserRole") != "HR")
                return Unauthorized();

            ViewBag.Contracts = _dataService.GetAllContracts();
            return View();
        }

        // POST: HR/CreateUser - Add new lecturer with validation
        // Reference: Microsoft (2024) Model validation in ASP.NET Core MVC
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateUser(Lecturer lecturer)
        {
            if (HttpContext.Session.GetString("UserRole") != "HR")
                return Unauthorized();

            if (ModelState.IsValid)
            {
                _dataService.AddLecturer(lecturer);
                TempData["Success"] = "Lecturer added successfully";
                return RedirectToAction(nameof(ManageUsers));
            }

            ViewBag.Contracts = _dataService.GetAllContracts();
            return View(lecturer);
        }

        // GET: HR/EditUser/{id} - Form to edit lecturer
        // Reference: Microsoft (2024) Routing in ASP.NET Core
        public IActionResult EditUser(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "HR")
                return Unauthorized();

            var lecturer = _dataService.GetLecturerById(id);
            if (lecturer == null)
                return NotFound();

            ViewBag.Contracts = _dataService.GetAllContracts();
            return View(lecturer);
        }

        // POST: HR/EditUser - Update lecturer with validation
        // Reference: Microsoft (2024) Model validation in ASP.NET Core MVC
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditUser(Lecturer lecturer)
        {
            if (HttpContext.Session.GetString("UserRole") != "HR")
                return Unauthorized();

            if (ModelState.IsValid)
            {
                _dataService.UpdateLecturer(lecturer);
                TempData["Success"] = "Lecturer updated successfully";
                return RedirectToAction(nameof(ManageUsers));
            }

            ViewBag.Contracts = _dataService.GetAllContracts();
            return View(lecturer);
        }

        // GET: HR/DeleteUser/{id} - Delete lecturer
        // Reference: Microsoft (2024) Routing in ASP.NET Core
        public IActionResult DeleteUser(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "HR")
                return Unauthorized();

            var lecturer = _dataService.GetLecturerById(id);
            if (lecturer == null)
                return NotFound();

            _dataService.DeleteLecturer(id);
            TempData["Success"] = "Lecturer deleted successfully";
            return RedirectToAction(nameof(ManageUsers));
        }
    }
}