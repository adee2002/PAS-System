namespace ProjectApprovalSystem.Models.ViewModels
{
    public class StudentDashboardViewModel
    {
        public List<StudentProjectViewModel> MyProjects { get; set; } = new();
        public int TotalProjectsSubmitted { get; set; }
        public int ActiveProjects { get; set; }
        public int MatchedProjects { get; set; }
    }

    public class StudentProjectViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ResearchAreaName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime SubmittedDate { get; set; }
        public bool CanEdit { get; set; }
        public string? SupervisorName { get; set; }
        public bool IsIdentityRevealed { get; set; }
    }
}