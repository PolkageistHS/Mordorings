using Mordorings.Modules;

namespace Mordorings.Models;

public record Tile(int X, int Y);

public record MonsterSubtypeIndexed(string Name, int Index);

public record MonsterTypeIndexed(string Name, int Index);

public record MonsterSpawnRates(Monster Monster, List<AreaSpawnChance> SpawnRates);

public record AreaSpawnChance(int Floor, int AreaNum, double SpawnChance);

public record TileFlagChangedEventArgs(Tile Tile, DungeonTileFlag AllFlags, MapObjects MapObjects);
