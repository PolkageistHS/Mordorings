using Mordorings.Controls;

namespace Mordorings.Models;

public sealed partial class DungeonFloor(Floor floor) : ObservableObject, IDisposable
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

    public IAutomapRenderer? Renderer { get; private set; }

    [ObservableProperty]
    private object? _image;

    public void Initialize(IAutomapRenderer renderer)
    {
        Renderer = renderer;
        Renderer.Initialize(this);
        Renderer.MapUpdated += ProcessMapSnapshot;
        Renderer.DrawDungeonFloorMap();
    }

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

    private void ProcessMapSnapshot(object? sender, EventArgs args)
    {
        if (sender is not IAutomapRenderer renderer)
            return;
        Image = renderer.GetMapSnapshot()?.ToBitmapSource();
    }

    public void Dispose()
    {
        Renderer?.Dispose();
    }
}
