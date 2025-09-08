namespace Mordorings.Modules.MonsterHeatMap;

public class MonsterHeatMapPresenter(IMordorIoFactory factory) : IMonsterHeatMapMediator
{
    private readonly MordorRecordReader _reader = factory.GetReader();

    public Floor[] GetFloors() => _reader.GetMordorRecord<DATA11DungeonMap>().Floors;

    public List<MonsterSpawnRates> GetAllMonsterSpawns() =>
        new MonsterSpawnCalculator(_reader).GetAllMonsterSpawns();

    public List<MonsterSubtypeIndexed> GetMonsterSubtypes() =>
        _reader.GetMordorRecord<DATA01GameData>().GetIndexedMonsterSubtypes().ToList();

    public IEnumerable<Monster> GetMonstersBySubtypeId(int? subtypeId) =>
        _reader.GetMordorRecord<DATA05Monsters>().GetMonstersBySubtypeId(subtypeId);
}
