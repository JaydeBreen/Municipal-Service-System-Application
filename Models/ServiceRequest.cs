using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MunicipalServices.Models
{
    public class ServiceRequest
    {
        public int Id { get; set; }

        [Required, Display(Name = "Request Location")]
        public required string Location { get; set; }

        [Required]
        public required string Category { get; set; }

        [Required]
        public required string Description { get; set; }

        [Display(Name = "Attachment File Path")]
        public string? AttachmentPath { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; } = "Submitted";

        [Display(Name = "Resolution Notes")]
        public string? ResolutionNotes { get; set; }

        [Display(Name = "Submitted Date")]
        public DateTime SubmittedDate { get; set; } = DateTime.Now;

        [Display(Name = "Resolution Date")]
        public DateTime? ResolutionDate { get; set; }

        public required string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }

        public int Priority { get; set; } = 0;
    }
}
