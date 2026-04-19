using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectApprovalSystem.Models;

namespace ProjectApprovalSystem.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> DebugUsers([FromServices] UserManager<ApplicationUser> userManager)
        {
            var users = await userManager.Users.ToListAsync();
            var html = "<html><head><title>Users in Database</title></head><body>";
            html += "<h1>Users in Database</h1>";

            if (!users.Any())
            {
                html += "<p style='color:red'>No users found! Seeding didn't work.</p>";
            }
            else
            {
                html += "<table border='1' cellpadding='10'>";
                html += "<tr><th>Email</th><th>First Name</th><th>Last Name</th><th>Roles</th></tr>";

                foreach (var user in users)
                {
                    var roles = await userManager.GetRolesAsync(user);
                    html += $"<tr>";
                    html += $"<td>{user.Email}</td>";
                    html += $"<td>{user.FirstName}</td>";
                    html += $"<td>{user.LastName}</td>";
                    html += $"<td>{string.Join(", ", roles)}</td>";
                    html += $"</tr>";
                }
                html += "</table>";
            }

            html += "<br><a href='/'>Go Home</a>";
            html += "</body></html>";

            return Content(html, "text/html");
        }
    }
}