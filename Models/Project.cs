namespace WebApplication1.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Abstract { get; set; } = string.Empty;
        public string TechStack { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";

        public int StudentId { get; set; }
        public ApplicationUser? Student { get; set; }

        public int ResearchAreaId { get; set; }
        public ResearchArea? ResearchArea { get; set; }
    }
}