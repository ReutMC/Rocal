using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Windows;

namespace Rocal.App.Services;

/// <summary>Loads UI strings from embedded Strings.&lt;code&gt;.json files and exposes them via an indexer for XAML bindings.</summary>
public sealed class LocalizationService : INotifyPropertyChanged
{
    public const string English = "en";
    public const string Persian = "fa";

    private readonly Dictionary<string, string> _fallback;
    private Dictionary<string, string> _current;

    public LocalizationService()
    {
        _fallback = Load(English);
        _current = _fallback;
        Language = English;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Language { get; private set; }

    public FlowDirection FlowDirection => Language == Persian ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

    public string this[string key] =>
        _current.TryGetValue(key, out var value) ? value :
        _fallback.TryGetValue(key, out var fallback) ? fallback :
        $"[{key}]";

    public void SetLanguage(string code)
    {
        if (code != English && code != Persian) code = English;

        _current = code == English ? _fallback : Load(code);
        Language = code;

        var culture = new CultureInfo(code == Persian ? "fa-IR" : "en-US");
        CultureInfo.DefaultThreadCurrentUICulture = culture;
        CultureInfo.CurrentUICulture = culture;

        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Language)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FlowDirection)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));
    }

    private static Dictionary<string, string> Load(string code)
    {
        var name = $"Strings.{code}.json";
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name)
                           ?? throw new FileNotFoundException($"Missing embedded language resource '{name}'.");
        return JsonSerializer.Deserialize<Dictionary<string, string>>(stream) ?? new();
    }
}
