namespace Mordorings.Modules.MonsterHeatMap;

public interface IMonsterHeatMapMediator
{
    Floor[] GetFloors();

    List<MonsterSpawnRates> GetAllMonsterSpawns();

    IEnumerable<Monster> GetMonstersBySubtypeId(int? subtypeId);

    List<MonsterSubtypeIndexed> GetMonsterSubtypes();
}
