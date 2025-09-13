namespace Calculations;

public class MonsterSpawning
{
    private readonly DATA05Monsters _monsters;
    private readonly DATA11DungeonMap _map;
    private readonly DATA01GameData _gameData;

    public MonsterSpawning(MordorRecordReader reader)
    {
        _gameData = reader.GetMordorRecord<DATA01GameData>();
        _monsters = reader.GetMordorRecord<DATA05Monsters>();
        _map = reader.GetMordorRecord<DATA11DungeonMap>();
    }

    public MonsterSpawning(string dataFileFolder)
    {
        MordorRecordReader reader = new(dataFileFolder);
        _gameData = reader.GetMordorRecord<DATA01GameData>();
        _monsters = reader.GetMordorRecord<DATA05Monsters>();
        _map = reader.GetMordorRecord<DATA11DungeonMap>();
    }

    /// <summary>
    /// Finds all monsters that can spawn in the provided tile and calculates their chance of spawning.
    /// </summary>
    /// <param name="x">The tile's x coordinate on the map.</param>
    /// <param name="y">The tile's y coordinate on the map.</param>
    /// <param name="floorNum">The dungeon floor number.</param>
    /// <param name="includeRandom">Nearly every Area has a 1% chance of spawning a monster from a
    /// random MonsterSubtype, even if the SpawnMask does not include that subtype. Set this to True
    /// to include the monsters that can spawn as part of that 1%.</param>
    /// <returns>A collection of monsters that can spawn on the provided tile with their chance of spawning.</returns>
    public List<MonsterSpawnRate> GetExpectedMonsterSpawnProbabilities(int x, int y, int floorNum, bool includeRandom)
    {
        Floor floor = _map.Floors[floorNum - 1];
        DungeonMapTile tile = floor.Tiles[x + (y - 1) * 30 - 1];
        short areaNum = tile.Area;
        Area area = floor.Areas[areaNum];
        if (area.SpawnMask == 0 || IsRock(tile.Tile))
            return [];
        bool isStud = IsStud(tile.Tile);
        int baseFloor;
        if (isStud)
        {
            baseFloor = floorNum + 1;
        }
        else
        {
            baseFloor = floorNum;
        }
        Dictionary<Monster, double> combinedChances = [];
        foreach ((int floorRange, double probabilityWeight) in GetPossibleFloorRanges(floorNum, isStud))
        {
            Dictionary<Monster, double> perRangeChances = GetMonsterSpawnChancesForFloorRange(area, areaNum, floorNum, baseFloor, floorRange, includeRandom);
            foreach ((Monster monster, double percent) in perRangeChances)
            {
                combinedChances.TryAdd(monster, 0);
                combinedChances[monster] += percent * probabilityWeight;
            }
        }
        List<MonsterSpawnRate> sortedChances = [];
        foreach ((Monster monster, double percent) in combinedChances)
        {
            sortedChances.Add(new MonsterSpawnRate(monster, percent));
        }
        return sortedChances.OrderByDescending(rate => rate.SpawnChance).ToList();
    }

    private static Dictionary<int, double> GetPossibleFloorRanges(int floorNum, bool isStud)
    {
        Dictionary<int, double> results = [];
        if (isStud)
        {
            const double studChance = 2.0 / 3.0;
            results[1] = studChance;
            int baseRange = floorNum / 5;
            int maxRange = Math.Min(baseRange, floorNum);
            const double fallbackChance = 1.0 / 3.0;
            if (!results.TryAdd(maxRange, fallbackChance))
            {
                results[maxRange] += fallbackChance;
            }
        }
        else
        {
            int baseRange = floorNum / 5;
            int range1 = Math.Min(baseRange + 1, floorNum);
            int range2 = Math.Min(baseRange, floorNum);
            const double range1Chance = 49.0 / 100.0;
            const double range2Chance = 51.0 / 100.0;
            results[range2] = range2Chance;
            if (!results.TryAdd(range1, range1Chance))
            {
                results[range1] += range1Chance;
            }
        }
        return results;
    }

    private Dictionary<Monster, double> GetMonsterSpawnChancesForFloorRange(Area area, int areaNum, int floorNum, int baseFloor, int floorRange, bool includeRandom)
    {
        const double fullWeight = 1.0;
        Monster? lairedMonster;
        double lairedPercent;
        if (area.LairId > 0)
        {
            lairedMonster = _monsters.MonstersList.First(monster => monster.Id == area.LairId);
            lairedPercent = 0.75;
        }
        else
        {
            lairedMonster = null;
            lairedPercent = 0.0;
        }
        double remainingPercent = fullWeight - lairedPercent;
        int minFloor = baseFloor - floorRange;
        int maxFloor = baseFloor;
        List<(MonsterSubtype subtype, List<Monster> Monsters)> validMonstersBySubtype = [];
        validMonstersBySubtype.AddRange(_gameData.MonsterSubtypes.Select((subtype, i) => (subtype, _monsters.MonstersList
                                                                                                            .Where(monster => monster.Id > 0 &&
                                                                                                                              monster.MonsterSubtype == i &&
                                                                                                                              monster.LevelFound >= minFloor &&
                                                                                                                              monster.LevelFound <= maxFloor &&
                                                                                                                              monster.EncounterChance > 0)
                                                                                                            .ToList())));
        if (validMonstersBySubtype.Count == 0)
            return [];
        Dictionary<Monster, double> monsterChances = [];
        if (lairedMonster != null)
        {
            monsterChances[lairedMonster] = lairedPercent;
        }
        double randomSubtypeChance;
        if ((floorNum == 1 && areaNum < 18) || !includeRandom)
        {
            randomSubtypeChance = 0;
        }
        else
        {
            randomSubtypeChance = 0.01;
        }
        double randomSubtypePortion = remainingPercent * randomSubtypeChance;
        foreach ((MonsterSubtype Subtype, List<Monster> ValidMonsters) subtypeEntry in validMonstersBySubtype)
        {
            double subtypeChance = randomSubtypePortion / validMonstersBySubtype.Count;
            double perMonsterChance = subtypeChance / subtypeEntry.ValidMonsters.Count;
            foreach (Monster monster in subtypeEntry.ValidMonsters)
            {
                monsterChances.TryAdd(monster, 0);
                monsterChances[monster] += perMonsterChance;
            }
        }
        List<(MonsterSubtype Subtype, List<Monster> ValidMonsters)> spawnableSubtypes = validMonstersBySubtype.Where((_, i) => CheckMask(area.SpawnMask, i)).ToList();
        Dictionary<MonsterSubtype, double> subtypeWeights = spawnableSubtypes.ToDictionary(tuple => tuple.Subtype, tuple => (double)tuple.ValidMonsters.Sum(monster => monster.EncounterChance));
        double totalSubtypeWeight = subtypeWeights.Values.Sum();
        if (totalSubtypeWeight == 0)
            return monsterChances;
        double setSubtypePortion = remainingPercent * (1 - randomSubtypeChance);
        foreach ((MonsterSubtype subtype, double subtypeWeight) in subtypeWeights)
        {
            List<Monster> validMonsters = spawnableSubtypes.First(tuple => tuple.Subtype == subtype).ValidMonsters;
            double subtypePercent = subtypeWeight / totalSubtypeWeight * setSubtypePortion;
            double monsterTotalWeight = validMonsters.Sum(monster => monster.EncounterChance);
            foreach (Monster monster in validMonsters)
            {
                double monsterChance = monster.EncounterChance / monsterTotalWeight * subtypePercent;
                monsterChances.TryAdd(monster, 0);
                monsterChances[monster] += monsterChance;
            }
        }
        return monsterChances.Where(pair => pair.Value > 0).ToDictionary();
    }

    private static bool IsStud(long tileMask) => CheckMask(tileMask, 22);

    private static bool IsRock(long tileMask) => CheckMask(tileMask, 19);

    private static bool CheckMask(long mask, int index)
    {
        int x = 1 << index;
        long compare = mask & x;
        return compare == x;
    }
}

public record MonsterSpawnRate(Monster Monster, double SpawnChance);
