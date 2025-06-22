using Microsoft.JSInterop;

namespace Frontend_Dashboard.Components.Services;

public sealed class ThemeService
{
    private readonly Lazy<Task<IJSObjectReference>> _mod;

    public ThemeService(IJSRuntime js)
        => _mod = new(() => js.InvokeAsync<IJSObjectReference>(
            "import", "/js/theme.js").AsTask());

    public async Task<bool> IsDarkAsync()
    {
        var m = await _mod.Value;
        var mode = await m.InvokeAsync<string>("getTheme");
        return mode == "dark";
    }
    public async Task<bool> ToggleAsync()
    {
        var m = await _mod.Value;
        var mode = await m.InvokeAsync<string>("toggleTheme");
        return mode == "dark";
    }
}