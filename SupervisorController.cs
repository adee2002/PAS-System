using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectApprovalSystem.Data;
using ProjectApprovalSystem.Models;
using ProjectApprovalSystem.Models.ViewModels;
using ProjectApprovalSystem.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectApprovalSystem.Controllers
{
    [Authorize(Roles = "Supervisor")]
    public class SupervisorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMatchingService _matchingService;

        public SupervisorController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IMatchingService matchingService)
        {
            _context = context;
            _userManager = userManager;
            _matchingService = matchingService;
        }

        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var availableProjects = await _matchingService.GetAvailableProjectsForSupervisorAsync(user.Id);

            var expertiseAreas = await _context.SupervisorExpertises
                .Include(se => se.ResearchArea)
                .Where(se => se.SupervisorId == user.Id)
                .Select(se => se.ResearchArea)
                .ToListAsync();

            var matchesQuery = await _context.Matches
                .Include(m => m.Project)
                .Include(m => m.Student)
                .Where(m => m.SupervisorId == user.Id && m.Status == MatchStatus.Confirmed)
                .ToListAsync();

            var matches = new List<SupervisorMatchViewModel>();
            foreach (var m in matchesQuery)
            {
                matches.Add(new SupervisorMatchViewModel
                {
                    MatchId = m.Id,
                    ProjectId = m.ProjectId,
                    ProjectTitle = m.Project?.Title ?? "Unknown",
                    StudentName = m.Student != null ? $"{m.Student.FirstName} {m.Student.LastName}" : "Unknown",
                    StudentEmail = m.Student?.Email ?? "Unknown",
                    Status = m.Status.ToString(),
                    MatchDate = m.ConfirmedDate ?? m.InterestDate,
                    IsIdentityRevealed = m.IsIdentityRevealed
                });
            }

            var viewModel = new SupervisorDashboardViewModel
            {
                AvailableProjects = availableProjects,
                MyExpertiseAreas = expertiseAreas ?? new List<ResearchArea>(),
                MyMatches = matches,
                TotalAvailableProjects = availableProjects.Count,
                TotalMatches = matches.Count
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> ManageExpertise()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var allResearchAreas = await _context.ResearchAreas.ToListAsync();
            var myExpertiseIds = await _context.SupervisorExpertises
                .Where(se => se.SupervisorId == user.Id)
                .Select(se => se.ResearchAreaId)
                .ToListAsync();

            ViewBag.ResearchAreas = allResearchAreas;
            ViewBag.MyExpertiseIds = myExpertiseIds;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateExpertise(List<int> selectedAreaIds)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var existingExpertise = await _context.SupervisorExpertises
                .Where(se => se.SupervisorId == user.Id)
                .ToListAsync();
            _context.SupervisorExpertises.RemoveRange(existingExpertise);

            if (selectedAreaIds != null)
            {
                foreach (var areaId in selectedAreaIds)
                {
                    _context.SupervisorExpertises.Add(new SupervisorExpertise
                    {
                        SupervisorId = user.Id,
                        ResearchAreaId = areaId
                    });
                }
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Your expertise areas have been updated!";
            return RedirectToAction(nameof(Dashboard));
        }

        [HttpPost]
        public async Task<IActionResult> ExpressInterest(int projectId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var result = await _matchingService.ExpressInterestAsync(projectId, user.Id);

            if (result)
            {
                TempData["Success"] = "Interest expressed successfully! The student will be notified.";
            }
            else
            {
                TempData["Error"] = "Unable to express interest. The project may no longer be available.";
            }

            return RedirectToAction(nameof(Dashboard));
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmMatch(int matchId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var match = await _context.Matches
                .Include(m => m.Project)
                .FirstOrDefaultAsync(m => m.Id == matchId && m.SupervisorId == user.Id);

            if (match == null)
            {
                TempData["Error"] = "Match not found.";
                return RedirectToAction(nameof(Dashboard));
            }

            return View(match);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmMatch(int matchId, bool confirm)
        {
            if (!confirm)
            {
                TempData["Error"] = "Match confirmation cancelled.";
                return RedirectToAction(nameof(Dashboard));
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var result = await _matchingService.ConfirmMatchAsync(matchId, user.Id);

            if (result)
            {
                TempData["Success"] = "Match confirmed! Identities have been revealed to both parties.";
            }
            else
            {
                TempData["Error"] = "Unable to confirm match.";
            }

            return RedirectToAction(nameof(Dashboard));
        }

        [HttpGet]
        public async Task<IActionResult> ProjectDetails(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var project = await _context.Projects
                .Include(p => p.ResearchArea)
                .FirstOrDefaultAsync(p => p.Id == id && p.Status != ProjectStatus.Matched && p.Status != ProjectStatus.Withdrawn);

            if (project == null)
            {
                TempData["Error"] = "Project not found or no longer available.";
                return RedirectToAction(nameof(Dashboard));
            }

            var hasInterest = await _context.Matches
                .AnyAsync(m => m.ProjectId == id && m.SupervisorId == user.Id);

            ViewBag.HasExpressedInterest = hasInterest;

            return View(project);
        }
    }
}