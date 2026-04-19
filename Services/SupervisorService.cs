using Microsoft.EntityFrameworkCore;

public class SupervisorService : ISupervisorService
{
    private readonly AppDbContext _dbContext;

    public SupervisorService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<ProjectViewModel>> GetBlindProjectsAsync(ProjectFilterViewModel filter)
    {
        var query = _dbContext.Projects
            .AsNoTracking()
            .Where(p => p.IsActive)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.ResearchArea))
        {
            query = query.Where(p => p.ResearchArea == filter.ResearchArea);
        }

        if (!string.IsNullOrWhiteSpace(filter.Keyword))
        {
            var keyword = filter.Keyword.Trim();
            query = query.Where(p =>
                p.Title.Contains(keyword) ||
                p.Abstract.Contains(keyword) ||
                p.TechStack.Contains(keyword));
        }

        if (!string.IsNullOrWhiteSpace(filter.Tags))
        {
            var tag = filter.Tags.Trim();
            query = query.Where(p => p.Tags.Contains(tag));
        }

        return await query
            .OrderByDescending(p => p.CreatedAtUtc)
            .Select(p => new ProjectViewModel
            {
                Id = p.Id,
                Title = p.Title,
                Abstract = p.Abstract,
                TechStack = p.TechStack,
                ResearchArea = p.ResearchArea,
                Tags = p.Tags
            })
            .ToListAsync();
    }

    public async Task<List<string>> GetResearchAreasAsync()
    {
        return await _dbContext.Projects
            .AsNoTracking()
            .Where(p => p.IsActive && !string.IsNullOrWhiteSpace(p.ResearchArea))
            .Select(p => p.ResearchArea)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync();
    }

    public async Task<bool> AddInterestAsync(int projectId, string supervisorId)
    {
        var projectExists = await _dbContext.Projects
            .AsNoTracking()
            .AnyAsync(p => p.Id == projectId && p.IsActive);

        if (!projectExists)
        {
            return false;
        }

        var alreadyInterested = await _dbContext.SupervisorInterests
            .AsNoTracking()
            .AnyAsync(i => i.ProjectId == projectId && i.SupervisorId == supervisorId);

        if (alreadyInterested)
        {
            return false;
        }

        var interest = new SupervisorInterest
        {
            ProjectId = projectId,
            SupervisorId = supervisorId,
            Status = "Interested",
            CreatedAtUtc = DateTime.UtcNow
        };

        _dbContext.SupervisorInterests.Add(interest);
        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<List<SupervisorInterestViewModel>> GetMyInterestsAsync(string supervisorId)
    {
        return await _dbContext.SupervisorInterests
            .AsNoTracking()
            .Include(i => i.Project)
            .Where(i => i.SupervisorId == supervisorId)
            .OrderByDescending(i => i.CreatedAtUtc)
            .Select(i => new SupervisorInterestViewModel
            {
                InterestId = i.Id,
                ProjectId = i.ProjectId,
                Title = i.Project != null ? i.Project.Title : string.Empty,
                Abstract = i.Project != null ? i.Project.Abstract : string.Empty,
                TechStack = i.Project != null ? i.Project.TechStack : string.Empty,
                ResearchArea = i.Project != null ? i.Project.ResearchArea : string.Empty,
                Tags = i.Project != null ? i.Project.Tags : string.Empty,
                Status = i.Status,
                InterestedAtUtc = i.CreatedAtUtc
            })
            .ToListAsync();
    }

    public async Task<HashSet<int>> GetInterestedProjectIdsAsync(string supervisorId)
    {
        var ids = await _dbContext.SupervisorInterests
            .AsNoTracking()
            .Where(i => i.SupervisorId == supervisorId)
            .Select(i => i.ProjectId)
            .ToListAsync();

        return ids.ToHashSet();
    }
}
