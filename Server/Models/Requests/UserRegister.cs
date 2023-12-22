using System.ComponentModel.DataAnnotations;

namespace Server.Models.Requests;

public class UserRegister
{
    [MaxLength(25, ErrorMessage = "Username must be at most 25 characters"), MinLength(1, ErrorMessage = "Username must be at least 1 character")]
    public string Username { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Email is required"), EmailAddress(ErrorMessage = "Invalid Email Address")]
    public string Email { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Password is required"), MinLength(8, ErrorMessage = "Password must be at least 8 character")]
    public string Password { get; set; } = string.Empty;
}