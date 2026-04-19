using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication1.Pages.Admin
{
    public class AdminLoginModel : PageModel
    {
        [BindProperty]
        public string Email { get; set; } = "";

        [BindProperty]
        public string Password { get; set; } = "";

        public string ErrorMessage { get; set; } = "";

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (Email == "admin@gmail.com" && Password == "123456")
            {
                return RedirectToPage("/Admin/Dashboard");
            }

            ErrorMessage = "Invalid email or password!";
            return Page();
        }
    }
}