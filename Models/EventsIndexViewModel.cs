using System.Collections.Generic;

namespace MunicipalServices.Models
{
    public class EventsIndexViewModel
    {
        public List<Event> Events { get; set; } = new List<Event>();
        public List<string> Categories { get; set; } = new List<string>();
        public List<Event> RecommendedEvents { get; set; } = new List<Event>();
        public string? CurrentCategoryFilter { get; set; }
        public string? CurrentSearchString { get; set; }
    }
}
