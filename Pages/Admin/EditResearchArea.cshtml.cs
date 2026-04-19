using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Pages.Admin
{
    public class EditResearchAreaModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditResearchAreaModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ResearchArea Area { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var area = await _context.ResearchAreas.FindAsync(id);
            if (area == null) return NotFound();

            Area = area;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var area = await _context.ResearchAreas.FindAsync(Area.Id);
            if (area == null) return NotFound();

            area.Name = Area.Name;
            await _context.SaveChangesAsync();

            return RedirectToPage("ResearchAreas");
        }
    }
}