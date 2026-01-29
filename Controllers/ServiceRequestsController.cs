﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MunicipalServices.Data;
using MunicipalServices.Utils;
using MunicipalServices.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore; 

namespace MunicipalServices.Controllers
{
    [Authorize] 
    public class ServiceRequestsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public ServiceRequestsController(ApplicationDbContext context,
                                        UserManager<ApplicationUser> userManager,
                                        IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }

        [HttpGet]
        public IActionResult Report()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Report(ServiceRequest request, IFormFile? attachment)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    TempData["Error"] = "You must be logged in to submit a request.";
                    return RedirectToAction("Index", "Home");
                }

                request.UserId = userId;
                request.SubmittedDate = DateTime.Now;
                request.Status = "Submitted"; 

                if (attachment != null && attachment.Length > 0)
                {
                    try
                    {
                        var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + attachment.FileName;
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await attachment.CopyToAsync(stream);
                        }

                        request.AttachmentPath = "/uploads/" + uniqueFileName;
                    }
                    catch (Exception ex)
                    {
                        TempData["Error"] = "Error saving file: " + ex.Message;
                    }
                }

                _context.ServiceRequests.Add(request);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Your service request has been successfully submitted!";
                return RedirectToAction(nameof(List));
            }

            return View(request);
        }

        public async Task<IActionResult> List(string? search, string? category, string? status)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            bool isStaffOrAdmin = User.IsInRole("Staff") || User.IsInRole("Admin");

            IQueryable<ServiceRequest> requestsQuery = _context.ServiceRequests.Include(r => r.User);

            ViewData["UserRole"] = isStaffOrAdmin ? "Staff" : "Citizen";

            if (!isStaffOrAdmin)
            {
                requestsQuery = requestsQuery.Where(r => r.UserId == userId);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                if (int.TryParse(search, out int requestId))
                {
                    requestsQuery = requestsQuery.Where(r => r.Id == requestId);
                }
                else
                {
                    string lowerSearch = search.Trim().ToLower();
                    requestsQuery = requestsQuery.Where(r =>
                        r.Location.ToLower().Contains(lowerSearch) || r.Description.ToLower().Contains(lowerSearch));
                }
            }

            if (!string.IsNullOrWhiteSpace(category) && category != "All Categories")
            {
                requestsQuery = requestsQuery.Where(r => r.Category == category);
                ViewData["CurrentCategory"] = category;
            }

            if (!string.IsNullOrWhiteSpace(status) && status != "All Statuses")
            {
                requestsQuery = requestsQuery.Where(r => r.Status == status);
                ViewData["CurrentStatus"] = status;
            }

            List<ServiceRequest> requests;

            if (isStaffOrAdmin)
            {
                // For staff, filter and then sort by priority using the heap
                var filteredRequests = await requestsQuery.ToListAsync();
                var priorityQueue = new ServiceRequestPriorityQueue();
                foreach (var request in filteredRequests)
                {
                    priorityQueue.Enqueue(request);
                }

                requests = new List<ServiceRequest>();
                while (priorityQueue.Count > 0)
                {
                    requests.Add(priorityQueue.Dequeue());
                }
            }
            else
            {
                // For citizens, sort by date as before
                requests = await requestsQuery.OrderByDescending(r => r.SubmittedDate).ToListAsync();
            }

            ViewData["Categories"] = new List<string> { "Pothole", "Water Leak", "Illegal Dumping", "Street Light Out", "Noise Complaint", "Other" };
            ViewData["Statuses"] = new List<string> { "Submitted", "In Progress", "Resolved" };

            return View(requests);
        }

        // GET: ServiceRequests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceRequest = await _context.ServiceRequests
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (serviceRequest == null)
            {
                return NotFound();
            }

            return View(serviceRequest);
        }

        [Authorize(Roles = "Staff,Admin")]
        public async Task<IActionResult> StaffUpdate(int id)
        {
            var request = await _context.ServiceRequests.Include(r => r.User).FirstOrDefaultAsync(r => r.Id == id);

            if (request == null)
            {
                return NotFound();
            }

            ViewData["Statuses"] = new List<string> { "Submitted", "In Progress", "Resolved" };

            return View(request);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Staff,Admin")]
        public async Task<IActionResult> StaffUpdate(int id, [Bind("Id,Status,ResolutionNotes")] ServiceRequest updatedRequest)
        {
            if (id != updatedRequest.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var requestToUpdate = await _context.ServiceRequests.FirstOrDefaultAsync(r => r.Id == id);

                if (requestToUpdate == null)
                {
                    return NotFound();
                }

                requestToUpdate.Status = updatedRequest.Status;
                requestToUpdate.ResolutionNotes = updatedRequest.ResolutionNotes;

                if (requestToUpdate.Status == "Resolved" && requestToUpdate.ResolutionDate == null)
                {
                    requestToUpdate.ResolutionDate = DateTime.Now;
                }
                else if (requestToUpdate.Status != "Resolved")
                {
                    requestToUpdate.ResolutionDate = null;
                }

                try
                {
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Request #{requestToUpdate.Id} status updated to {requestToUpdate.Status}.";
                    return RedirectToAction(nameof(List));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.ServiceRequests.Any(e => e.Id == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            // If ModelState is invalid, reload the view with the necessary data.
            ViewData["Statuses"] = new List<string> { "Submitted", "In Progress", "Resolved" };
            // It's good practice to reload the full entity to display in the view upon validation failure.
            var originalRequest = await _context.ServiceRequests.AsNoTracking().Include(r => r.User).FirstOrDefaultAsync(r => r.Id == id);
            return View(originalRequest);
        }
    }
}
