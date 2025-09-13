using CommunityToolkit.Mvvm.Messaging;
using Mordorings.Messages;

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
        WeakReferenceMessenger.Default.Send(new ViewContentChangedMessage(factory.CreateViewModel<MapEditorViewModel>()));
    }

    [RelayCommand]
    private void OpenMonsterHeatMap()
    {
        WeakReferenceMessenger.Default.Send(new ViewContentChangedMessage(factory.CreateViewModel<MonsterHeatMapViewModel>()));
    }
}
