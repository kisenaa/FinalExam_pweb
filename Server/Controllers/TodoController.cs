using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Server.Data;
using Server.Data.Repository;
using Server.Models.Database;
using Server.Models.Requests;

namespace Server.Controllers;

[ApiController]
[Route("api/todo/")]
public class TodoController: BaseController
{
    public TodoController(AppDbContext dbContext, IRepository repository) : base(dbContext, repository)
    {
    }

    [HttpGet("getTodo")]
    public async Task<IActionResult> GetTodo([FromHeader(Name = "X-Userid")] string userId )
    {
        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest("X-userId header is missing");
        }
        var todolist = await _repository.Todolist.GetByUserId(userId);
        if (todolist.IsNullOrEmpty())
        {
            return BadRequest("Not Found");
        }
        return Ok(todolist);
    }

    [HttpPost("addTodo")]
    public async Task<IActionResult> AddTodo([FromBody] TodolistReq todolist)
    {
        string? userId = HttpContext.Request.Headers["X-Userid"];
        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest("X-userId header is missing");
        }
        
        var todo = new Todolist()
        {
            Title = todolist.Title,
            Description = todolist.Description,
            IsFinished = todolist.IsFinished,
            UserId = userId
        };
        await _repository.Todolist.Add(todo);
        await _repository.SaveChangesAsync();
        return Ok(todo);
    }

    [HttpDelete("removeTodo")]
    public async Task<IActionResult> DeleteTodo([FromBody] int id)
    {
        await _repository.Todolist.Remove(id);
        await _repository.SaveChangesAsync();
        return Ok("Deleted");
    }

    [HttpPost("editTodo")]
    public async Task<IActionResult> EditTodo([FromBody] TodolistEdit todolistEdit)
    {
        await _repository.Todolist.SetFinished(todolistEdit.id, todolistEdit.isFinished);
        await _repository.SaveChangesAsync();
        return Ok("Edited");
    }
        
}