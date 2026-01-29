using System;
using System.ComponentModel.DataAnnotations;

namespace MunicipalServices.Models
{
    public class SearchHistory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string SearchTerm { get; set; } = string.Empty;
        public DateTime SearchDate { get; set; } = DateTime.Now;
        public string UserId { get; set; } = string.Empty;
    }
}
