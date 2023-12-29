namespace Server.Data.Repository;

public interface IRepository
{
    public ITokenRepository RefreshToken { get; }
    public IUserRepository Users { get; }
    public ITodoRepository Todolist { get; }
    public Task SaveChangesAsync();
    void Dispose();
}

public class Repository: IRepository, IDisposable
{
    private readonly AppDbContext _dbContext;
    public ITokenRepository RefreshToken { get; }
    public IUserRepository Users { get; }
    public ITodoRepository Todolist { get; }

    public Repository(AppDbContext context)
    {
        _dbContext = context;
        RefreshToken = new TokenRepository(_dbContext);
        Users = new UserRepository(_dbContext);
        Todolist = new TodoRepository(_dbContext);
    }
    
    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}