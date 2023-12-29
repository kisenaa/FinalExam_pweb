using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;

namespace Client.Utils;

public class Authentication: AuthenticationStateProvider
{
    private readonly HttpClient _http;
    private string _token = string.Empty;
    private string _userId = string.Empty;
    
    public Authentication(HttpClient http)
    {
        _http = http;
    }

    public void SetAuthProperty(string token, string userId)
    {
        _token = token;
        _userId = userId;
    }
    
    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var identity = new ClaimsIdentity();
        _http.DefaultRequestHeaders.Authorization = null;
        
        if (!string.IsNullOrEmpty(_token))
        {
            identity = new ClaimsIdentity(ParseClaimsFromJwt(_token), "jwt");
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _token.Replace("\"", ""));
            _http.DefaultRequestHeaders.Add("x-userId", _userId);
        }
        
        var state = new AuthenticationState(new ClaimsPrincipal(identity));
        NotifyAuthenticationStateChanged(Task.FromResult(state));
    
        return Task.FromResult(state);
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