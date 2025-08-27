namespace Mordorings.Models;

public sealed partial class DungeonFloor(Floor floor) : ObservableObject
{
    public Floor Floor
    {
        get
        {
            for (int x = 0; x < 30; x++)
            {
                for (int y = 0; y < 30; y++)
                {
                    floor.Tiles[x + y * 30].Tile = Tiles[x, y];
                }
            }
            return floor;
        }
    }

    public long[,] Tiles { get; } = GetTiles(floor);

    [ObservableProperty]
    private object? _image;

    private static long[,] GetTiles(Floor floor)
    {
        long[,] tiles = new long[30, 30];
        for (int x = 0; x < 30; x++)
        {
            for (int y = 0; y < 30; y++)
            {
                tiles[x, y] = floor.Tiles[x + y * 30].Tile;
            }
        }
        return tiles;
    }
}
