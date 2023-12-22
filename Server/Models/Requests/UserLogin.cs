using System.ComponentModel.DataAnnotations;

namespace Server.Models.Requests;

public class UserLogin
{
    [Required]
    public string IdentityInput { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
}