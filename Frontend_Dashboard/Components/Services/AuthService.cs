using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Frontend_Dashboard.Components.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Frontend_Dashboard.Components.Services;

/// <summary>
/// Very small client-side auth helper.
///  • Sends login / register requests to the backend.
///  • Stores the JWT in localStorage so it survives a page refresh.
///  • Exposes basic flags you can bind against (IsAuthenticated, Email …).
/// </summary>
public sealed class AuthService
{
    private const string TOKEN_KEY = "jwt";

    private readonly HttpClient       _http;
    private readonly IJSRuntime       _js;
    private readonly NavigationManager _nav;

    public string? Jwt    { get; private set; }
    public int?    UserId { get; private set; }
    public string? Email  { get; private set; }
    public bool IsAuthenticated => Jwt is not null;

    public AuthService(HttpClient http, IJSRuntime js, NavigationManager nav)
    {
        _http = http;
        _js   = js;
        _nav  = nav;
    }

    public async Task<bool> LoginAsync(string email, string password,
                                       CancellationToken ct = default)
    {
        var dto  = new LoginDto { email = email, password = password };
        var resp = await _http.PostAsJsonAsync("api/Auth/login", dto, ct);

        if (!resp.IsSuccessStatusCode) return false;

        var auth = await resp.Content.ReadFromJsonAsync<AuthResponse>(ct);
        await CacheAndApplyAsync(auth);
        return true;
    }

    public async Task<string?> RegisterAsync(string email, string password,
                                             CancellationToken ct = default)
    {
        var dto  = new RegisterDto { email = email, password = password };
        var resp = await _http.PostAsJsonAsync("api/Auth/register", dto, ct);

        if (resp.StatusCode == System.Net.HttpStatusCode.Conflict)
            return "E-mail already registered";

        if (!resp.IsSuccessStatusCode)
            return $"Server error ({(int)resp.StatusCode})";

        var auth = await resp.Content.ReadFromJsonAsync<AuthResponse>(ct);
        await CacheAndApplyAsync(auth);
        return null;
    }

    public async Task LoadFromStorageAsync()
    {
        Jwt = await _js.InvokeAsync<string?>("localStorage.getItem", TOKEN_KEY);
        if (!string.IsNullOrWhiteSpace(Jwt))
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", Jwt);
    }

    public async Task LogoutAsync()
    {
        Jwt    = null;
        UserId = null;
        Email  = null;

        _http.DefaultRequestHeaders.Authorization = null;
        await _js.InvokeVoidAsync("localStorage.removeItem", TOKEN_KEY);
        _nav.NavigateTo("/");
    }

    // -------------------------------------------------- helpers
    private async Task CacheAndApplyAsync(AuthResponse? auth)
    {
        if (auth is null) return;

        Jwt    = auth.token;
        UserId = auth.userId;
        Email  = auth.email;

        // attach to every future request
        _http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", Jwt);

        // persist across reloads
        await _js.InvokeVoidAsync("localStorage.setItem", TOKEN_KEY, Jwt);
    }
}
