namespace Mordorings.Modules;

public partial class DungeonStateViewModel : ViewModelBase
{
    private readonly IDungeonStateMediator _mediator;
    private readonly IDialogFactory _dialogFactory;

    public DungeonStateViewModel(IDungeonStateMediator mediator, IDialogFactory dialogFactory)
    {
        _mediator = mediator;
        _dialogFactory = dialogFactory;
        Monsters = _mediator.GetMonsters().ToList();
        Model = new DungeonStateModel(Monsters.First());
    }

    public bool IsReady => SpawnMonster || SpawnTreasure;

    public List<Monster> Monsters { get; }

    public DungeonStateModel Model { get; }

    private bool _spawnMonster;

    public bool SpawnMonster
    {
        get => _spawnMonster;
        set
        {
            SetProperty(ref _spawnMonster, value);
            OnPropertyChanged(nameof(IsReady));
        }
    }

    private bool _spawnTreasure;

    public bool SpawnTreasure
    {
        get => _spawnTreasure;
        set
        {
            SetProperty(ref _spawnTreasure, value);
            OnPropertyChanged(nameof(IsReady));
        }
    }

    [RelayCommand]
    private void WriteMonsterState()
    {
        if (Model.IsValid())
        {
            _mediator.WriteDungeonState((short)Monsters.IndexOf(Model.SelectedMonster), Model);
            _dialogFactory.ShowMessage("Dungeon state written.", "Success");
        }
        else
        {
            _dialogFactory.ShowErrorMessage(string.Join('\n', Model.GetAllErrors().Select(result => result.ErrorMessage)), "Input Error");
        }
    }

    public override string Instructions => "Overwrites the current dungeon state to fill every square with the monster/treasure selected. This is not permanent; normal respawn rules still apply.";
}
