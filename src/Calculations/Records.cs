using MordorDataLibrary.Models;

namespace Calculations;

public record MonsterLoots(string MonsterName, List<ItemDropRate> Loot);

public record ItemDropRate(Item Item, double DropRate);

public record MonsterSpawnRates(string MonsterName, List<AreaSpawnChance> SpawnRates);

public record AreaSpawnChance(int Floor, int AreaNum, double SpawnChance);
