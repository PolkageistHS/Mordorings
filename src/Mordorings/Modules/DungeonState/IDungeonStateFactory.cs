using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using MordorDataLibrary.Models;

namespace Mordorings.Modules.DungeonState;

public interface IDungeonStateFactory
{
    bool SpawnMonster { get; set; }

    bool SpawnTreasure { get; set; }

    ObservableCollection<Monster> Monsters { get; }

    DungeonStateModel Model { get; }

    IRelayCommand WriteMonsterStateCommand { get; }

    bool IsReady { get; }
}
