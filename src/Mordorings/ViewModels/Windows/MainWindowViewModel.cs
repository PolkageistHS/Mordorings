using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Mordorings.Factories;
using Mordorings.Messages;
using Mordorings.ViewModels.Abstractions;

namespace Mordorings.ViewModels;

public partial class MainWindowViewModel : ObservableRecipient
{
    /// <inheritdoc/>
    public MainWindowViewModel(IViewModelFactory viewModelFactory)
    {
        _viewModelFactory = viewModelFactory;
        Messenger.Register<MainWindowViewModel, ViewContentChangedMessage>(this, (_, message) => { ViewModel = message.Value; });
    }

    [ObservableProperty]
    private IViewModel? _viewModel;

    [ObservableProperty]
    private string? _folder;

    [ObservableProperty]
    private object? _subMenu;

    private readonly IViewModelFactory _viewModelFactory;

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

    [RelayCommand]
    private void OpenSimulationsMenu()
    {
        SubMenu = _viewModelFactory.CreateViewModel<SimulationsMenuViewModel>();
    }
}
