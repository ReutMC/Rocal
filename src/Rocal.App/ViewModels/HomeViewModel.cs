using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Rocal.App.Services;
using Rocal.Networking;

namespace Rocal.App.ViewModels;

public sealed partial class HomeViewModel : PageViewModel
{
    private readonly NavigationService _navigation;

    [ObservableProperty] private string _localAddress = "-";
    [ObservableProperty] private bool _isNetworkAvailable;

    public HomeViewModel(LocalizationService loc, NavigationService navigation)
        : base(PageId.Home, loc, "Nav_Home", "\uE80F")
    {
        _navigation = navigation;
        Refresh();
    }

    public string ComputerName => Environment.MachineName;

    public string NetworkStatusText => IsNetworkAvailable ? Loc["Home_Connected"] : Loc["Home_NoNetwork"];

    [RelayCommand]
    private void Refresh()
    {
        var primary = LocalNetworkInfo.GetPrimary();
        IsNetworkAvailable = primary is not null;
        LocalAddress = primary?.Address.ToString() ?? "-";
        OnPropertyChanged(nameof(NetworkStatusText));
    }

    [RelayCommand]
    private void Navigate(PageId id) => _navigation.Navigate(id);
}
