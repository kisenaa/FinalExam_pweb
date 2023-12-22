using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models.Database;

public class RefreshToken
{
    [Required, Key, Column(TypeName = "char(32)")]
    public string Token { get; set; } = null!;
    
    [Required, Column(TypeName = "char(27)"), ForeignKey(nameof(User))]
    public string UserId { get; set; } = null!;
    
    [Required]
    public DateTime Created { get; set; } = DateTime.UtcNow;
    
    [Required]
    public DateTime Expires { get; set; }
    
    // One to One Dependent Relationship
    public virtual User User { get; set; } = null!;
}