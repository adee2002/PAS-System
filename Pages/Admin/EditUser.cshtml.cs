using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Pages.Admin
{
    public class EditUserModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditUserModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ApplicationUser UserItem { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound();

            UserItem = user;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _context.Users.FindAsync(UserItem.Id);

            if (user == null)
                return NotFound();

            user.FullName = UserItem.FullName;
            user.Email = UserItem.Email;
            user.PasswordHash = UserItem.PasswordHash;
            user.Role = UserItem.Role;
            user.IsActive = UserItem.IsActive;

            await _context.SaveChangesAsync();

            return RedirectToPage("/Admin/Users");
        }
    }
}