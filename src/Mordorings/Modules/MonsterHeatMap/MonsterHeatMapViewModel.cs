using System.Drawing;
using Mordorings.Controls;

namespace Mordorings.Modules.MonsterHeatMap;

public partial class MonsterHeatMapViewModel : MapViewModelBase
{
    private List<MonsterSpawnRates> _cachedMonsterSpawnRates = [];
    private readonly IHeatmapRenderer _heatmapRenderer;
    private readonly MordorRecordReader _reader;
    private readonly Dictionary<int, Bitmap?> _cachedMaps = [];
    private readonly List<MonsterHeatmapFloor> _cachedFloors = [];

    public MonsterHeatMapViewModel(IMordorIoFactory ioFactory, IMapRenderFactory mapRenderFactory, IHeatmapRenderer heatmapRenderer) : base(ioFactory, mapRenderFactory)
    {
        _heatmapRenderer = heatmapRenderer;
        _reader = IoFactory.GetReader();
        MonsterTypes = _reader.GetMordorRecord<DATA01GameData>().GetIndexedMonsterSubtypes().ToList();
        Tooltips.TooltipLocationChanged += SetTooltip;
        _ = LoadData();
    }

    private async Task LoadData()
    {
        try
        {
            IsDataLoaded = false;
            await Task.Run(() =>
            {
                for (int i = 0; i < Renderers.Length; i++)
                {
                    _cachedMaps[i] = Renderers[i].GetMapSnapshot();
                }
                _cachedMonsterSpawnRates = new MonsterSpawnCalculator(_reader).GetAllMonsterSpawns();
            });
        }
        finally
        {
            IsDataLoaded = true;
        }
    }

    private void SetTooltip(object? sender, TooltipChangedEventArgs e)
    {
        if (SelectedFloor is null)
            return;
        int area = SelectedFloor.DungeonFloor.GetAreaFromTile(e.Tile);
        AreaSpawnChance? spawn = SelectedFloor.SpawnRates.FirstOrDefault(chance => chance.AreaNum == area);
        string? text;
        if (spawn != null)
        {
            text = $"{e.Tile.X + 1},{e.Tile.Y + 1} - Area {area}\nChance: {spawn.SpawnChance:P2}";
        }
        else
        {
            text = "";
        }
        e.TooltipText = text;
    }

    public List<MonsterSubtypeIndexed> MonsterTypes { get; }

    public ObservableCollection<Monster> Monsters { get; } = [];

    [ObservableProperty]
    private MonsterSubtypeIndexed? _selectedMonsterType;

    [ObservableProperty]
    private Monster? _selectedMonster;

    [ObservableProperty]
    private bool _isDataLoaded;

    [ObservableProperty]
    private MonsterHeatmapFloor? _selectedFloor;

    partial void OnSelectedMonsterTypeChanged(MonsterSubtypeIndexed? value)
    {
        Monsters.Clear();
        foreach (Monster monster in _reader.GetMordorRecord<DATA05Monsters>().GetMonstersBySubtypeId(value?.Index))
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
    }

    private void ProcessMonsterSpawnChances(Monster value)
    {
        IOrderedEnumerable<IGrouping<int, AreaSpawnChance>> spawnChances = _cachedMonsterSpawnRates.First(rates => Equals(rates.Monster, value))
                                                                                                   .SpawnRates
                                                                                                   .OrderByDescending(chance => chance.SpawnChance)
                                                                                                   .GroupBy(chance => chance.Floor)
                                                                                                   .OrderBy(grouping => grouping.Key);
        _cachedFloors.Clear();
        foreach (IGrouping<int, AreaSpawnChance> grouping in spawnChances)
        {
            Bitmap? bitmap = _cachedMaps[grouping.Key - 1];
            if (bitmap == null)
                continue;
            _cachedFloors.Add(new MonsterHeatmapFloor(grouping.Key, Map.Floors[grouping.Key - 1], bitmap, grouping.ToList()));
        }
        SelectedFloorNum = _cachedFloors.OrderBy(floor => floor.FloorNum).First().FloorNum;
    }

    protected override bool CanIncreaseFloor
    {
        get
        {
            if (_cachedFloors.Count == 0)
                return false;
            return _cachedFloors.Select(floor => floor.FloorNum).Max() > SelectedFloorNum;
        }
    }

    protected override bool CanDecreaseFloor
    {
        get
        {
            if (_cachedFloors.Count == 0)
                return false;
            return _cachedFloors.Select(floor => floor.FloorNum).Min() < SelectedFloorNum;
        }
    }

    protected override void HandleOnSelectedFloorNumChanged(int oldValue, int newValue)
    {
        SelectedFloor = _cachedFloors.FirstOrDefault(floor => floor.FloorNum == newValue);
        _heatmapRenderer.Render(SelectedFloor);
        Image = _heatmapRenderer.GetMapSnapshot()?.ToBitmapSource();
    }

    protected override int NormalizeFloorNum(int oldValue, int newValue)
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
