// ReSharper disable InconsistentNaming
namespace Server.Models.Requests;

public class TodolistEdit
{
    public required int id { get; set; }
    public required bool isFinished { get; set; }
}