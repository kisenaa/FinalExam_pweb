using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models.Database;

public class Todolist
{
    [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required, MaxLength(64)] 
    public string Title { get; set; } = string.Empty;

    [Required, MaxLength(200)] 
    public string Description { get; set; } = string.Empty;

    [Required] 
    public bool IsFinished { get; set; }
    
    [Required, Column(TypeName = "char(27)"), ForeignKey(nameof(User))]
    public string UserId { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}