using System.Drawing;

namespace Mordorings.Controls;

public sealed partial class DungeonFloor(Floor floor) : ObservableObject, IDisposable
{
    public Floor Floor { get; } = floor;

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
        for (int i = 0; i < 30; i++)
        {
            for (int j = 0; j < 30; j++)
            {
                tiles[i, j] = floor.Tiles[i + j * 30].Tile;
            }
        }
        return tiles;
    }

    private void ProcessMapSnapshot(object? sender, EventArgs args)
    {
        if (sender is not IAutomapRenderer renderer)
            return;
        Bitmap? mapSnapshot = renderer.GetMapSnapshot();
        Image = mapSnapshot?.ToBitmapSource();
    }

    public void Dispose()
    {
        Renderer?.Dispose();
    }
}
