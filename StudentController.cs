using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectApprovalSystem.Data;
using ProjectApprovalSystem.Models;
using ProjectApprovalSystem.Models.ViewModels;
using ProjectApprovalSystem.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectApprovalSystem.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMatchingService _matchingService;

        public StudentController(
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

            var projects = await _context.Projects
                .Include(p => p.ResearchArea)
                .Include(p => p.Supervisor)
                .Where(p => p.StudentId == user.Id)
                .OrderByDescending(p => p.SubmittedDate)
                .ToListAsync();

            var viewModel = new StudentDashboardViewModel
            {
                MyProjects = projects.Select(p => new StudentProjectViewModel
                {
                    Id = p.Id,
                    Title = p.Title,
                    ResearchAreaName = p.ResearchArea?.Name ?? "Unknown",
                    Status = p.Status.ToString(),
                    SubmittedDate = p.SubmittedDate,
                    CanEdit = p.Status == ProjectStatus.Pending,
                    SupervisorName = p.Supervisor != null ? $"{p.Supervisor.FirstName} {p.Supervisor.LastName}" : null,
                    IsIdentityRevealed = p.Status == ProjectStatus.Matched
                }).ToList(),
                TotalProjectsSubmitted = projects.Count,
                ActiveProjects = projects.Count(p => p.Status != ProjectStatus.Matched && p.Status != ProjectStatus.Withdrawn),
                MatchedProjects = projects.Count(p => p.Status == ProjectStatus.Matched)
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> CreateProposal()
        {
            var viewModel = new CreateProjectViewModel
            {
                ResearchAreas = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                    await _context.ResearchAreas.ToListAsync(),
                    "Id", "Name")
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProposal(CreateProjectViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return Challenge();

                var project = new Project
                {
                    Title = model.Title,
                    Abstract = model.Abstract,
                    TechnicalStack = model.TechnicalStack,
                    ResearchAreaId = model.ResearchAreaId,
                    StudentId = user.Id,
                    Status = ProjectStatus.Pending,
                    SubmittedDate = DateTime.UtcNow
                };

                _context.Projects.Add(project);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Project proposal submitted successfully!";
                return RedirectToAction(nameof(Dashboard));
            }

            model.ResearchAreas = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                await _context.ResearchAreas.ToListAsync(),
                "Id", "Name", model.ResearchAreaId);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditProposal(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == id && p.StudentId == user.Id);

            if (project == null || project.Status != ProjectStatus.Pending)
            {
                TempData["Error"] = "Cannot edit this proposal. It may already be under review or matched.";
                return RedirectToAction(nameof(Dashboard));
            }

            var viewModel = new CreateProjectViewModel
            {
                Title = project.Title,
                Abstract = project.Abstract,
                TechnicalStack = project.TechnicalStack,
                ResearchAreaId = project.ResearchAreaId,
                ResearchAreas = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                    await _context.ResearchAreas.ToListAsync(),
                    "Id", "Name", project.ResearchAreaId)
            };

            ViewBag.ProjectId = id;
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProposal(int id, CreateProjectViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == id && p.StudentId == user.Id);

            if (project == null || project.Status != ProjectStatus.Pending)
            {
                TempData["Error"] = "Cannot edit this proposal.";
                return RedirectToAction(nameof(Dashboard));
            }

            if (ModelState.IsValid)
            {
                project.Title = model.Title;
                project.Abstract = model.Abstract;
                project.TechnicalStack = model.TechnicalStack;
                project.ResearchAreaId = model.ResearchAreaId;
                project.LastUpdatedDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                TempData["Success"] = "Proposal updated successfully!";
                return RedirectToAction(nameof(Dashboard));
            }

            model.ResearchAreas = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                await _context.ResearchAreas.ToListAsync(),
                "Id", "Name", model.ResearchAreaId);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> WithdrawProposal(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == id && p.StudentId == user.Id);

            if (project != null && project.Status == ProjectStatus.Pending)
            {
                project.Status = ProjectStatus.Withdrawn;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Proposal withdrawn successfully.";
            }

            return RedirectToAction(nameof(Dashboard));
        }

        [HttpGet]
        public async Task<IActionResult> MatchDetails(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var match = await _context.Matches
                .Include(m => m.Supervisor)
                .Include(m => m.Project)
                .FirstOrDefaultAsync(m => m.ProjectId == id && m.StudentId == user.Id && m.Status == MatchStatus.Confirmed);

            if (match == null || !match.IsIdentityRevealed)
            {
                TempData["Error"] = "Match details not available yet.";
                return RedirectToAction(nameof(Dashboard));
            }

            return View(match);
        }
    }
}