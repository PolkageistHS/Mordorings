using System.Drawing;
using System.Drawing.Drawing2D;

namespace Mordorings.Controls;

public class MapRendererBase : IDisposable, IMapRendererBase
{
    protected const int TileSize = 14;
    protected const int MapWidth = 30;
    protected const int MapHeight = 30;

    protected Bitmap? MapBuffer { get; private set; }

    protected Graphics? MapGraphics { get; private set; }

    private static Size MapPixelSize => new(MapWidth * TileSize + 1, MapHeight * TileSize + 1);

    protected MapRendererBase()
    {
        MapBuffer = new Bitmap(MapPixelSize.Width, MapPixelSize.Height);
        MapGraphics = Graphics.FromImage(MapBuffer);
        MapGraphics.CompositingMode = CompositingMode.SourceOver;
        MapGraphics.InterpolationMode = InterpolationMode.NearestNeighbor;
    }

    public Bitmap? GetMapSnapshot()
    {
        if (MapBuffer is not null)
            return new Bitmap(MapBuffer);
        return null;
    }

    protected void ReplaceBitmap(Bitmap bitmap)
    {
        MapBuffer = new Bitmap(bitmap);
        MapGraphics?.Dispose();
        MapGraphics = Graphics.FromImage(MapBuffer);
        MapGraphics.CompositingMode = CompositingMode.SourceOver;
        MapGraphics.InterpolationMode = InterpolationMode.NearestNeighbor;
    }

    protected void DrawTileBorder(Tile tile, Color color, int alpha)
    {
        if (MapGraphics is null)
            return;
        int screenTileY = MapHeight - 1 - tile.Y;
        int baseX = tile.X * TileSize;
        int baseY = screenTileY * TileSize;
        Brush brush = new SolidBrush(Color.FromArgb(alpha, color));
        MapGraphics.DrawRectangle(new Pen(brush, 2), baseX, baseY, TileSize, TileSize);
    }

    protected void DrawRectangleOnTile(int tileX, int tileY, Color color, int alpha)
    {
        DrawRectangleOnTile(tileX, tileY, Color.FromArgb(alpha, color));
    }

    protected void DrawRectangleOnTile(int tileX, int tileY, Color color)
    {
        if (MapGraphics is null)
            return;
        int screenTileY = MapHeight - 1 - tileY;
        int baseX = tileX * TileSize;
        int baseY = screenTileY * TileSize;
        Brush brush = new SolidBrush(color);
        MapGraphics.FillRectangle(brush, baseX, baseY, TileSize, TileSize);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing)
            return;
        MapGraphics?.Dispose();
        MapBuffer?.Dispose();
    }
}
