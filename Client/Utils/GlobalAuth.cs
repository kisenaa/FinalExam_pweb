public class GlobalAuth
{
    public string JwtToken { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    
    public event EventHandler<EventArgs>? HasChanged;
    
    public void SetProperties(string jwtToken, string userId)
    {
        JwtToken = jwtToken;
        UserId = userId;
        HasChanged?.Invoke(this, EventArgs.Empty);
    }
}