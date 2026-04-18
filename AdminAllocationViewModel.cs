namespace ProjectApprovalSystem.Models.ViewModels
{
    public class AdminAllocationViewModel
    {
        public List<MatchViewModel> Matches { get; set; } = new();
        public int TotalMatches { get; set; }
        public int PendingMatches { get; set; }
        public int ConfirmedMatches { get; set; }
    }

    public class MatchViewModel
    {
        public int MatchId { get; set; }
        public int ProjectId { get; set; }
        public string ProjectTitle { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string StudentEmail { get; set; } = string.Empty;
        public string SupervisorName { get; set; } = string.Empty;
        public string SupervisorEmail { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime MatchDate { get; set; }
    }
}