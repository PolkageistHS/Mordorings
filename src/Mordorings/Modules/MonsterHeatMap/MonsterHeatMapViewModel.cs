using System.Diagnostics.CodeAnalysis;

namespace Mordorings.Modules;

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
        if (!TryGetDetailsForTile(args, out Tile tile, out HeatMapTileDetails? details))
            return;
        SelectedTileDetails = details.AreaSpawnChance != null
            ? $"{tile.X + 1},{tile.Y + 1} - Area {details.AreaNumber}\nChance: {details.AreaSpawnChance.SpawnChance:P2}"
            : null;
    }

    [RelayCommand]
    private void GetImageMouseClick(object? args)
    {
        if (TryGetDetailsForTile(args, out Tile tile, out HeatMapTileDetails? details))
        {
            SpawnRates = details.AreaSpawnChance != null
                ? _mediator.GetSpawnsForTile(tile, CurrentFloorNumber)
                : null;
        }
    }

    private bool TryGetDetailsForTile(object? args, out Tile tile, [NotNullWhen(true)] out HeatMapTileDetails? details)
    {
        details = null;
        tile = AutomapEventConversion.GetMapCoordinatesFromEvent(args);
        if (tile.X is < 0 or >= Game.FloorWidth || tile.Y is < 0 or >= Game.FloorHeight)
            return false;
        details = _mediator.GetTileDetails(tile);
        return true;
    }

    public List<MonsterSubtypeIndexed> MonsterTypes { get; private set; }

    private List<MonsterSpawnRate>? _spawnRates;

    public List<MonsterSpawnRate>? SpawnRates
    {
        get => _spawnRates;
        private set => SetProperty(ref _spawnRates, value);
    }

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
        SpawnRates = null;
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
            SpawnRates = null;
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
