namespace Mordorings;

public static class DATA01GameDataExtensions
{
    public static IEnumerable<MonsterSubtypeIndexed> GetIndexedMonsterSubtypes(this DATA01GameData gameData)
    {
        MonsterSubtype[] monsterTypes = gameData.MonsterSubtypes;
        for (int i = 0; i < monsterTypes.Length; i++)
        {
            yield return new MonsterSubtypeIndexed(monsterTypes[i].Name, i);
        }
    }

    public static IEnumerable<MonsterTypeIndexed> GetIndexedMonsterTypes(this DATA01GameData gameData)
    {
        MonsterType[] monsterTypes = gameData.MonsterTypes;
        for (int i = 0; i < monsterTypes.Length; i++)
        {
            yield return new MonsterTypeIndexed(monsterTypes[i].Name, i);
        }
    }
}
