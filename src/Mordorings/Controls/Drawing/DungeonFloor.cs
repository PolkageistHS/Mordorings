using System.Drawing;

namespace Mordorings.Controls;

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

    public Teleporter? GetTeleporter(int x, int y) =>
        floor.Teleporters.FirstOrDefault(teleporter => teleporter.x == x + 1 && teleporter.y == y + 1);

    public bool SaveTeleporter(int x, int y, int x2, int y2, int z2)
    {
        Teleporter? teleporter = GetTeleporter(x, y);
        if (teleporter is not null)
        {
            teleporter.x2 = (short)x2;
            teleporter.y2 = (short)y2;
            teleporter.z2 = (short)z2;
            return true;
        }
        int i = GetFirstFreeTeleporter();
        if (i == -1)
            return false;
        floor.Teleporters[i] = new Teleporter { x = (short)(x + 1), y = (short)(y + 1), x2 = (short)x2, y2 = (short)y2, z2 = (short)z2 };
        return true;
    }

    public void DeleteTeleporter(int x, int y)
    {
        foreach (Teleporter teleporter in floor.Teleporters.Where(teleporter => teleporter.x == x + 1 && teleporter.y == y + 1))
        {
            teleporter.x = 0;
            teleporter.y = 0;
            teleporter.x2 = 0;
            teleporter.y2 = 0;
            teleporter.z2 = 0;
        }
    }

    private int GetFirstFreeTeleporter()
    {
        for (int i = 0; i < 20; i++)
        {
            if (floor.Teleporters[i].x == 0)
                return i;
        }
        return -1;
    }

    public Chute? GetChute(int x, int y) =>
        floor.Chutes.FirstOrDefault(chute => chute.x == x + 1 && chute.y == y + 1);

    public bool SaveChute(int x, int y, int depth)
    {
        Chute? chute = GetChute(x, y);
        if (chute is not null)
        {
            chute.Depth = (short)depth;
            return true;
        }
        int i = GetFirstFreeChute();
        if (i == -1)
            return false;
        floor.Chutes[i] = new Chute { x = (short)(x + 1), y = (short)(y + 1), Depth = (short)depth };
        return true;
    }

    public void DeleteChute(int x, int y)
    {
        foreach (Chute chute in floor.Chutes.Where(chute => chute.x == x + 1 && chute.y == y + 1))
        {
            chute.x = 0;
            chute.y = 0;
            chute.Depth = 0;
        }
    }

    private int GetFirstFreeChute()
    {
        for (int i = 0; i < 10; i++)
        {
            if (floor.Chutes[i].x == 0)
                return i;
        }
        return -1;
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
        Bitmap? mapSnapshot = renderer.GetMapSnapshot();
        Image = mapSnapshot?.ToBitmapSource();
    }

    public void Dispose()
    {
        Renderer?.Dispose();
    }
}
