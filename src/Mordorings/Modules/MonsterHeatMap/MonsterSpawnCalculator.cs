namespace Mordorings.Modules.MonsterHeatMap;

public class MonsterSpawnCalculator(MordorRecordReader reader)
{
    public List<MonsterSpawnRates> GetAllMonsterSpawns()
    {
        var spawning = new MonsterSpawning(reader);
        var map = reader.GetMordorRecord<DATA11DungeonMap>();
        List<MonsterSpawnRates> monsterRates = [];
        for (int floor = 1; floor <= 15; floor++)
        {
            for (int y = 0; y < 30; y++)
            {
                for (int x = 0; x < 30; x++)
                {
                    short area = map.Floors[floor - 1].Tiles[x + y * 30].Area;
                    foreach ((Monster monster, double chance) in spawning.GetExpectedMonsterSpawnProbabilities(x + 1, y + 1, floor, false))
                    {
                        double rounded = Math.Round(chance, 3);
                        MonsterSpawnRates? monsterEntry = monsterRates.FirstOrDefault(rates => rates.Monster.Name == monster.Name);
                        if (monsterEntry == null)
                        {
                            monsterRates.Add(new MonsterSpawnRates(monster, [new AreaSpawnChance(floor, area, rounded)]));
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
        return monsterRates;
    }
}
