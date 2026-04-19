namespace ProjectApprovalSystem.Models.ViewModels
{
    public class BlindReviewViewModel
    {
        public int ProjectId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Abstract { get; set; } = string.Empty;
        public string TechnicalStack { get; set; } = string.Empty;
        public string ResearchAreaName { get; set; } = string.Empty;
        public bool HasExpressedInterest { get; set; }
        public bool IsAlreadyMatched { get; set; }
    }
}