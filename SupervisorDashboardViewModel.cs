namespace ProjectApprovalSystem.Models.ViewModels
{
    public class SupervisorDashboardViewModel
    {
        public List<BlindReviewViewModel> AvailableProjects { get; set; } = new();
        public List<SupervisorMatchViewModel> MyMatches { get; set; } = new();
        public List<ResearchArea> MyExpertiseAreas { get; set; } = new();
        public int TotalAvailableProjects { get; set; }
        public int TotalMatches { get; set; }
    }

    public class SupervisorMatchViewModel
    {
        public int MatchId { get; set; }
        public int ProjectId { get; set; }
        public string ProjectTitle { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string StudentEmail { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime MatchDate { get; set; }
        public bool IsIdentityRevealed { get; set; }
    }

    // DO NOT include BlindReviewViewModel here - it's in its own file
}