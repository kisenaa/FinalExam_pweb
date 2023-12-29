// ReSharper disable InconsistentNaming
namespace Client.Models;

public class Todolist
{
    public int? id { get; set; }
    public string? title {get; set;}
    public string? description {get; set;} 
    public bool? isFinished {get; set;}
}