using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MunicipalServices.Models;
using MunicipalServices.Services;
using System.Linq;

namespace MunicipalServices.Controllers
{
    public class EventsController : Controller
    {
        private readonly EventDataService _eventDataService;

        public EventsController(EventDataService eventDataService)
        {
            _eventDataService = eventDataService;
        }

        // GET: /Events
        public IActionResult Index(string categoryFilter, string searchString)
        {

            var events = _eventDataService.GetPrioritizedEvents().ToList();

            if (!string.IsNullOrEmpty(categoryFilter) && categoryFilter != "All")
            {
                events = events.Where(e => e.Category == categoryFilter).ToList();
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.Trim();
                events = events.Where(e => e.Title.Contains(searchString) || e.Description.Contains(searchString)).ToList();
            }

            _eventDataService.LogUserSearch("Guest", searchString ?? "", categoryFilter ?? "All");

            var categories = _eventDataService.GetUniqueCategoriesForFilter().ToList();
            categories.Insert(0, "All");

            var viewModel = new EventsIndexViewModel
            {
                Events = events, 
                Categories = categories,
                RecommendedEvents = _eventDataService.GetRecommendedEvents("Guest").ToList(),
                CurrentCategoryFilter = categoryFilter,
                CurrentSearchString = searchString
            };

            return View(viewModel);
        }

        public IActionResult Details(int id)
        {
            // Dictionary/Hash Table Retrieval 
            var eventItem = _eventDataService.GetEventDetailsById(id);

            if (eventItem == null)
            {
                return NotFound();
            }

            return View(eventItem);
        }

        [Authorize(Roles = "Admin, Staff")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Staff")]
        public IActionResult Create(Event eventItem)
        {
            if (ModelState.IsValid)
            {
                // Saving the event 
                return RedirectToAction(nameof(Index));
            }
            return View(eventItem);
        }
    }
}
