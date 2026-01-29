using MunicipalServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MunicipalServices.Services
{
    public class EventDataService
    {
        // Internal data structure fields
        private readonly List<Event> _events;
        private readonly Dictionary<int, Event> _eventLookup;
        private readonly HashSet<string> _categories;

        // Priority Queue 
        private readonly PriorityQueue<Event, string> _priorityQueue;

        public EventDataService()
        {
            _events = GenerateMockEvents();
            _eventLookup = _events.ToDictionary(e => e.Id);

            // Storage and quick retrieval 
            _categories = new HashSet<string>(_events.Select(e => e.Category).Distinct());

            // Populating the Priority Queue
            _priorityQueue = new PriorityQueue<Event, string>(
                Comparer<string>.Create((p1, p2) =>
                {
                    if (p1 == p2) return 0;
                    if (p1 == "High") return -1; 
                    if (p2 == "High") return 1;  
                    if (p1 == "Medium") return -1;
                    if (p2 == "Medium") return 1;
                    return p1.CompareTo(p2); 
                })
            );

            foreach (var e in _events)
            {
                _priorityQueue.Enqueue(e, e.Priority);
            }
        }

        private List<Event> GenerateMockEvents()
        {
            return new List<Event>
            {
                new Event
                {
                    Id = 1,
                    Title = "Public Works Road Closure (High Alert)",
                    Description = "Emergency closure of Main Street (urgent pipe repair). Expected to last 48 hours.",
                    EventType = "Announcement",
                    EventDateTime = DateTime.Now.AddHours(2),
                    Location = "Main Street, Central District",
                    PostedDate = DateTime.Now,
                    Priority = "High",
                    Category = "Infrastructure"
                },
                new Event
                {
                    Id = 2,
                    Title = "Annual Community Cleanup Day",
                    Description = "Join us to help clean up local parks and green spaces. Free gift vouchers will be handed out!",
                    EventType = "Community Event",
                    EventDateTime = DateTime.Now.AddDays(7).AddHours(10),
                    Location = "City Park",
                    PostedDate = DateTime.Now.AddDays(-1),
                    Priority = "Medium",
                    Category = "Volunteer"
                },
                new Event
                {
                    Id = 3,
                    Title = "Budget Review Meeting",
                    Description = "City Council meeting to discuss the annual operating budget for the next fiscal year.",
                    EventType = "Council Meeting",
                    EventDateTime = DateTime.Now.AddDays(14).AddHours(18),
                    Location = "City Hall, Room 101",
                    PostedDate = DateTime.Now.AddDays(-5),
                    Priority = "Medium",
                    Category = "Governance"
                },
                new Event
                {
                    Id = 4,
                    Title = "Parks Department Seasonal Job Fair",
                    Description = "The Parks Department is now hiring staff for maintenance.",
                    EventType = "Job Fair",
                    EventDateTime = DateTime.Now.AddDays(20).AddHours(11),
                    Location = "Community Center Auditorium",
                    PostedDate = DateTime.Now.AddDays(-10),
                    Priority = "Low",
                    Category = "Employment"
                },
                new Event
                {
                    Id = 5,
                    Title = "Storm Drain Maintenance",
                    Description = "Routine maintenance in the downtown area, expect minor traffic delays.",
                    EventType = "Announcement",
                    EventDateTime = DateTime.Now.AddDays(5),
                    Location = "Various Downtown Locations",
                    PostedDate = DateTime.Now.AddDays(-2),
                    Priority = "High",
                    Category = "Infrastructure"
                }
            };
        }

        public IEnumerable<Event> GetAllEvents() => _events;

        // Dictionary/Hash Table Retrieval 
        public Event GetEventDetailsById(int id)
        {
            return _eventLookup.GetValueOrDefault(id);
        }

        // Set Retrieval 
        public HashSet<string> GetUniqueCategoriesForFilter() => _categories;


        // Priority Queue
        public IEnumerable<Event> GetPrioritizedEvents()
        {
            return _events.OrderByDescending(e => GetPriorityValue(e.Priority))
                          .ThenBy(e => e.EventDateTime); 
        }

        private static int GetPriorityValue(string priority)
        {
            return priority switch
            {
                "High" => 3,
                "Medium" => 2,
                "Low" => 1,
                _ => 0,
            };
        }


        // Search/Filter/Recommendation 
        public IEnumerable<Event> GetRecommendedEvents(string userId)
        {
            return GetPrioritizedEvents().Take(2);
        }

        // Search History Logging
        public void LogUserSearch(string userId, string searchTerms, string filterCategory)
        {
            Console.WriteLine($"[LOG] User {userId} searched for '{searchTerms}' with filter '{filterCategory}' at {DateTime.Now}");
        }
    }
}
