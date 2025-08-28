namespace Calculations;

public class MonsterLooting
{
    public static int CalculateGoldForMonster(Monster currentMonster, short groupSize)
    {
        int goldFactor = currentMonster.GoldFactor;
        int totalGold = (int)(goldFactor * Math.Pow(10, goldFactor - 0.5) / 4);
        totalGold = (int)(totalGold / 4.0 + (Random.Shared.NextDouble() * (totalGold / 2.0) + Random.Shared.NextDouble() * (totalGold / 2.0)) * Math.Log((currentMonster.LevelFound + 1) / Math.Log(2)));
        if (totalGold < 500)
        {
            totalGold = (int)(Random.Shared.NextDouble() * 500 + 2);
        }
        return (int)(totalGold * Math.Log(groupSize + 1) * (Math.Log(groupSize + 1) / 2));
    }

    private record ItemsWithWeights(int Weight, Dictionary<Item, int> ItemsWithRarity);

    private record MonsterLoot(Item? Item, int? ItemSubtypeId, int Weight)
    {
        public bool IsRandom() => Item == null && ItemSubtypeId == null;
    }

    private readonly IEnumerable<short> _lairedMonsterIds;
    private readonly DATA03Items _items;

    public MonsterLooting(string dataFileFolder)
    {
        MordorRecordReader reader = new(dataFileFolder);
        var map = reader.GetMordorRecord<DATA11DungeonMap>();
        _lairedMonsterIds = map.Floors.SelectMany(floor => floor.Areas).Select(area => area.LairId).Where(id => id >= 0).Distinct();
        _items = reader.GetMordorRecord<DATA03Items>();
    }

    public List<ItemDropRate> CalculateDropRatesFromMonster(Monster monster)
    {
        Dictionary<Item, double> chestItems = GetFirstItems(monster, ChestType.Chest);
        Dictionary<Item, double> boxItems = GetFirstItems(monster, ChestType.Box);
        return CalculateWeightedDropRates(monster, boxItems, chestItems).OrderByDescending(itemDropRate => itemDropRate.DropRate).ToList();
    }

    private Dictionary<Item, double> GetFirstItems(Monster monster, ChestType chestType)
    {
        if (monster.BoxChance[1] == 0 && chestType == ChestType.Box || monster.BoxChance[2] == 0 && chestType == ChestType.Chest || !CanMonsterBePrimary(monster))
        {
            return [];
        }
        (int Min, int Max) floorRange = GetFloorRanges(monster, chestType);
        List<MonsterLoot> loots = GetMonsterLootRecords(monster).ToList();
        int totalWeight = loots.Sum(loot => loot.Weight);
        List<ItemsWithWeights> itemsWithWeights = GetItemsWithWeights(loots, floorRange.Min, floorRange.Max).ToList();
        Dictionary<Item, double> retval = new(new ItemEqualityComparer());
        foreach (ItemsWithWeights item in itemsWithWeights.Where(items => items.ItemsWithRarity.Count == 0))
        {
            totalWeight -= item.Weight;
        }
        foreach (ItemsWithWeights itemsWithWeight in itemsWithWeights.Where(items => items.ItemsWithRarity.Count > 0))
        {
            int weight = itemsWithWeight.ItemsWithRarity.Values.Sum();
            foreach ((Item item, int rarity) in itemsWithWeight.ItemsWithRarity)
            {
                retval.TryAdd(item, 0);
                retval[item] += rarity * itemsWithWeight.Weight / ((double)weight * totalWeight);
            }
        }
        foreach (MonsterLoot loot in loots)
        {
            if (loot.Item == null) continue;
            retval.TryAdd(loot.Item, 0);
            retval[loot.Item] += loot.Weight / (double)totalWeight;
        }
        return retval;
    }

    private bool CanMonsterBePrimary(Monster monster) => monster.EncounterChance > 0 || IsMonsterLaired(monster);

    private static (int min, int max) GetFloorRanges(Monster monster, ChestType? chestType)
    {
        int min;
        int max;
        if (monster.Name == "Asmodeus")
        {
            min = monster.ItemDropLevel - 2;
            max = monster.ItemDropLevel;
        }
        else
        {
            min = monster.ItemDropLevel / 2;
            max = monster.ItemDropLevel;
            if (chestType is ChestType.Chest)
            {
                min++;
            }
        }
        if (min == max)
        {
            // minFloor and maxFloor can never be equal to each other by the time the item subtype selection is done,
            // so just decrement minFloor from the get-go to avoid trying to figure out whether the loop restarts or not
            min--;
        }
        return (Math.Max(min, 1), Math.Max(max, 1));
    }

    private IEnumerable<MonsterLoot> GetMonsterLootRecords(Monster monster)
    {
        Dictionary<int, int> weights = [];
        for (int i = 0; i < 10; i++)
        {
            short itemId = monster.Items[i];
            weights.TryAdd(itemId, 0);
            weights[itemId]++;
        }
        foreach ((int id, int weight) in weights)
        {
            Item? lootItem = null;
            int? subtypeId = null;
            switch (id)
            {
                case < -1: // values less than -1 actually refer to the IDs for specific items a monster can drop
                    lootItem = _items.ItemList.First(item => item.Id == Math.Abs(id) - 2);
                    break;
                case -1: // -1 is any random item within the level range
                    break;
                case > -1:
                    subtypeId = id;
                    break;
            }
            yield return new MonsterLoot(lootItem, subtypeId, weight);
        }
    }

    private IEnumerable<ItemsWithWeights> GetItemsWithWeights(IEnumerable<MonsterLoot> lootTable, int minFloor, int maxFloor)
    {
        IQueryable<Item> items = _items.ItemList.Where(item => item.Floor >= minFloor && item.Floor <= maxFloor && item.Rarity > 0).AsQueryable();
        foreach (MonsterLoot loot in lootTable)
        {
            if (loot.ItemSubtypeId != null)
            {
                yield return new ItemsWithWeights(loot.Weight, items.Where(item => item.SubtypeId == loot.ItemSubtypeId).ToDictionary(item => item, item => (int)item.Rarity));
            }
            else if (loot.IsRandom())
            {
                yield return new ItemsWithWeights(loot.Weight, items.ToDictionary(item => item, item => (int)item.Rarity));
            }
        }
    }

    private List<ItemDropRate> CalculateWeightedDropRates(Monster monster, Dictionary<Item, double> boxItems, Dictionary<Item, double> chestItems)
    {
        (double BoxRate, double ChestRate) rates = GetActualChestTypeRates(monster.BoxChance[1], monster.BoxChance[2], IsMonsterLaired(monster));
        List<ItemDropRate> combinedDropList = [];
        if (rates.BoxRate > 0)
        {
            foreach ((Item item, double dropRate) in boxItems)
            {
                double weightedRate = dropRate * rates.BoxRate;
                combinedDropList.Add(new ItemDropRate(item, weightedRate));
            }
        }
        if (rates.ChestRate > 0)
        {
            foreach ((Item item, double dropRate) in chestItems)
            {
                double weightedRate = dropRate * rates.ChestRate;
                combinedDropList.Add(new ItemDropRate(item, weightedRate));
            }
        }
        double totalRate = combinedDropList.Sum(itemDropRate => itemDropRate.DropRate);
        if (totalRate == 0)
        {
            return combinedDropList;
        }
        return combinedDropList.GroupBy(itemDropRate => itemDropRate.Item)
                               .Select(grouping => new ItemDropRate(grouping.Key, grouping.Sum(itemDropRate => itemDropRate.DropRate / totalRate)))
                               .ToList();
    }

    private static (double BoxRate, double ChestRate) GetActualChestTypeRates(double boxChance, double chestChance, bool isLaired)
    {
        int modifier = isLaired ? 25 : 0;
        double adjustedBoxChance = boxChance + modifier;
        double adjustedChestChance = chestChance + modifier;
        double boxRate = adjustedBoxChance - Math.Min(adjustedBoxChance, adjustedChestChance);
        return (BoxRate: boxRate, ChestRate: adjustedChestChance);
    }

    private bool IsMonsterLaired(Monster monster) => _lairedMonsterIds.Contains(monster.Id);

    private class ItemEqualityComparer : IEqualityComparer<Item>
    {
        public bool Equals(Item? x, Item? y) => x?.Id == y?.Id && x?.Name == y?.Name;

        public int GetHashCode(Item obj) => HashCode.Combine(obj.Id, obj.Name);
    }
}

public record ItemDropRate(Item Item, double DropRate);
