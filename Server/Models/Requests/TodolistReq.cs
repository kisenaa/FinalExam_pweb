using System.ComponentModel.DataAnnotations;

namespace Server.Models.Requests;

public class TodolistReq
{
    [Required]
    public string Title { get; set; } = string.Empty;
    [Required]
    public string Description { get; set; } = string.Empty;
    [Required]
    public bool IsFinished { get; set; }
    [Required]
    public string UserId { get; set; } = null!;
}