// ReSharper disable InconsistentNaming
namespace Client.Models;

public class TodolistRequest
{
    public required string? title {get; set;}
    public required string? description {get; set;} 
    public required bool isFinished {get; set;}
    public required string userId { get; set; }
}