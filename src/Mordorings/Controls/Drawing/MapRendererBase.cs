using System.Drawing.Drawing2D;

namespace Mordorings.Controls;

public abstract class MapRendererBase : IMapRendererBase, IDisposable
{
    public const int LeftGutterWidth = 14;
    protected const int BottomGutterHeight = 14;
    protected internal const int TileSize = 14;

    private Bitmap MapBuffer { get; set; } = new(ImagePixelSize.Width, ImagePixelSize.Height);

    protected Graphics MapGraphics { get; private set; }

    protected internal static Size ImagePixelSize =>
        new(MapPixelSize.Width + LeftGutterWidth, MapPixelSize.Height + BottomGutterHeight);

    protected static Size MapPixelSize =>
        new(Game.FloorWidth * TileSize, Game.FloorHeight * TileSize);

    protected MapRendererBase()
    {
        MapGraphics = Graphics.FromImage(MapBuffer);
        MapGraphics.CompositingMode = CompositingMode.SourceOver;
        MapGraphics.InterpolationMode = InterpolationMode.NearestNeighbor;
    }

    public Bitmap GetMapSnapshot() => new(MapBuffer);

    protected void ReplaceBitmap(Bitmap bitmap)
    {
        if (bitmap.Width != ImagePixelSize.Width || bitmap.Height != ImagePixelSize.Height)
            throw new ArgumentException("Bitmap must be the same size as the map.");
        MapBuffer = new Bitmap(bitmap);
        MapGraphics.Dispose();
        MapGraphics = Graphics.FromImage(MapBuffer);
        MapGraphics.CompositingMode = CompositingMode.SourceOver;
        MapGraphics.InterpolationMode = InterpolationMode.NearestNeighbor;
    }

    protected void DrawTileBorder(Tile tile, Color color, int alpha)
    {
        (int x, int y) = GetPixels(tile);
        Brush brush = new SolidBrush(Color.FromArgb(alpha, color));
        MapGraphics.DrawRectangle(new Pen(brush, 2), x, y, TileSize, TileSize);
    }

    protected void DrawRectangleOnTile(Tile tile, Color color)
    {
        (int x, int y) = GetPixels(tile);
        Brush brush = new SolidBrush(color);
        MapGraphics.FillRectangle(brush, x, y, TileSize, TileSize);
    }

    private static (int x, int y) GetPixels(Tile tile) =>
        (tile.X * TileSize + LeftGutterWidth, (Game.FloorHeight - 1 - tile.Y) * TileSize);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing)
            return;
        MapGraphics.Dispose();
        MapBuffer.Dispose();
    }
}
