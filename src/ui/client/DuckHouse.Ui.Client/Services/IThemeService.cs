namespace DuckHouse.Ui.Client.Services;

public enum AppTheme { Light, Dark, Auto }

public interface IThemeService
{
    Task<AppTheme> GetThemeAsync();
    Task SetThemeAsync(AppTheme theme);
    event Action ThemeChanged;
}
