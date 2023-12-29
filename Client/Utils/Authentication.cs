using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace Client.Utils;

public class Authentication: AuthenticationStateProvider
{
    private readonly HttpClient _http;
    private static string _userId = string.Empty;
    private static string _token = string.Empty;

    public Authentication(HttpClient http)
    {
        _http = http;
    }

    public static void SetAuthenticationProp(string userId, string token)
    {
        _userId = userId;
        _token = token;
    }
    
    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var identity = new ClaimsIdentity();
        _http.DefaultRequestHeaders.Authorization = null;
        _http.DefaultRequestHeaders.Remove("X-Userid");
        
        if (!string.IsNullOrEmpty(_token))
        {
            _userId = _userId.Trim('\"');
            _token = _token.Trim('\"');
            identity = new ClaimsIdentity(ParseClaimsFromJwt(_token), "jwt");
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _token.Replace("\"", ""));
            _http.DefaultRequestHeaders.Add("X-Userid", _userId);
        }
        
        var state = new AuthenticationState(new ClaimsPrincipal(identity));
        NotifyAuthenticationStateChanged(Task.FromResult(state));
    
        return Task.FromResult((state));
    }

    private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var payload = jwt.Split('.')[1];
        ReadOnlySpan<byte> jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
        if (keyValuePairs != null) 
            return keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString() ?? string.Empty));
        return new List<Claim>();
    }
    private static ReadOnlySpan<byte> ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }
}