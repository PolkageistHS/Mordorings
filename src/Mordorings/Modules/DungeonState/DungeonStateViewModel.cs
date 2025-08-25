namespace Mordorings.Modules.DungeonState;

public partial class DungeonStateViewModel : ViewModelBase
{
    private readonly IMordorIoFactory _ioFactory;
    private readonly IDialogFactory _dialogFactory;

    public DungeonStateViewModel(IMordorIoFactory ioFactory, IDialogFactory dialogFactory)
    {
        _ioFactory = ioFactory;
        _dialogFactory = dialogFactory;
        MordorRecordReader reader = _ioFactory.GetReader();
        Monsters = new ObservableCollection<Monster>(reader.GetMordorRecord<DATA05Monsters>().MonstersList);
        Model = new DungeonStateModel(Monsters.First());
    }

    [ObservableProperty]
    private bool _spawnMonster;

    [ObservableProperty]
    private bool _spawnTreasure;

    public bool IsReady => SpawnMonster || SpawnTreasure;

    [ObservableProperty]
    private ObservableCollection<Monster> _monsters;

    [ObservableProperty]
    private DungeonStateModel _model;

    [RelayCommand]
    private void WriteMonsterState()
    {
        if (!Model.IsValid())
        {
            _dialogFactory.ShowErrorMessage(string.Join('\n', Model.GetAllErrors().Select(result => result.ErrorMessage)), "Input Error");
            return;
        }
        DATA10DungeonState dungeonState = _ioFactory.GetReader().GetMordorRecord<DATA10DungeonState>();
        DungeonStateProcessor processor = new((short)Monsters.IndexOf(Model.SelectedMonster), Model, dungeonState);
        DATA10DungeonState newState = processor.Process();
        _ioFactory.GetWriter().WriteMordorRecord(newState);
        _dialogFactory.ShowMessage("Dungeon state written.", "Success");
    }

    partial void OnSpawnMonsterChanged(bool value)
    {
        OnPropertyChanged(nameof(IsReady));
    }

    partial void OnSpawnTreasureChanged(bool value)
    {
        OnPropertyChanged(nameof(IsReady));
    }
    public override string Instructions => "Overwrites the current dungeon state to fill every square with the monster/treasure selected. This is not permanent; normal respawn rules still apply.";
}
