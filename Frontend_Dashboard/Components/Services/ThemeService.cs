using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace Frontend_Dashboard.Components.Services;

public sealed class ThemeService
{
    private readonly IJSRuntime _js;
    public ThemeService(IJSRuntime js) => _js = js;
    
    public ValueTask ToggleAsync() => _js.InvokeVoidAsync("toggleTheme");
    public ValueTask SetAsync(string theme) => _js.InvokeVoidAsync("setTheme", theme);
}