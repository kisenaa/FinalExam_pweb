using System.Diagnostics;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace Client.Utils;

public class Authentication: AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;
    private readonly HttpClient _http;

    public Authentication(ILocalStorageService localStorage, HttpClient http)
    {
        _localStorage = localStorage;
        _http = http;
    }
    
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        string userId;
        string token;
        try
        {
            userId = (await _localStorage.GetItemAsStringAsync("userId_final_exams"));
            token = (await _localStorage.GetItemAsStringAsync("jwt_final_exams"));
        }
        catch (Exception ex)
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
            
        var identity = new ClaimsIdentity();
        _http.DefaultRequestHeaders.Authorization = null;
        _http.DefaultRequestHeaders.Remove("X-Userid");
        
        if (!string.IsNullOrEmpty(token))
        {
            userId = userId.Trim('\"');
            token = token.Trim('\"');
            identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token.Replace("\"", ""));
            _http.DefaultRequestHeaders.Add("X-Userid", userId);
        }
        
        var state = new AuthenticationState(new ClaimsPrincipal(identity));
        NotifyAuthenticationStateChanged(Task.FromResult(state));
    
        return (state);
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