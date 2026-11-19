using CommunityToolkit.Mvvm.ComponentModel;
using Rocal.App.Services;

namespace Rocal.App.ViewModels;

/// <summary>Base class for pages shown in the sidebar. Re-raises all bindings when the language changes.</summary>
public abstract class PageViewModel : ObservableObject
{
    private readonly string _titleKey;

    protected PageViewModel(LocalizationService loc, string titleKey, string glyph)
    {
        Loc = loc;
        _titleKey = titleKey;
        Glyph = glyph;
        loc.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == "Item[]") OnPropertyChanged(string.Empty);
        };
    }

    public LocalizationService Loc { get; }

    /// <summary>Segoe Fluent Icons / Segoe MDL2 Assets glyph.</summary>
    public string Glyph { get; }

    public string Title => Loc[_titleKey];
}
