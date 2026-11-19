using System.Globalization;
using System.Text.Json;

namespace Rocal.App.Services;

public sealed record AppSettings
{
    public string Language { get; init; } = LocalizationService.English;
}

/// <summary>Persists non-sensitive application settings to %AppData%\Rocal\settings.json.</summary>
public sealed class SettingsService
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    private readonly string _path = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Rocal", "settings.json");

    public AppSettings Current { get; private set; } = new();

    public void Load()
    {
        try
        {
            if (File.Exists(_path))
            {
                Current = JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(_path)) ?? new AppSettings();
                return;
            }
        }
        catch (Exception ex) when (ex is IOException or JsonException or UnauthorizedAccessException)
        {
            // Corrupt or unreadable settings must never prevent the app from starting.
        }

        var system = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        Current = new AppSettings { Language = system == LocalizationService.Persian ? LocalizationService.Persian : LocalizationService.English };
    }

    public void Update(AppSettings settings)
    {
        Current = settings;
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
            File.WriteAllText(_path, JsonSerializer.Serialize(settings, JsonOptions));
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            // Keep running with in-memory settings if the disk is unavailable.
        }
    }
}
