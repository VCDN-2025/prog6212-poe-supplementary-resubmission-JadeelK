using Microsoft.AspNetCore.Mvc;
using CMCS.Models;
using CMCS.Services;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using System.Linq;

/*
    References used in this controller:
    - Microsoft (2024) Routing in ASP.NET Core. https://learn.microsoft.com/en-us/aspnet/core/fundamentals/routing
    - Microsoft (2024) Dependency injection in ASP.NET Core. https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection
    - Microsoft (2024) Model validation in ASP.NET Core MVC. https://learn.microsoft.com/en-us/aspnet/core/mvc/models/validation
    - Microsoft (2024) File uploads in ASP.NET Core. https://learn.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads
    - Microsoft (2024) TempData in ASP.NET Core. (used for success/error messages)
    - Microsoft (2024) Anti-forgery token in ASP.NET Core. (used for POST security)
*/

namespace CMCS.Controllers
{
    public class ClaimsController : Controller
    {
        private readonly InMemoryDataService _dataService;
        private readonly FileEncryptionService _encryptionService;

        public ClaimsController(InMemoryDataService dataService, FileEncryptionService encryptionService)
        {
            _dataService = dataService;
            _encryptionService = encryptionService;
        }

        // GET: Claims/Index - Displays all claims with role-based actions
        // Reference: Microsoft (2024) Routing in ASP.NET Core
        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(role))
                return RedirectToAction("Index", "Home");

            var claims = _dataService.GetAllClaims(); // All roles see all claims for simplicity/debug

            ViewBag.UserRole = role;

            return View(claims);
        }

        // GET: Claims/Create - Form for new claim (Lecturer only)
        // Reference: Microsoft (2024) Model validation in ASP.NET Core MVC
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("UserRole") != "Lecturer")
                return RedirectToAction("Index", "Home");

            ViewBag.Lecturers = _dataService.GetAllLecturers();
            ViewBag.Contracts = _dataService.GetAllContracts();

            return View(new Claim());
        }

        // POST: Claims/Create - Submit claim with validation & encryption
        // References: Microsoft (2024) File uploads in ASP.NET Core; Microsoft (2024) Model validation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Claim claim, IFormFile[] documents)
        {
            ModelState.Remove("Status");
            ModelState.Remove("LecturerName");
            ModelState.Remove("TotalAmount");
            ModelState.Remove("SubmittedAt");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .Where(m => !string.IsNullOrEmpty(m));

                TempData["Error"] = "Submission failed: " +
                    (errors.Any() ? string.Join("; ", errors) : "Check required fields");

                ViewBag.Lecturers = _dataService.GetAllLecturers();
                ViewBag.Contracts = _dataService.GetAllContracts();
                return View(claim);
            }

            var lecturer = _dataService.GetLecturerById(claim.LecturerId);
            if (lecturer == null)
            {
                TempData["Error"] = "Invalid lecturer selected.";
                ViewBag.Lecturers = _dataService.GetAllLecturers();
                ViewBag.Contracts = _dataService.GetAllContracts();
                return View(claim);
            }

            claim.LecturerName = lecturer.FullName;
            claim.CalculateTotalAmount();

            var addedClaim = _dataService.AddClaim(claim);

            if (documents?.Any() == true)
            {
                foreach (var file in documents.Where(f => f?.Length > 0))
                {
                    if (!_encryptionService.IsValidFileType(file.FileName) ||
                        !_encryptionService.IsValidFileSize(file.Length))
                    {
                        TempData["Warning"] ??= "";
                        TempData["Warning"] += $"Skipped invalid file: {file.FileName}; ";
                        continue;
                    }

                    using var ms = new MemoryStream();
                    file.CopyTo(ms);
                    var encrypted = _encryptionService.EncryptFile(ms.ToArray());

                    _dataService.AddDocument(new SupportingDocument
                    {
                        ClaimId = addedClaim.ClaimId,
                        FileName = file.FileName,
                        FilePath = $"/uploads/{Guid.NewGuid()}{Path.GetExtension(file.FileName)}",
                        FileSizeKB = file.Length / 1024,
                        EncryptedContent = encrypted
                    });
                }
            }

            TempData["Success"] = $"Claim #{addedClaim.ClaimId} submitted successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Claims/Details/{id} - View claim details (open to all roles)
        // Reference: Microsoft (2024) Routing in ASP.NET Core
        public IActionResult Details(int id)
        {
            var claim = _dataService.GetClaimById(id);
            if (claim == null) return NotFound();

            ViewBag.Documents = _dataService.GetDocumentsByClaim(id);
            ViewBag.Approvals = _dataService.GetApprovalsByClaim(id);
            ViewBag.UserRole = HttpContext.Session.GetString("UserRole");

            return View(claim);
        }

        // POST: Claims/Approve - Approve/reject claim (Coordinator/Manager only)
        // Reference: Microsoft (2024) Anti-forgery token in ASP.NET Core (for POST security)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Approve(int claimId, string decision, string comments)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Coordinator" && role != "Manager")
                return Unauthorized("You do not have permission to approve claims.");

            var claim = _dataService.GetClaimById(claimId);
            if (claim == null) return NotFound();

            var approval = new Approval
            {
                ClaimId = claimId,
                ApproverRole = role,
                ApproverName = $"{role} User",
                Decision = decision,
                Comments = comments ?? ""
            };
            _dataService.AddApproval(approval);

            if (decision == "Approved")
            {
                if (role == "Coordinator")
                    _dataService.UpdateClaimStatus(claimId, "Verified");
                else
                    _dataService.UpdateClaimStatus(claimId, "Approved");
            }
            else
            {
                _dataService.UpdateClaimStatus(claimId, "Rejected");
            }

            TempData["Success"] = $"Claim {decision.ToLower()} successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Claims/DownloadDocument/{id} - Download decrypted file
        public IActionResult DownloadDocument(int id)
        {
            var doc = _dataService.GetDocumentById(id);
            if (doc == null) return NotFound();

            var decrypted = _encryptionService.DecryptFile(doc.EncryptedContent);
            return File(decrypted, "application/octet-stream", doc.FileName);
        }
    }
}