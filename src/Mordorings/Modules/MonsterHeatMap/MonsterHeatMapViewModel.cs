using System.Drawing;

namespace Mordorings.Modules.MonsterHeatMap;

public partial class MonsterHeatMapViewModel : ViewModelBase
{
    private const string SpriteSheetFile = "Assets/DungeonSprites.bmp";

    private List<MonsterSpawnRates> _cachedMonsterSpawnRates = [];

    private readonly Floor[] _floors;
    private readonly IHeatmapRenderer _heatmapRenderer;
    private readonly IMonsterHeatMapMediator _mediator;
    private readonly IMapRendererFactory _mapRendererFactory;
    private readonly Dictionary<int, Bitmap?> _cachedMaps = [];
    private readonly List<MonsterHeatmapFloor> _cachedFloors = [];

    public MonsterHeatMapViewModel(IMapRendererFactory mapRendererFactory, IMonsterHeatMapMediator mediator)
    {
        _mapRendererFactory = mapRendererFactory;
        _mediator = mediator;
        _heatmapRenderer = _mapRendererFactory.CreateHeatmapRenderer();
        _floors = _mediator.GetFloors();
        _ = LoadData();
    }

    private async Task LoadData()
    {
        try
        {
            MonsterTypes = _mediator.GetMonsterSubtypes();
            InitializeRenderers();
            IsDataLoaded = false;
            await Task.Run(() => { _cachedMonsterSpawnRates = _mediator.GetAllMonsterSpawns(); });
        }
        finally
        {
            IsDataLoaded = true;
        }
    }

    private void InitializeRenderers()
    {
        for (int i = 0; i < _floors.Length; i++)
        {
            var dungeonFloor = new DungeonFloor(_floors[i]);
            IAutomapRenderer renderer = _mapRendererFactory.CreateAutomapRenderer();
            renderer.LoadSpriteSheet(SpriteSheetFile);
            renderer.Initialize(dungeonFloor);
            renderer.DrawDungeonFloorMap();
            _cachedMaps[i] = renderer.GetMapSnapshot();
        }
    }

    [RelayCommand(CanExecute = nameof(CanIncreaseFloor))]
    private void IncreaseFloor()
    {
        SelectedFloorNum = NormalizeFloorNum(SelectedFloorNum, SelectedFloorNum + 1);
    }

    [RelayCommand(CanExecute = nameof(CanDecreaseFloor))]
    private void DecreaseFloor()
    {
        SelectedFloorNum = NormalizeFloorNum(SelectedFloorNum, SelectedFloorNum - 1);
    }

    [RelayCommand]
    private void ShowTileDetails(object? args)
    {
        Tile tile = AutomapEventConversion.GetMapCoordinatesFromEvent(args);
        if (tile.X < 0 || tile.Y < 0)
            return;
        if (SelectedFloor is null)
            return;
        int area = SelectedFloor.DungeonFloor.GetAreaFromTile(tile);
        AreaSpawnChance? spawn = SelectedFloor.SpawnRates.FirstOrDefault(chance => chance.AreaNum == area);
        SelectedTileDetails = spawn != null ? $"{tile.X + 1},{tile.Y + 1} - Area {area}\nChance: {spawn.SpawnChance:P2}" : null;
    }

    public List<MonsterSubtypeIndexed> MonsterTypes { get; private set; } = [];

    public ObservableCollection<Monster> Monsters { get; } = [];

    private int _selectedFloorNum;

    public int SelectedFloorNum
    {
        get => _selectedFloorNum;
        set
        {
            SetProperty(ref _selectedFloorNum, value);
            HandleOnSelectedFloorNumChanged(value);
            IncreaseFloorCommand.NotifyCanExecuteChanged();
            DecreaseFloorCommand.NotifyCanExecuteChanged();
        }
    }

    [ObservableProperty]
    private bool _isDataLoaded;

    [ObservableProperty]
    private object? _image;

    [ObservableProperty]
    private MonsterSubtypeIndexed? _selectedMonsterType;

    [ObservableProperty]
    private Monster? _selectedMonster;

    [ObservableProperty]
    private MonsterHeatmapFloor? _selectedFloor;

    [ObservableProperty]
    private string? _selectedTileDetails;

    partial void OnSelectedMonsterTypeChanged(MonsterSubtypeIndexed? value)
    {
        Monsters.Clear();
        foreach (Monster monster in _mediator.GetMonstersBySubtypeId(value?.Index))
        {
            Monsters.Add(monster);
        }
    }

    partial void OnSelectedMonsterChanged(Monster? value)
    {
        if (value != null)
        {
            ProcessMonsterSpawnChances(value);
        }
        else
        {
            SelectedTileDetails = null;
            Image = null;
        }
    }

    private void ProcessMonsterSpawnChances(Monster value)
    {
        IOrderedEnumerable<IGrouping<int, AreaSpawnChance>>? spawnChances = _cachedMonsterSpawnRates.FirstOrDefault(rates => Equals(rates.Monster, value))
                                                                                                    ?.SpawnRates
                                                                                                    .OrderByDescending(chance => chance.SpawnChance)
                                                                                                    .GroupBy(chance => chance.Floor)
                                                                                                    .OrderBy(grouping => grouping.Key);
        _cachedFloors.Clear();
        if (spawnChances != null)
        {
            foreach (IGrouping<int, AreaSpawnChance> grouping in spawnChances)
            {
                Bitmap? bitmap = _cachedMaps[grouping.Key - 1];
                if (bitmap == null)
                    continue;
                _cachedFloors.Add(new MonsterHeatmapFloor(grouping.Key, _floors[grouping.Key - 1], bitmap, grouping.ToList()));
            }
            SelectedFloorNum = _cachedFloors.OrderBy(floor => floor.FloorNum).First().FloorNum;
        }
        else
        {
            Image = null;
            SelectedTileDetails = "Cannot spawn as primary.";
        }
    }

    protected bool CanIncreaseFloor
    {
        get
        {
            if (_cachedFloors.Count == 0)
                return false;
            return _cachedFloors.Select(floor => floor.FloorNum).Max() > SelectedFloorNum;
        }
    }

    protected bool CanDecreaseFloor
    {
        get
        {
            if (_cachedFloors.Count == 0)
                return false;
            return _cachedFloors.Select(floor => floor.FloorNum).Min() < SelectedFloorNum;
        }
    }

    private void HandleOnSelectedFloorNumChanged(int newValue)
    {
        SelectedFloor = _cachedFloors.FirstOrDefault(floor => floor.FloorNum == newValue);
        _heatmapRenderer.Render(SelectedFloor);
        Image = _heatmapRenderer.GetMapSnapshot()?.ToBitmapSource();
        SelectedTileDetails = null;
    }

    private int NormalizeFloorNum(int oldValue, int newValue)
    {
        if (oldValue == newValue)
            return newValue;
        int[] floors = _cachedFloors.Select(floor => floor.FloorNum).OrderBy(i => i).ToArray();
        if (floors.Contains(newValue))
            return newValue;
        int maxFloor = floors.Last();
        if (newValue > maxFloor)
            return maxFloor;
        int minFloor = floors.First();
        if (newValue < minFloor)
            return minFloor;
        if (oldValue > newValue)
            return floors.Last(floor => floor < newValue);
        return floors.First(floor => floor > newValue);
    }

    public override string Instructions => "See where monsters are capable of spawning and the relative chances of spawning.";
}
