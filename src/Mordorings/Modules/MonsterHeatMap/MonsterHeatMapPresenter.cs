using System.Drawing;

namespace Mordorings.Modules.MonsterHeatMap;

public class MonsterHeatMapPresenter(IMordorIoFactory factory, IMapRendererFactory mapRendererFactory) : IMonsterHeatMapMediator
{
    private const string SpriteSheetFile = "Assets/DungeonSprites.bmp";

    private Floor[]? _floors;
    private List<MonsterSpawnRates> _cachedMonsterSpawnRates = [];
    private readonly MordorRecordReader _reader = factory.GetReader();
    private readonly IHeatMapRenderer _heatMapRenderer = mapRendererFactory.CreateHeatMapRenderer();
    private readonly Dictionary<int, Bitmap?> _cachedMaps = [];
    private readonly List<MonsterHeatMapFloor> _cachedFloors = [];
    private MonsterHeatMapFloor? _currentFloor;

    public async Task Initialize()
    {
        await InitializeRenderers();
    }

    public List<MonsterSubtypeIndexed> GetMonsterSubtypes() =>
        _reader.GetMordorRecord<DATA01GameData>().GetIndexedMonsterSubtypes().ToList();

    public IEnumerable<Monster> GetMonstersBySubtypeId(int? subtypeId) =>
        _reader.GetMordorRecord<DATA05Monsters>().GetMonstersBySubtypeId(subtypeId);

    public object? GetHeatMapImage(int floorNum)
    {
        _currentFloor = _cachedFloors.FirstOrDefault(floor => floor.FloorNum == floorNum);
        _heatMapRenderer.Render(_currentFloor);
        return _heatMapRenderer.GetMapSnapshot()?.ToBitmapSource();
    }

    public bool HasHigherFloor(int floorNum)
    {
        if (_cachedFloors.Count == 0)
            return false;
        return _cachedFloors.Select(floor => floor.FloorNum).Max() > floorNum;
    }

    public bool HasLowerFloor(int floorNum)
    {
        if (_cachedFloors.Count == 0)
            return false;
        return _cachedFloors.Select(floor => floor.FloorNum).Min() < floorNum;
    }

    public HeatMapTileDetails GetTileDetails(Tile tile)
    {
        if (_currentFloor == null)
            return new HeatMapTileDetails(null, 0);
        int area = _currentFloor.DungeonFloor.GetAreaFromTile(tile);
        AreaSpawnChance? spawn = _currentFloor.SpawnRates.FirstOrDefault(chance => chance.AreaNum == area);
        return new HeatMapTileDetails(spawn, area);
    }

    public int? GetFirstFloorForMonster(Monster monster)
    {
        if (_floors == null)
            throw new InvalidOperationException($"Call {nameof(Initialize)}() before using this method.");
        IOrderedEnumerable<IGrouping<int, AreaSpawnChance>>? spawnChances = _cachedMonsterSpawnRates.FirstOrDefault(rates => Equals(rates.Monster, monster))
                                                                                                    ?.SpawnRates
                                                                                                    .OrderByDescending(chance => chance.SpawnChance)
                                                                                                    .GroupBy(chance => chance.Floor)
                                                                                                    .OrderBy(grouping => grouping.Key);
        _cachedFloors.Clear();
        if (spawnChances == null)
            return null;
        foreach (IGrouping<int, AreaSpawnChance> grouping in spawnChances)
        {
            Bitmap? bitmap = _cachedMaps[grouping.Key - 1];
            if (bitmap == null)
                continue;
            _cachedFloors.Add(new MonsterHeatMapFloor(grouping.Key, _floors[grouping.Key - 1], bitmap, grouping.ToList()));
        }
        return _cachedFloors.OrderBy(floor => floor.FloorNum).First().FloorNum;

    }

    public int GetNextValidFloorNumber(int oldValue, int newValue)
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

    private async Task InitializeRenderers()
    {
        _floors = _reader.GetMordorRecord<DATA11DungeonMap>().Floors;
        for (int i = 0; i < _floors.Length; i++)
        {
            var dungeonFloor = new DungeonFloor(_floors[i]);
            IAutomapRenderer renderer = mapRendererFactory.CreateAutomapRenderer();
            renderer.LoadSpriteSheet(SpriteSheetFile);
            renderer.Initialize(dungeonFloor);
            renderer.DrawDungeonFloorMap();
            _cachedMaps[i] = renderer.GetMapSnapshot();
        }
        await Task.Run(() => { _cachedMonsterSpawnRates = new MonsterSpawnCalculator(_reader).GetAllMonsterSpawns(); });
    }
}
