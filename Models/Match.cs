namespace WebApplication1.Models
{
    public class Match
    {
        public int Id { get; set; }

        public int ProjectId { get; set; }
        public Project? Project { get; set; }

        public int SupervisorId { get; set; }
        public ApplicationUser? Supervisor { get; set; }

        public DateTime MatchedDate { get; set; } = DateTime.Now;
    }
}