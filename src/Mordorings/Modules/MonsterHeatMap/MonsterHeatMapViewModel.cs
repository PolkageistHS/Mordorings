namespace Mordorings.Modules.MonsterHeatMap;

public partial class MonsterHeatMapViewModel : ViewModelBase
{
    private readonly IMonsterHeatMapMediator _mediator;

    public MonsterHeatMapViewModel(IMonsterHeatMapMediator mediator)
    {
        _mediator = mediator;
        MonsterTypes = _mediator.GetMonsterSubtypes();
        _ = LoadData();
    }

    private async Task LoadData()
    {
        try
        {
            IsDataLoaded = false;
            await _mediator.Initialize();
        }
        finally
        {
            IsDataLoaded = true;
        }
    }

    [RelayCommand(CanExecute = nameof(CanIncreaseFloor))]
    private void IncreaseFloor()
    {
        CurrentFloorNumber = _mediator.GetNextValidFloorNumber(CurrentFloorNumber, CurrentFloorNumber + 1);
    }

    protected bool CanIncreaseFloor => _mediator.HasHigherFloor(CurrentFloorNumber);

    [RelayCommand(CanExecute = nameof(CanDecreaseFloor))]
    private void DecreaseFloor()
    {
        CurrentFloorNumber = _mediator.GetNextValidFloorNumber(CurrentFloorNumber, CurrentFloorNumber - 1);
    }

    protected bool CanDecreaseFloor => _mediator.HasLowerFloor(CurrentFloorNumber);

    [RelayCommand]
    private void ShowTileDetails(object? args)
    {
        Tile tile = AutomapEventConversion.GetMapCoordinatesFromEvent(args);
        if (tile.X < 0 || tile.Y < 0)
            return;
        (AreaSpawnChance? spawnChance, int areaNumber) = _mediator.GetTileDetails(tile);
        SelectedTileDetails = spawnChance != null ? $"{tile.X + 1},{tile.Y + 1} - Area {areaNumber}\nChance: {spawnChance.SpawnChance:P2}" : null;
    }

    public List<MonsterSubtypeIndexed> MonsterTypes { get; private set; }

    public ObservableCollection<Monster> Monsters { get; } = [];

    private int _currentFloorNumber;

    public int CurrentFloorNumber
    {
        get => _currentFloorNumber;
        set
        {
            SetProperty(ref _currentFloorNumber, value);
            UpdateSelectedFloor(value);
        }
    }

    private void UpdateSelectedFloor(int value)
    {
        Image = _mediator.GetHeatMapImage(value);
        SelectedTileDetails = null;
        IncreaseFloorCommand.NotifyCanExecuteChanged();
        DecreaseFloorCommand.NotifyCanExecuteChanged();
    }

    private bool _isDataLoaded;

    public bool IsDataLoaded
    {
        get => _isDataLoaded;
        set => SetProperty(ref _isDataLoaded, value);
    }

    private object? _image;

    public object? Image
    {
        get => _image;
        private set => SetProperty(ref _image, value);
    }

    private MonsterSubtypeIndexed? _selectedMonsterType;

    public MonsterSubtypeIndexed? SelectedMonsterType
    {
        get => _selectedMonsterType;
        set
        {
            SetProperty(ref _selectedMonsterType, value);
            RefreshMonsterList(value);
        }
    }

    private void RefreshMonsterList(MonsterSubtypeIndexed? value)
    {
        Monsters.Clear();
        foreach (Monster monster in _mediator.GetMonstersBySubtypeId(value?.Index))
        {
            Monsters.Add(monster);
        }
    }

    private Monster? _selectedMonster;

    public Monster? SelectedMonster
    {
        get => _selectedMonster;
        set
        {
            SetProperty(ref _selectedMonster, value);
            UpdateForSelectedMonster(value);
        }
    }

    private void UpdateForSelectedMonster(Monster? value)
    {
        if (value != null)
        {
            int? floorNum = _mediator.GetFirstFloorForMonster(value);
            if (floorNum != null)
            {
                CurrentFloorNumber = floorNum.Value;
            }
            else
            {
                Image = null;
                SelectedTileDetails = "Cannot spawn as primary.";
            }
        }
        else
        {
            SelectedTileDetails = null;
            Image = null;
        }
    }

    private string? _selectedTileDetails;

    public string? SelectedTileDetails
    {
        get => _selectedTileDetails;
        private set => SetProperty(ref _selectedTileDetails, value);
    }

    public override string Instructions => "See where monsters are capable of spawning and the relative chances of spawning.";
}
