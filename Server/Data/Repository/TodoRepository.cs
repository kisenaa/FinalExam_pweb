using System.Collections;
using Microsoft.EntityFrameworkCore;
using Server.Models.Database;

namespace Server.Data.Repository;

public interface ITodoRepository
{
    Task<List<Todolist>> GetByUserId(string userId);
    Task Add(Todolist todolist);
    Task Remove(int id);
    Task SetFinished(int id, bool isFinished);
}

internal class TodoQuery
{
    public static readonly Func<AppDbContext, string, Task<List<Todolist>>> GetByUserId = EF.CompileAsyncQuery(
        (AppDbContext context, string userId) =>
            context.Todolists
                .AsNoTracking()
                .Where(u => u.UserId == userId)
                .AsQueryable()
                .ToList()
            );

    public static readonly Func<AppDbContext, Todolist, Task> AddTodolist = EF.CompileAsyncQuery(
        (AppDbContext context, Todolist todolist) => 
            context.Todolists.Add(todolist)
    );

    public static readonly Func<AppDbContext, int, Task> RemoveTodolist = EF.CompileAsyncQuery(
        (AppDbContext context, int id) => 
            context.Todolists
                .AsNoTracking()
                .Where(u => u.Id == id)
                .ExecuteDelete()
    );

    public static readonly Func<AppDbContext, int, bool, Task> SetFinished = EF.CompileAsyncQuery(
        (AppDbContext context, int id, bool isFinished) => 
            context.Todolists
                .AsNoTracking()
                .Where(u => u.Id == id)
                .ExecuteUpdate(s => s.SetProperty(b => b.IsFinished, isFinished))
    );
}

public class TodoRepository: ITodoRepository
{
    private readonly AppDbContext _dbContext;

    public TodoRepository(AppDbContext context)
    {
        _dbContext = context;
    }

    
    public async Task<List<Todolist>> GetByUserId(string userId)
    {
       return await _dbContext.Todolists.AsNoTracking().Where(u => u.UserId == userId).ToListAsync();
    }

    public async Task Add(Todolist todolist)
    {
        await _dbContext.Todolists.AddAsync(todolist);
    }

    public async Task Remove(int id)
    {
        await TodoQuery.RemoveTodolist(_dbContext, id);
    }

    public async Task SetFinished(int id, bool isFinished)
    {
        await TodoQuery.SetFinished(_dbContext, id, isFinished);
    }
}