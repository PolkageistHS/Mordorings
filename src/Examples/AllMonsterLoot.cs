using Calculations;
using MordorDataLibrary.Data;
using MordorDataLibrary.Models;

namespace Examples;

public static class AllMonsterLoot
{
    /// <summary>
    /// Calculates every item drop rate for every monster.
    /// </summary>
    /// <param name="dataFileFolder">The path to the folder containing the MDR files to read from.</param>
    /// <returns>A collection of all monsters and all items they can drop.</returns>
    public static IEnumerable<MonsterLoots> GetAllMonsterLoot(string dataFileFolder)
    {
        MordorRecordReader reader = new(dataFileFolder);
        MonsterLooting looting = new(dataFileFolder);
        DATA05Monsters monsters = reader.GetMordorRecord<DATA05Monsters>();
        foreach (Monster monster in monsters.MonstersList)
        {
            List<ItemDropRate> items = looting.CalculateDropRatesFromMonster(monster).Select(itemDropRate => new ItemDropRate(itemDropRate.Item, itemDropRate.DropRate)).ToList();
            yield return new MonsterLoots(monster.Name, items);
        }
    }
}
