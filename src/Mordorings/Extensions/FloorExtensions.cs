namespace Mordorings;

public static class FloorExtensions
{
    public static int GetAreaFromTile(this Floor floor, Tile tile) =>
        floor.Tiles[tile.X + tile.Y * 30].Area;

    public static IEnumerable<Tile> GetTilesForArea(this Floor floor, int area)
    {
        for (int x = 0; x < 30; x++)
        {
            for (int y = 0; y < 30; y++)
            {
                if (floor.Tiles[x + y * 30].Area == area)
                {
                    yield return new Tile(x, y);
                }
            }
        }
    }
}
