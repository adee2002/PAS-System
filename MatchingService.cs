using Microsoft.EntityFrameworkCore;
using ProjectApprovalSystem.Data;
using ProjectApprovalSystem.Models;
using ProjectApprovalSystem.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectApprovalSystem.Services
{
    public class MatchingService : IMatchingService
    {
        private readonly ApplicationDbContext _context;

        public MatchingService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExpressInterestAsync(int projectId, string supervisorId)
        {
            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == projectId && p.Status == ProjectStatus.Pending);

            if (project == null)
                return false;

            var existingInterest = await _context.Matches
                .FirstOrDefaultAsync(m => m.ProjectId == projectId && m.SupervisorId == supervisorId);

            if (existingInterest != null)
                return false;

            var match = new Match
            {
                ProjectId = projectId,
                StudentId = project.StudentId,
                SupervisorId = supervisorId,
                Status = MatchStatus.Pending,
                InterestDate = DateTime.UtcNow,
                IsIdentityRevealed = false
            };

            _context.Matches.Add(match);
            await _context.SaveChangesAsync();

            project.Status = ProjectStatus.UnderReview;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ConfirmMatchAsync(int matchId, string supervisorId)
        {
            var match = await _context.Matches
                .Include(m => m.Project)
                .FirstOrDefaultAsync(m => m.Id == matchId && m.SupervisorId == supervisorId);

            if (match == null || match.Status != MatchStatus.Pending)
                return false;

            if (match.Project == null)
                return false;

            if (match.Project.Status != ProjectStatus.Pending && match.Project.Status != ProjectStatus.UnderReview)
                return false;

            match.Status = MatchStatus.Confirmed;
            match.ConfirmedDate = DateTime.UtcNow;
            match.Project.Status = ProjectStatus.Matched;
            match.Project.SupervisorId = supervisorId;
            match.Project.LastUpdatedDate = DateTime.UtcNow;
            match.IsIdentityRevealed = true;
            match.IdentityRevealedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Reject other pending interests
            var otherMatches = await _context.Matches
                .Where(m => m.ProjectId == match.ProjectId && m.Id != matchId && m.Status == MatchStatus.Pending)
                .ToListAsync();

            foreach (var otherMatch in otherMatches)
            {
                otherMatch.Status = MatchStatus.Rejected;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RevealIdentityAsync(int projectId)
        {
            var match = await _context.Matches
                .FirstOrDefaultAsync(m => m.ProjectId == projectId && m.Status == MatchStatus.Confirmed);

            if (match == null || match.IsIdentityRevealed)
                return false;

            match.IsIdentityRevealed = true;
            match.IdentityRevealedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<BlindReviewViewModel>> GetAvailableProjectsForSupervisorAsync(string supervisorId)
        {
            var expertiseAreas = await _context.SupervisorExpertises
                .Where(se => se.SupervisorId == supervisorId)
                .Select(se => se.ResearchAreaId)
                .ToListAsync();

            var projects = await _context.Projects
                .Include(p => p.ResearchArea)
                .Where(p => (p.Status == ProjectStatus.Pending || p.Status == ProjectStatus.UnderReview)
                    && (expertiseAreas.Contains(p.ResearchAreaId) || !expertiseAreas.Any()))
                .OrderByDescending(p => p.SubmittedDate)
                .ToListAsync();

            var existingMatches = await _context.Matches
                .Where(m => m.SupervisorId == supervisorId)
                .Select(m => m.ProjectId)
                .ToListAsync();

            var result = new List<BlindReviewViewModel>();
            foreach (var p in projects)
            {
                result.Add(new BlindReviewViewModel
                {
                    ProjectId = p.Id,
                    Title = p.Title,
                    Abstract = p.Abstract,
                    TechnicalStack = p.TechnicalStack,
                    ResearchAreaName = p.ResearchArea?.Name ?? "Unknown",
                    HasExpressedInterest = existingMatches.Contains(p.Id),
                    IsAlreadyMatched = p.Status == ProjectStatus.Matched
                });
            }
            return result;
        }

        public async Task<Match?> GetPendingMatchForProjectAsync(int projectId)
        {
            return await _context.Matches
                .Include(m => m.Supervisor)
                .Include(m => m.Student)
                .FirstOrDefaultAsync(m => m.ProjectId == projectId && m.Status == MatchStatus.Pending);
        }

        public async Task<bool> IsProjectAvailableForMatchingAsync(int projectId)
        {
            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == projectId && p.Status != ProjectStatus.Matched && p.Status != ProjectStatus.Withdrawn);
            return project != null;
        }
    }
}