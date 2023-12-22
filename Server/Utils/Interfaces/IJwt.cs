using System.Security.Claims;

namespace Server.Utils.Interfaces;

public interface IJwt
{
    string CreateToken(string userId);
    ClaimsPrincipal? ExpiredTokenInfo(string token);
}