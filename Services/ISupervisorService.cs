public interface ISupervisorService
{
    Task<List<ProjectViewModel>> GetBlindProjectsAsync(ProjectFilterViewModel filter);
    Task<List<string>> GetResearchAreasAsync();
    Task<bool> AddInterestAsync(int projectId, string supervisorId);
    Task<List<SupervisorInterestViewModel>> GetMyInterestsAsync(string supervisorId);
    Task<HashSet<int>> GetInterestedProjectIdsAsync(string supervisorId);
}
