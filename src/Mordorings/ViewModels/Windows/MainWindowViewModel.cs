using CommunityToolkit.Mvvm.Messaging;
using Mordorings.Messages;

namespace Mordorings.ViewModels;

public partial class MainWindowViewModel : ObservableRecipient
{
    public MainWindowViewModel(IViewModelFactory viewModelFactory)
    {
        _viewModelFactory = viewModelFactory;
        Messenger.Register<MainWindowViewModel, ViewContentChangedMessage>(this, (_, message) => { ViewModel = message.Value; });
    }

    private readonly IViewModelFactory _viewModelFactory;

    [ObservableProperty]
    private IViewModel? _viewModel;

    [ObservableProperty]
    private string? _folder;

    [ObservableProperty]
    private object? _subMenu;

    [RelayCommand]
    private void OpenEditMenu()
    {
        SubMenu = _viewModelFactory.CreateViewModel<EditMenuViewModel>();
    }

    [RelayCommand]
    private void OpenMapMenu()
    {
        SubMenu = _viewModelFactory.CreateViewModel<MapMenuViewModel>();
    }

    [RelayCommand]
    private void OpenCalculationsMenu()
    {
        SubMenu = _viewModelFactory.CreateViewModel<CalculationsMenuViewModel>();
    }
}
