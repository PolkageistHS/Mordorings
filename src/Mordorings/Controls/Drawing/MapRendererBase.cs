using System.Drawing;
using System.Drawing.Drawing2D;

namespace Mordorings.Controls;

public class MapRendererBase : IDisposable
{
    protected readonly Bitmap? MapBuffer;
    protected readonly Graphics? MapGraphics;

    protected const int TileSize = 14;
    protected const int MapWidth = 30;
    protected const int MapHeight = 30;

    private static Size MapPixelSize => new(MapWidth * TileSize + 1, MapHeight * TileSize + 1);

    protected MapRendererBase()
    {
        MapBuffer = new Bitmap(MapPixelSize.Width, MapPixelSize.Height);
        MapGraphics = Graphics.FromImage(MapBuffer);
        MapGraphics.CompositingMode = CompositingMode.SourceOver;
        MapGraphics.InterpolationMode = InterpolationMode.NearestNeighbor;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing)
            return;
        MapGraphics?.Dispose();
        MapBuffer?.Dispose();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
