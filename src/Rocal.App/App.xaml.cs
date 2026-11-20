using System.Windows;
using System.Windows.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocal.App.Services;
using Rocal.App.ViewModels;

namespace Rocal.App;

public partial class App : Application
{
    private IHost? _host;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        DispatcherUnhandledException += OnDispatcherUnhandledException;

        _host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddSingleton<SettingsService>();
                services.AddSingleton<LocalizationService>();
                services.AddSingleton<NavigationService>();
                services.AddSingleton<OnboardingViewModel>();
                services.AddSingleton<MainViewModel>();
                services.AddSingleton<MainWindow>();
            })
            .Build();
        await _host.StartAsync();

        // Settings and language must be ready before any view model is created.
        var settings = _host.Services.GetRequiredService<SettingsService>();
        settings.Load();
        _host.Services.GetRequiredService<LocalizationService>().SetLanguage(settings.Current.Language);

        _host.Services.GetRequiredService<MainWindow>().Show();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        if (_host is not null)
        {
            await _host.StopAsync();
            _host.Dispose();
        }
        base.OnExit(e);
    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        _host?.Services.GetService<ILogger<App>>()?.LogError(e.Exception, "Unhandled UI exception");
        MessageBox.Show(e.Exception.Message, "Rocal", MessageBoxButton.OK, MessageBoxImage.Error);
        e.Handled = true;
    }
}
