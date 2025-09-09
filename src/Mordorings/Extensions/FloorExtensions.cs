namespace Mordorings;

public static class FloorExtensions
{
    public static int GetAreaFromTile(this Floor floor, Tile tile) =>
        floor.Tiles[tile.X + tile.Y * Game.FloorHeight].Area;

    public static IEnumerable<Tile> GetTilesForArea(this Floor floor, int area)
    {
        for (int x = 0; x < Game.FloorWidth; x++)
        {
            for (int y = 0; y < Game.FloorHeight; y++)
            {
                if (floor.Tiles[x + y * Game.FloorHeight].Area == area)
                {
                    yield return new Tile(x, y);
                }
            }
        }
    }
}
