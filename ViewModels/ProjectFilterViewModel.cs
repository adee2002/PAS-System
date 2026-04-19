public class ProjectFilterViewModel
{
    public string? ResearchArea { get; set; }
    public string? Keyword { get; set; }
    public string? Tags { get; set; }

    public List<string> AvailableResearchAreas { get; set; } = new();
    public List<ProjectViewModel> Projects { get; set; } = new();
    public HashSet<int> InterestedProjectIds { get; set; } = new();
    public List<SupervisorInterestViewModel> InterestedProjects { get; set; } = new();
}
