using Microsoft.EntityFrameworkCore;
using Server.Models.Database;

namespace Server.Data.Repository;

public interface ITokenRepository
{
    Task<RefreshToken?> GetRefreshToken(string refreshToken);
    Task Add(RefreshToken refreshToken);
    void Remove(RefreshToken refreshToken);
}

public class TokenRepository : ITokenRepository
{
    private readonly AppDbContext _dbContext;

    public TokenRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    private static readonly Func<AppDbContext, string, Task<RefreshToken?>> GetRefreshTokens =
        EF.CompileAsyncQuery(
            (AppDbContext context, string refreshToken) =>
                context.RefreshTokens
                    .AsNoTracking()
                    .FirstOrDefault(u => u.Token == refreshToken));

    public async Task<RefreshToken?> GetRefreshToken(string refreshToken)
    {
        return await GetRefreshTokens(_dbContext, refreshToken);
    }

    public async Task Add(RefreshToken refreshToken)
    {
        await _dbContext.RefreshTokens.AddAsync(refreshToken);
    }
    
    public void Remove(RefreshToken refreshToken)
    {
        _dbContext.Entry(refreshToken).State = EntityState.Deleted;
    }
    
}