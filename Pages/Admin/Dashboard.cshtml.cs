using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;

namespace WebApplication1.Pages.Admin
{
    public class DashboardModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DashboardModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public int TotalStudents { get; set; }
        public int TotalSupervisors { get; set; }
        public int TotalProjects { get; set; }
        public int TotalResearchAreas { get; set; }
        public int TotalMatches { get; set; }
        public int PendingProjects { get; set; }

        public async Task OnGetAsync()
        {
            TotalStudents = await _context.Users.CountAsync(u => u.Role == "Student");
            TotalSupervisors = await _context.Users.CountAsync(u => u.Role == "Supervisor");
            TotalProjects = await _context.Projects.CountAsync();
            TotalResearchAreas = await _context.ResearchAreas.CountAsync();
            TotalMatches = await _context.Matches.CountAsync();
            PendingProjects = await _context.Projects.CountAsync(p => p.Status == "Pending");
        }
    }
}