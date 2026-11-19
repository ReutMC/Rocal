using System.Windows;
using Rocal.App.ViewModels;

namespace Rocal.App;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
