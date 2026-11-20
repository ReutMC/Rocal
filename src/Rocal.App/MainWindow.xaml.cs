using System.Windows;
using System.Windows.Input;
using Rocal.App.ViewModels;

namespace Rocal.App;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;

        // Esc closes the first-run tour. Added in code because InputBindings do not inherit DataContext reliably.
        InputBindings.Add(new KeyBinding(viewModel.Onboarding.SkipCommand, Key.Escape, ModifierKeys.None));
    }
}
