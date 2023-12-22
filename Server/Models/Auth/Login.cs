namespace Server.Models.Auth;

public class Login
{
    public string? UserId { get; set; }
    public Memory<byte>? PasswordHash { get; set; }
}

public class LoginContext
{
    public string? UserId { get; set; }
    public string? Role { get; set; }
}