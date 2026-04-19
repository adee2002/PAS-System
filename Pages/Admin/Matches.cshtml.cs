using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Pages.Admin
{
    public class MatchesModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public MatchesModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Match> MatchList { get; set; } = new();
        public List<Project> Projects { get; set; } = new();
        public List<ApplicationUser> Supervisors { get; set; } = new();

        [BindProperty]
        public Match NewMatch { get; set; } = new();

        public async Task OnGetAsync()
        {
            MatchList = await _context.Matches
                .Include(m => m.Project)
                .Include(m => m.Project.Student)
                .Include(m => m.Supervisor)
                .ToListAsync();

            Projects = await _context.Projects
                .Include(p => p.Student)
                .ToListAsync();

            Supervisors = await _context.Users
                .Where(u => u.Role == "Supervisor")
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Projects = await _context.Projects.ToListAsync();
            Supervisors = await _context.Users
                .Where(u => u.Role == "Supervisor")
                .ToListAsync();

            if (!ModelState.IsValid)
            {
                MatchList = await _context.Matches.ToListAsync();
                return Page();
            }

            NewMatch.MatchedDate = DateTime.Now;

            _context.Matches.Add(NewMatch);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var match = await _context.Matches.FindAsync(id);

            if (match != null)
            {
                _context.Matches.Remove(match);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }
    }
}