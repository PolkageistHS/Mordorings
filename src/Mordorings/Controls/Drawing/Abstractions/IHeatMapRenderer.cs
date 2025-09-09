namespace Mordorings.Controls;

public interface IHeatMapRenderer : IMapRendererBase
{
    void Render(MonsterHeatMapFloor? floor);
}

public record MonsterHeatMapFloor(int FloorNum, Floor DungeonFloor, Bitmap Map, List<AreaSpawnChance> SpawnRates);
