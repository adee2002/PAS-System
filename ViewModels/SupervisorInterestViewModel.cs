public class SupervisorInterestViewModel
{
    public int InterestId { get; set; }
    public int ProjectId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Abstract { get; set; } = string.Empty;
    public string TechStack { get; set; } = string.Empty;
    public string ResearchArea { get; set; } = string.Empty;
    public string Tags { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime InterestedAtUtc { get; set; }
}
