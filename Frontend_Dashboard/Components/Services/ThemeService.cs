using Microsoft.JSInterop;

namespace Frontend_Dashboard.Components.Services;

/// <summary>
/// Thin wrapper around <c>wwwroot/js/theme.js</c>.
/// </summary>
public sealed class ThemeService
{
    private const string JS_MODULE = "/js/theme.js";   // path under wwwroot
    private const string FN_GET    = "getTheme";       // export names in theme.js
    private const string FN_TOGGLE = "toggleTheme";

    private readonly Lazy<Task<IJSObjectReference>> _module;

    public ThemeService(IJSRuntime js)
    {
        // defer loading until the first call (saves bandwidth on prerender)
        _module = new(() =>
            js.InvokeAsync<IJSObjectReference>("import", JS_MODULE).AsTask());
    }

    /// <returns><c>true</c> when the current theme is <c>dark</c></returns>
    public async Task<bool> IsDarkAsync()
    {
        var mod   = await _module.Value;
        var theme = await mod.InvokeAsync<string>(FN_GET);
        return theme == "dark";
    }

    /// <returns><c>true</c> when the theme *after* toggling is <c>dark</c></returns>
    public async Task<bool> ToggleAsync()
    {
        var mod   = await _module.Value;
        var theme = await mod.InvokeAsync<string>(FN_TOGGLE);
        return theme == "dark";
    }
}