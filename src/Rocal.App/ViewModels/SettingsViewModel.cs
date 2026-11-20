using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Rocal.App.Services;

namespace Rocal.App.ViewModels;

public sealed record LanguageOption(string Code, string Name);

public sealed partial class SettingsViewModel : PageViewModel
{
    private readonly SettingsService _settings;
    private readonly OnboardingViewModel _onboarding;

    [ObservableProperty] private LanguageOption _selectedLanguage;

    public SettingsViewModel(LocalizationService loc, SettingsService settings, OnboardingViewModel onboarding)
        : base(PageId.Settings, loc, "Nav_Settings", "\uE713")
    {
        _settings = settings;
        _onboarding = onboarding;
        Languages =
        [
            new LanguageOption(LocalizationService.English, "English"),
            new LanguageOption(LocalizationService.Persian, "فارسی")
        ];
        _selectedLanguage = Languages.First(l => l.Code == loc.Language);
    }

    public IReadOnlyList<LanguageOption> Languages { get; }

    partial void OnSelectedLanguageChanged(LanguageOption value)
    {
        if (value is null) return;
        _settings.Update(_settings.Current with { Language = value.Code });
        Loc.SetLanguage(value.Code);
    }

    [RelayCommand]
    private void ShowTour() => _onboarding.Show();
}
