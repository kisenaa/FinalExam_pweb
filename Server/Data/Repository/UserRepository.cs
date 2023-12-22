using Microsoft.EntityFrameworkCore;
using Server.Models.Auth;
using Server.Models.Database;

namespace Server.Data.Repository;

public interface IUserRepository
{
    Task<Login?> HashFromEmail (string email);
    Task<Login?> HashFromUsername (string username);
    Task Add(User user);
    void Remove(User user);
}

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _dbContext;

    public UserRepository(AppDbContext context)
    {
        _dbContext = context;
    }

    private static readonly Func<AppDbContext, string, Task<Login?>> HashFromEmails = EF.CompileAsyncQuery(
        (AppDbContext context, string email) =>
            context.Users
                .AsNoTracking()
                .Where(u => u.Email == email)
                .Select(u => new Login
                {
                    UserId = u.UserId,
                    PasswordHash = u.PasswordHash
                })
                .FirstOrDefault()
    );
    
    private static readonly  Func<AppDbContext, string, Task<Login?>> HashFromUsernames = EF.CompileAsyncQuery(
        (AppDbContext context, string username) =>
            context.Users
                .AsNoTracking()
                .Where(u => u.Username == username)
                .Select(u => new Login
                {
                    UserId = u.UserId,
                    PasswordHash = u.PasswordHash
                })
                .FirstOrDefault() 
    );

    private static readonly Func<AppDbContext, string, Task<User?>> GetUser = EF.CompileAsyncQuery(
        (AppDbContext context, string userId) =>
            context.Users
                .AsNoTracking()
                .FirstOrDefault(u => u.UserId == userId)
    );

    public async Task<Login?> HashFromEmail (string email)
    {
        return await HashFromEmails(_dbContext, email);
    }
    
    public async Task<Login?> HashFromUsername (string username)
    {
        return await HashFromUsernames(_dbContext, username);
    }
    
    public async Task Add(User user)
    {
        await _dbContext.Users.AddAsync(user);
    }

    public void Remove(User user)
    { 
        _dbContext.Entry(user).State = EntityState.Deleted;
    }
    
}