using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectApprovalSystem.Models
{
    public class Project
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Project Title")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(4000)]
        [Display(Name = "Abstract")]
        public string Abstract { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        [Display(Name = "Technical Stack")]
        public string TechnicalStack { get; set; } = string.Empty;

        [Required]
        public int ResearchAreaId { get; set; }

        [Required]
        public string StudentId { get; set; } = string.Empty;

        public string? SupervisorId { get; set; }

        [Required]
        public ProjectStatus Status { get; set; } = ProjectStatus.Pending;

        public DateTime SubmittedDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastUpdatedDate { get; set; }

        [ForeignKey("ResearchAreaId")]
        public virtual ResearchArea? ResearchArea { get; set; }

        [ForeignKey("StudentId")]
        public virtual ApplicationUser? Student { get; set; }

        [ForeignKey("SupervisorId")]
        public virtual ApplicationUser? Supervisor { get; set; }

        public virtual ICollection<Match> Matches { get; set; } = new List<Match>();
    }

    public enum ProjectStatus
    {
        [Display(Name = "Pending")]
        Pending = 1,
        [Display(Name = "Under Review")]
        UnderReview = 2,
        [Display(Name = "Matched")]
        Matched = 3,
        [Display(Name = "Withdrawn")]
        Withdrawn = 4
    }
}