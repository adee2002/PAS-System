using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Pages.Admin
{
    public class EditProjectModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditProjectModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Project ProjectItem { get; set; } = new();

        public List<ApplicationUser> Students { get; set; } = new();
        public List<ResearchArea> ResearchAreas { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Students = await _context.Users
                .Where(u => u.Role == "Student")
                .ToListAsync();

            ResearchAreas = await _context.ResearchAreas.ToListAsync();

            var project = await _context.Projects.FindAsync(id);

            if (project == null)
                return NotFound();

            ProjectItem = project;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Students = await _context.Users
                .Where(u => u.Role == "Student")
                .ToListAsync();

            ResearchAreas = await _context.ResearchAreas.ToListAsync();

            var project = await _context.Projects.FindAsync(ProjectItem.Id);

            if (project == null)
                return NotFound();

            project.Title = ProjectItem.Title;
            project.Abstract = ProjectItem.Abstract;
            project.TechStack = ProjectItem.TechStack;
            project.StudentId = ProjectItem.StudentId;
            project.ResearchAreaId = ProjectItem.ResearchAreaId;
            project.Status = ProjectItem.Status;

            await _context.SaveChangesAsync();

            return RedirectToPage("/Admin/Projects");
        }
    }
}