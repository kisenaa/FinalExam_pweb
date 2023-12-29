using System.Security.Claims;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Data;
using Server.Data.Repository;
using Server.Models.Auth;
using Server.Models.Database;
using Server.Models.Requests;
using Server.Utils;

namespace Server.Controllers;

[ApiController]
[Route("api/user/")]
public class UserController: BaseController
{
    private static readonly int Iterations = 2;
    private static readonly int MemoryCost = 8192; // Kilo Bytes Format
    private static readonly Argon Argon = new Argon(Iterations, MemoryCost);
    private static readonly Regex Regex = new Regex(@"\A[a-z0-9!#$%&'*+\/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+\/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\z");
    private readonly JWT _jwt;
    
    public UserController(AppDbContext dbContext, IRepository repository,  IConfiguration config) : base(dbContext, repository)
    {
        _jwt = new JWT(config.GetSection("Jwt:Key").Value!, config.GetSection("Jwt:Issuer").Value!, config.GetSection("Jwt:Audience").Value!);
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] UserRegister userRegister)
    {
        var encryptTask = Task.Run(() => Argon.EncryptPassword(userRegister.Password));
        
        var userData = new User()
        {
            UserId = KSUID.NewKsuid(),
            Username = userRegister.Username,
            Email = userRegister.Email,
            PasswordHash = await encryptTask
        };
        try
        {
            await _repository.Users.Add(userData);
            await _repository.SaveChangesAsync();
            return Ok("success");
        }
        catch (Exception e)
        {
            return StatusCode(500, new { Message = "An error occurred while processing your request.", Error = e});
        }
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login(UserLogin userLogin)
    {
        string userId;
        if (Regex.IsMatch(userLogin.IdentityInput))
        {
            var user = await _repository.Users.HashFromEmail(userLogin.IdentityInput);
            if (user == null)
            {
                return BadRequest("Email Not Found !");
            }

            if (!Argon.VerifyPassword(userLogin.Password, user.PasswordHash!.Value.Span))
            {
                return BadRequest("Wrong Password");
            }
            userId = user.UserId!;
        }
        else
        {
            var user = await _repository.Users.HashFromUsername(userLogin.IdentityInput);
            if (user == null)
            {
                return BadRequest("Username Not Found !");
            }

            if (!Argon.VerifyPassword(userLogin.Password, user.PasswordHash!.Value.Span))
            {
                return BadRequest("Wrong Password");
            }
            userId = user.UserId!;
        }
        var cookie = Request.Cookies["refreshToken_Final_Exam"];
        
        if (cookie == null)
        {
            // Generate and Add refresh token in database
            var refreshToken = JWT.GenerateRefreshToken(userId);
            var addRefreshToken = _repository.RefreshToken.Add(refreshToken);

            SetRefreshToken(refreshToken.Expires, refreshToken.Token);
            var newToken = _jwt.CreateToken(userId);

            await addRefreshToken;
            await _repository.SaveChangesAsync();

            return Ok(new { token = newToken, userId });
        }
        string token = _jwt.CreateToken(userId);

        return Ok(new{token, userId});
    }

    [HttpPost("RefreshToken")]
    public async Task<IActionResult> RefreshToken()
    {
        var refreshToken = Request.Cookies["refreshToken_Final_Exam"];
        if (refreshToken == null)
        {
            return Unauthorized("Can not find refresh token cookies");
        }

        var oldToken = await _repository.RefreshToken.GetRefreshToken(refreshToken);
        if (oldToken == null)
        {
            return Unauthorized("Invalid refresh token cookies");
        } 
        else if (oldToken.Expires < DateTime.UtcNow)
        {
            _repository.RefreshToken.Remove(oldToken);
            await _repository.SaveChangesAsync();
            return Unauthorized("Refresh token expired");
        }
        
        _repository.RefreshToken.Remove(oldToken);
        await _repository.SaveChangesAsync();

        var newRefreshToken = JWT.GenerateRefreshToken(oldToken.UserId);
        var addNewToken = _repository.RefreshToken.Add(newRefreshToken);
        
        SetRefreshToken(newRefreshToken.Expires, newRefreshToken.Token);
        var newToken = _jwt.CreateToken(newRefreshToken.UserId);

        await addNewToken;
        await _repository.SaveChangesAsync();

        return Ok(newToken);
    }
    
    [HttpGet("GetRoles")]
    [Authorize(Roles = "Base")]
    public IActionResult GetRoles()
    {
        var currentUser = GetCurrentUser();
        
        return Ok($"Hi {currentUser!.UserId}, you are an {currentUser.Role}");
    }
    
    private void SetRefreshToken(DateTime expires, string token )
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = expires
        };
        Response.Cookies.Append("refreshToken_Final_Exam", token, cookieOptions);
    }
    
    private LoginContext? GetCurrentUser()
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        if (identity != null)
        {
            var userClaims = identity.Claims;
            return new LoginContext
            {
                UserId = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.PrimarySid)?.Value,
                Role = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Role)?.Value
            };
        }
        return null;
    }
}