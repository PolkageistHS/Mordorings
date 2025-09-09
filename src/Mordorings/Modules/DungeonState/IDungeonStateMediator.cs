namespace Mordorings.Modules;

public interface IDungeonStateMediator
{
    Monster[] GetMonsters();

    void WriteDungeonState(short monsterId, DungeonStateModel model);
}
