using CommunityToolkit.Mvvm.ComponentModel;
using Mordorings.ViewModels.Abstractions;

namespace Mordorings.Modules.DungeonState;

public partial class DungeonStateViewModel(IDungeonStateFactory factory) : ViewModelBase
{
    [ObservableProperty]
    private IDungeonStateFactory _factory = factory;

    public override string Instructions => "Overwrites the current dungeon state to fill every square with the monster/treasure selected. This is not permanent; normal respawn rules still apply.";
}
