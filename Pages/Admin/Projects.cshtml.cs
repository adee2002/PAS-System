using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Pages.Admin
{
    public class ProjectsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ProjectsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Project> ProjectList { get; set; } = new();
        public List<ApplicationUser> Students { get; set; } = new();
        public List<ResearchArea> ResearchAreas { get; set; } = new();

        [BindProperty]
        public Project NewProject { get; set; } = new();

        public string? SearchTerm { get; set; }
        public string? StatusFilter { get; set; }

        public async Task OnGetAsync(string? searchTerm, string? statusFilter)
        {
            SearchTerm = searchTerm;
            StatusFilter = statusFilter;

            Students = await _context.Users
                .Where(u => u.Role == "Student")
                .ToListAsync();

            ResearchAreas = await _context.ResearchAreas.ToListAsync();

            var query = _context.Projects
                .Include(p => p.Student)
                .Include(p => p.ResearchArea)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p => p.Title.Contains(searchTerm));
            }

            if (!string.IsNullOrEmpty(statusFilter))
            {
                query = query.Where(p => p.Status == statusFilter);
            }

            ProjectList = await query.ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Students = await _context.Users
                .Where(u => u.Role == "Student")
                .ToListAsync();

            ResearchAreas = await _context.ResearchAreas.ToListAsync();

            if (!ModelState.IsValid)
            {
                var query = _context.Projects
                    .Include(p => p.Student)
                    .Include(p => p.ResearchArea)
                    .AsQueryable();

                ProjectList = await query.ToListAsync();
                return Page();
            }

            _context.Projects.Add(NewProject);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var project = await _context.Projects.FindAsync(id);

            if (project != null)
            {
                _context.Projects.Remove(project);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUpdateStatusAsync(int id, string status)
        {
            var project = await _context.Projects.FindAsync(id);

            if (project != null)
            {
                project.Status = status;
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }
    }
}