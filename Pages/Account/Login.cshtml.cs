using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class LoginModel : PageModel
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;

    public LoginModel(SignInManager<IdentityUser> signInManager,
                      UserManager<IdentityUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [BindProperty]
    public LoginViewModel Input { get; set; }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var result = await _signInManager.PasswordSignInAsync(
            Input.Email, Input.Password, false, false);

        if (result.Succeeded)
        {
            var user = await _userManager.FindByEmailAsync(Input.Email);
            var roles = await _userManager.GetRolesAsync(user);

            
            if (roles.Contains("Supervisor"))
                return RedirectToPage("/Supervisor/Dashboard");

            if (roles.Contains("Student"))
                return RedirectToPage("/Student/Dashboard");

            if (roles.Contains("Admin"))
                return RedirectToPage("/Admin/Dashboard");
        }

        ModelState.AddModelError("", "Invalid login attempt");
        return Page();
    }
}
