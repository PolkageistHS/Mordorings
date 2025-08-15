using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Mordorings.Factories;
using Mordorings.Messages;

namespace Mordorings.ViewModels;

public partial class MapMenuViewModel
{
    private readonly IViewModelFactory _factory;

    public MapMenuViewModel(IViewModelFactory factory)
    {
        _factory = factory;
    }

    [RelayCommand]
    private void OpenDungeonStateEditor()
    {
        WeakReferenceMessenger.Default.Send(new ViewContentChangedMessage(_factory.CreateViewModel<Modules.DungeonState.DungeonStateViewModel>()));
    }
}
