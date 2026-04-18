using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectApprovalSystem.Models
{
    public class Match
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProjectId { get; set; }

        [Required]
        public string StudentId { get; set; } = string.Empty;

        [Required]
        public string SupervisorId { get; set; } = string.Empty;

        public MatchStatus Status { get; set; } = MatchStatus.Pending;
        public DateTime InterestDate { get; set; } = DateTime.UtcNow;
        public DateTime? ConfirmedDate { get; set; }
        public bool IsIdentityRevealed { get; set; } = false;
        public DateTime? IdentityRevealedDate { get; set; }

        [ForeignKey("ProjectId")]
        public virtual Project? Project { get; set; }

        [ForeignKey("StudentId")]
        public virtual ApplicationUser? Student { get; set; }

        [ForeignKey("SupervisorId")]
        public virtual ApplicationUser? Supervisor { get; set; }
    }

    public enum MatchStatus
    {
        [Display(Name = "Pending")]
        Pending = 1,
        [Display(Name = "Confirmed")]
        Confirmed = 2,
        [Display(Name = "Rejected")]
        Rejected = 3
    }
}