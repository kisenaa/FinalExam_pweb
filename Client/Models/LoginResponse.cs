// ReSharper disable InconsistentNaming
namespace Client.Models;

public class LoginResponse
{
    public string token { get; set; } = string.Empty;
    public string userId { get; set; } = string.Empty;
}