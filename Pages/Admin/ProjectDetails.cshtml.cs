using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Pages.Admin
{
    public class ProjectDetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ProjectDetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Project? ProjectItem { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            ProjectItem = await _context.Projects
                .Include(p => p.Student)
                .Include(p => p.ResearchArea)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (ProjectItem == null)
                return NotFound();

            return Page();
        }
    }
}