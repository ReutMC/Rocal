using CommunityToolkit.Mvvm.ComponentModel;
using Rocal.App.Services;

namespace Rocal.App.ViewModels;

public sealed partial class MainViewModel : ObservableObject
{
    [ObservableProperty] private PageViewModel _selectedPage;

    public MainViewModel(LocalizationService loc, SettingsService settings)
    {
        Loc = loc;
        Pages =
        [
            new HomeViewModel(loc),
            new ComingSoonPageViewModel(loc, "Nav_Devices", "\uE772", 2),
            new ComingSoonPageViewModel(loc, "Nav_Files", "\uE8A5", 3),
            new ComingSoonPageViewModel(loc, "Nav_Messages", "\uE8BD", 4),
            new ComingSoonPageViewModel(loc, "Nav_Remote", "\uE7F4", 5),
            new SettingsViewModel(loc, settings)
        ];
        _selectedPage = Pages[0];
    }

    public LocalizationService Loc { get; }

    public IReadOnlyList<PageViewModel> Pages { get; }
}
