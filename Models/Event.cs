using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MunicipalServices.Models
{
    public class Event
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public required string Title { get; set; }

        [Required]
        public required string Description { get; set; }

        [Required]
        [Display(Name = "Event Type")]
        [StringLength(50)]
        public required string EventType { get; set; }

        [Required]
        [Display(Name = "Event Date/Time")]
        [DataType(DataType.DateTime)]
        public DateTime EventDateTime { get; set; } = DateTime.Now;

        [Required]
        [StringLength(100)]
        public required string Location { get; set; }

        [Display(Name = "Posted Date")]
        public DateTime PostedDate { get; set; } = DateTime.Now;

        [Required]
        [StringLength(20)]
        public required string Priority { get; set; }

        [Required]
        [StringLength(50)]
        public required string Category { get; set; }

        [NotMapped] 
        public string Date => EventDateTime.ToShortDateString();

        [NotMapped] 
        public string Time => EventDateTime.ToShortTimeString();
    }
}