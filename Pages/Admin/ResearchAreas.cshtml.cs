using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Pages.Admin
{
    public class ResearchAreasModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ResearchAreasModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<ResearchArea> AreaList { get; set; } = new();

        [BindProperty]
        public ResearchArea NewArea { get; set; } = new();

        public async Task OnGetAsync()
        {
            AreaList = await _context.ResearchAreas.ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                AreaList = await _context.ResearchAreas.ToListAsync();
                return Page();
            }

            _context.ResearchAreas.Add(NewArea);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var area = await _context.ResearchAreas.FindAsync(id);
            if (area != null)
            {
                _context.ResearchAreas.Remove(area);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }
    }
}