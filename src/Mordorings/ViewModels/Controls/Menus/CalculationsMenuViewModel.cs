using CommunityToolkit.Mvvm.Messaging;
using Mordorings.Messages;
using Mordorings.Modules.GuildSpells;
using Mordorings.Modules.ReqsForLevel;

namespace Mordorings.ViewModels;

public partial class CalculationsMenuViewModel(IViewModelFactory factory)
{
    [RelayCommand]
    private void OpenReqsForLevel()
    {
        WeakReferenceMessenger.Default.Send(new ViewContentChangedMessage(factory.CreateViewModel<LevelRequirementsViewModel>()));
    }

    [RelayCommand]
    private void OpenSpellStats()
    {
        WeakReferenceMessenger.Default.Send(new ViewContentChangedMessage(factory.CreateViewModel<GuildSpellsViewModel>()));
    }
}
