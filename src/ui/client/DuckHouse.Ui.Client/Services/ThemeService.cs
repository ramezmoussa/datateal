using Microsoft.JSInterop;

namespace DuckHouse.Ui.Client.Services;

public sealed class ThemeService(IJSRuntime js) : IThemeService
{
    public event Action? ThemeChanged;

    public async Task<AppTheme> GetThemeAsync()
    {
        var stored = await js.InvokeAsync<string>("getStoredDuckhouseTheme");
        return stored switch
        {
            "dark"  => AppTheme.Dark,
            "light" => AppTheme.Light,
            _       => AppTheme.Auto,
        };
    }

    public async Task SetThemeAsync(AppTheme theme)
    {
        var value = theme switch
        {
            AppTheme.Dark  => "dark",
            AppTheme.Light => "light",
            _              => "auto",
        };
        await js.InvokeVoidAsync("setDuckhouseTheme", value);
        ThemeChanged?.Invoke();
    }
}
