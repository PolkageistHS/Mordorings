using System.Drawing;

namespace Mordorings.Controls;

public class HeatmapRenderer : MapRendererBase, IHeatmapRenderer
{
    public void Render(MonsterHeatmapFloor? floor)
    {
        if (floor == null)
            return;
        ReplaceBitmap(floor.Map);
        foreach (AreaSpawnChance spawnRate in floor.SpawnRates)
        {
            for (int x = 0; x < MapWidthInTiles; x++)
            {
                for (int y = 0; y < MapHeightInTiles; y++)
                {
                    var tile = new Tile(x, y);
                    if (floor.DungeonFloor.GetAreaFromTile(tile) != spawnRate.AreaNum)
                        continue;
                    Color color = ProbabilityToColor(spawnRate.SpawnChance);
                    DrawRectangleOnTile(tile, color);
                }
            }
        }
    }

    private static Color ProbabilityToColor(double probability)
    {
        if (probability == 0)
            return Color.FromArgb(0, Color.Transparent);
        double scaled = Math.Sqrt(probability);
        int i = (int)(scaled * 255);
        int alpha = (int)(scaled * 175);
        return Color.FromArgb(Math.Max(alpha, 75), 255, 255 - i, 255 - i);
    }
}
