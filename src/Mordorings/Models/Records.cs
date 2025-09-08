using System.Drawing;
using Mordorings.Modules.EditMap;

namespace Mordorings.Models;

public record MonsterHeatMapFloor(int FloorNum, Floor DungeonFloor, Bitmap Map, List<AreaSpawnChance> SpawnRates);

public record Tile(int X, int Y);

public record MonsterSubtypeIndexed(string Name, int Index);

public record MonsterTypeIndexed(string Name, int Index);

public record MonsterSpawnRates(Monster Monster, List<AreaSpawnChance> SpawnRates);

public record AreaSpawnChance(int Floor, int AreaNum, double SpawnChance);

public record TileFlagChangedEventArgs(Tile Tile, DungeonTileFlag AllFlags, MapObjects MapObjects);
