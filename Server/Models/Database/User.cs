using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models.Database;

public class User
{
    [Required, Column(TypeName = "char(27)")]
    public string UserId { get; set; } = null!;
    
    [Required, MaxLength(25)] 
    public string Username { get; set; } = null!;
    
    [Required, MaxLength(100)] 
    public string Email { get; set; } = null!;
    
    [Required] 
    public Memory<byte> PasswordHash { get; set; } = null!;
    // One to One Dependent Relationship
    public virtual RefreshToken? RefreshToken { get; set; }
}