using Rocal.App.ViewModels;

namespace Rocal.App.Services;

/// <summary>Lets any view model ask the shell to switch to another page.</summary>
public sealed class NavigationService
{
    public event Action<PageId>? NavigationRequested;

    public void Navigate(PageId id) => NavigationRequested?.Invoke(id);
}
