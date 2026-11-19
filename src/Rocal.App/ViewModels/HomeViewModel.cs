using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Rocal.App.Services;
using Rocal.Networking;

namespace Rocal.App.ViewModels;

public sealed partial class HomeViewModel : PageViewModel
{
    [ObservableProperty] private string _localAddress = "-";
    [ObservableProperty] private bool _isNetworkAvailable;

    public HomeViewModel(LocalizationService loc) : base(loc, "Nav_Home", "\uE80F")
    {
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
}
