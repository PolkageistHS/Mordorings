using Calculations;
using MordorDataLibrary.Data;
using MordorDataLibrary.Models;

namespace Examples;

public static class AllSpawnRates
{
    /// <summary>
    /// Calculates the rate at which monsters can spawn on every tile in the dungeon and groups them by monster
    /// </summary>
    /// <param name="dataFileFolder">The path to the folder containing the MDR files to read from.</param>
    /// <param name="includeRandomSubtypeSpawns">Nearly every Area has a 1% chance of spawning a monster from a
    /// random MonsterSubtype, even if the SpawnMask does not include that subtype. Set this to True
    /// to include the monsters that can spawn as part of that 1%.</param>
    public static IEnumerable<MonsterSpawnRates> GetAllMonsterSpawnRates(string dataFileFolder, bool includeRandomSubtypeSpawns)
    {
        MordorRecordReader reader = new(dataFileFolder);
        MonsterSpawning spawning = new(dataFileFolder);
        DATA11DungeonMap map = reader.GetMordorRecord<DATA11DungeonMap>();
        List<MonsterSpawnRates> monsterRates = [];
        for (int floor = 1; floor <= 15; floor++)
        {
            for (int y = 1; y <= 30; y++)
            {
                for (int x = 1; x <= 30; x++)
                {
                    short area = map.Floors[floor - 1].Tiles[x + (y - 1) * 30 - 1].Area;
                    foreach ((Monster monster, double chance) in spawning.GetExpectedMonsterSpawnProbabilities(x, y, floor, includeRandomSubtypeSpawns))
                    {
                        double rounded = Math.Round(chance, 3);
                        MonsterSpawnRates? monsterEntry = monsterRates.FirstOrDefault(rates => rates.MonsterName == monster.Name);
                        if (monsterEntry == null)
                        {
                            monsterRates.Add(new MonsterSpawnRates(monster.Name, [new AreaSpawnChance(floor, area, rounded)]));
                        }
                        else
                        {
                            AreaSpawnChance? existingRate = monsterEntry.SpawnRates.FirstOrDefault(spawn => spawn.AreaNum == area && spawn.Floor == floor);
                            if (existingRate == null)
                            {
                                monsterEntry.SpawnRates.Add(new AreaSpawnChance(floor, area, rounded));
                            }
                        }
                    }
                }
            }
        }
        return Sort(monsterRates);
    }

    private static IEnumerable<MonsterSpawnRates> Sort(List<MonsterSpawnRates> monsterRates)
    {
        return monsterRates.OrderBy(rates => rates.MonsterName)
                           .Select(rates => rates with
                           {
                               SpawnRates = rates.SpawnRates
                                            .OrderByDescending(spawn => spawn.SpawnChance)
                                            .ThenBy(spawn => spawn.Floor)
                                            .ThenBy(spawn => spawn.AreaNum)
                                            .ToList()
                           });
    }
}
