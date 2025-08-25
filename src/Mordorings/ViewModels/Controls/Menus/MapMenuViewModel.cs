using CommunityToolkit.Mvvm.Messaging;
using Mordorings.Messages;
using Mordorings.Modules.DungeonState;
using Mordorings.Modules.EditMap;

namespace Mordorings.ViewModels;

public partial class MapMenuViewModel(IViewModelFactory factory)
{
    [RelayCommand]
    private void OpenDungeonStateEditor()
    {
        WeakReferenceMessenger.Default.Send(new ViewContentChangedMessage(factory.CreateViewModel<DungeonStateViewModel>()));
    }

    [RelayCommand]
    private void OpenMapEditor()
    {
        WeakReferenceMessenger.Default.Send(new ViewContentChangedMessage(factory.CreateViewModel<EditMapViewModel>()));
    }
}
