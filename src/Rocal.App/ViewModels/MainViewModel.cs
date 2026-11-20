using CommunityToolkit.Mvvm.ComponentModel;
using Rocal.App.Services;

namespace Rocal.App.ViewModels;

public sealed partial class MainViewModel : ObservableObject
{
    [ObservableProperty] private PageViewModel _selectedPage;

    public MainViewModel(
        LocalizationService loc,
        SettingsService settings,
        NavigationService navigation,
        OnboardingViewModel onboarding)
    {
        Loc = loc;
        Onboarding = onboarding;
        Pages =
        [
            new HomeViewModel(loc, navigation),
            new ComingSoonPageViewModel(PageId.Devices, loc, "Nav_Devices", "\uE772", 2),
            new ComingSoonPageViewModel(PageId.Files, loc, "Nav_Files", "\uE8A5", 3),
            new ComingSoonPageViewModel(PageId.Messages, loc, "Nav_Messages", "\uE8BD", 4),
            new ComingSoonPageViewModel(PageId.Remote, loc, "Nav_Remote", "\uE7F4", 5),
            new ComingSoonPageViewModel(PageId.Games, loc, "Nav_Games", "\uE7FC", 6),
            new SettingsViewModel(loc, settings, onboarding)
        ];
        _selectedPage = Pages[0];

        navigation.NavigationRequested += id =>
        {
            var page = Pages.FirstOrDefault(p => p.Id == id);
            if (page is not null) SelectedPage = page;
        };
    }

    public LocalizationService Loc { get; }

    public OnboardingViewModel Onboarding { get; }

    public IReadOnlyList<PageViewModel> Pages { get; }
}
