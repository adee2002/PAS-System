using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class SupervisorInterest
{
    public int Id { get; set; }

    [Required]
    public string SupervisorId { get; set; } = string.Empty;

    public int ProjectId { get; set; }

    [Required]
    [StringLength(30)]
    public string Status { get; set; } = "Interested";

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(SupervisorId))]
    public ApplicationUser? Supervisor { get; set; }

    [ForeignKey(nameof(ProjectId))]
    public Project? Project { get; set; }
}
