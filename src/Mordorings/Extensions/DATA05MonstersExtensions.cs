namespace Mordorings;

public static class DATA05MonstersExtensions
{
    public static IEnumerable<Monster> GetMonstersBySubtypeId(this DATA05Monsters monsters, int? subtypeId) =>
        monsters.MonstersList.Where(monster => monster.MonsterSubtype == subtypeId).OrderBy(monster => monster.Name);
}
