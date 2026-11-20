using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Rocal.App.Services;
using Rocal.Core.Onboarding;

namespace Rocal.App.ViewModels;

/// <summary>First-run tour shown as an overlay. Completion is persisted so it only appears once.</summary>
public sealed partial class OnboardingViewModel : ObservableObject
{
    private const int StepCount = 5;

    // Discover, Pair, Connect, Share, Play
    private static readonly string[] Glyphs = ["\uE721", "\uE72E", "\uE8BD", "\uE8A5", "\uE7FC"];

    private readonly SettingsService _settings;
    private readonly OnboardingFlow _flow = new(StepCount);

    [ObservableProperty] private bool _isVisible;

    public OnboardingViewModel(LocalizationService loc, SettingsService settings)
    {
        Loc = loc;
        _settings = settings;
        loc.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == "Item[]") Refresh();
        };
        IsVisible = !settings.Current.OnboardingCompleted;
    }

    public LocalizationService Loc { get; }

    public string StepTitle => Loc[$"Onboarding_Step{_flow.CurrentIndex + 1}_Title"];
    public string StepBody => Loc[$"Onboarding_Step{_flow.CurrentIndex + 1}_Body"];
    public string StepGlyph => Glyphs[_flow.CurrentIndex];
    public string NextLabel => Loc[_flow.IsLast ? "Onboarding_Done" : "Onboarding_Next"];
    public bool CanGoBack => !_flow.IsFirst;
    public IReadOnlyList<bool> Dots => Enumerable.Range(0, StepCount).Select(i => i == _flow.CurrentIndex).ToList();

    /// <summary>Re-opens the tour from the first step (used by Settings).</summary>
    public void Show()
    {
        _flow.Reset();
        IsVisible = true;
        Refresh();
    }

    [RelayCommand]
    private void Next()
    {
        if (!IsVisible) return;
        _flow.Next();
        if (_flow.IsFinished) Finish();
        else Refresh();
    }

    [RelayCommand]
    private void Back()
    {
        if (!IsVisible) return;
        _flow.Back();
        Refresh();
    }

    [RelayCommand]
    private void Skip()
    {
        if (!IsVisible) return;
        _flow.Skip();
        Finish();
    }

    private void Finish()
    {
        IsVisible = false;
        if (!_settings.Current.OnboardingCompleted)
            _settings.Update(_settings.Current with { OnboardingCompleted = true });
    }

    private void Refresh() => OnPropertyChanged(string.Empty);
}
