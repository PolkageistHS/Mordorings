using Mordorings.ViewModels;

namespace Mordorings.Windows;

public partial class MainWindow
{
    public MainWindow(MainWindowViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
