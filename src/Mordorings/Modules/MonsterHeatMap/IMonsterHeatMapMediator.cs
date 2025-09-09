namespace Mordorings.Modules;

public interface IMonsterHeatMapMediator
{
    IEnumerable<Monster> GetMonstersBySubtypeId(int? subtypeId);

    List<MonsterSubtypeIndexed> GetMonsterSubtypes();

    object? GetHeatMapImage(int floorNum);

    Task Initialize();

    int? GetFirstFloorForMonster(Monster monster);

    bool HasHigherFloor(int floorNum);

    bool HasLowerFloor(int floorNum);

    int GetNextValidFloorNumber(int oldValue, int newValue);

    HeatMapTileDetails GetTileDetails(Tile tile);
}

public record HeatMapTileDetails(AreaSpawnChance? SpawnChance, int AreaNumber);
