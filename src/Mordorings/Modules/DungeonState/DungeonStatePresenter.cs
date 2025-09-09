namespace Mordorings.Modules;

public class DungeonStatePresenter(IMordorIoFactory ioFactory) : IDungeonStateMediator
{
    public Monster[] GetMonsters() =>
        ioFactory.GetReader().GetMordorRecord<DATA05Monsters>().MonstersList;

    public void WriteDungeonState(short monsterId, DungeonStateModel model)
    {
        var dungeonState = ioFactory.GetReader().GetMordorRecord<DATA10DungeonState>();
        var processor = new DungeonStateProcessor(monsterId, model, dungeonState);
        DATA10DungeonState newState = processor.Process();
        ioFactory.GetWriter().WriteMordorRecord(newState);
    }
}
