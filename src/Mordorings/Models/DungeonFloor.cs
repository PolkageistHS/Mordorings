namespace Mordorings.Models;

public sealed class DungeonFloor(Floor floor)
{
    public Floor Floor
    {
        get
        {
            for (int x = 0; x < Game.FloorWidth; x++)
            {
                for (int y = 0; y < Game.FloorHeight; y++)
                {
                    floor.Tiles[x + y * Game.FloorHeight].Tile = Tiles[x, y];
                }
            }
            return floor;
        }
    }

    public long[,] Tiles { get; } = GetTiles(floor);

    private static long[,] GetTiles(Floor floor)
    {
        long[,] tiles = new long[Game.FloorWidth, Game.FloorHeight];
        for (int x = 0; x < Game.FloorWidth; x++)
        {
            for (int y = 0; y < Game.FloorHeight; y++)
            {
                tiles[x, y] = floor.Tiles[x + y * Game.FloorHeight].Tile;
            }
        }
        return tiles;
    }
}
