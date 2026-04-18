using System.ComponentModel.DataAnnotations;

public class RegisterViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [MinLength(6)]
    public string Password { get; set; }

    [Required]
    public string Role { get; set; }
}