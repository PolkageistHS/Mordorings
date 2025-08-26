using System.Drawing;
using System.Drawing.Drawing2D;

namespace Mordorings.Controls;

public class AutomapRenderer : IAutomapRenderer
{
    private DungeonFloor? _dungeonFloor;
    private Bitmap? _spriteSheet;
    private Bitmap? _mapBuffer;
    private Graphics? _mapGraphics;

    private const int TileSize = 14;

    public const int MapWidth = 30;
    public const int MapHeight = 30;

    public event EventHandler? MapUpdated;

    public static Size MapPixelSize => new(MapWidth * TileSize + 1, MapHeight * TileSize + 1);

    public void Initialize(DungeonFloor? dungeonFloor)
    {
        ArgumentNullException.ThrowIfNull(dungeonFloor);
        _dungeonFloor?.Dispose();
        _dungeonFloor = dungeonFloor;
        _mapBuffer?.Dispose();
        _mapGraphics?.Dispose();
        _mapBuffer = new Bitmap(MapPixelSize.Width, MapPixelSize.Height);
        _mapGraphics = Graphics.FromImage(_mapBuffer);
        _mapGraphics.CompositingMode = CompositingMode.SourceOver;
        _mapGraphics.InterpolationMode = InterpolationMode.NearestNeighbor;
        _mapGraphics.Clear(Color.Black);
        DrawMapBorders();
    }

    public void LoadSpriteSheet(string? filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        _spriteSheet?.Dispose();
        _spriteSheet = new Bitmap(filePath);
    }

    public void LoadSpriteSheet(Bitmap? bitmap)
    {
        ArgumentNullException.ThrowIfNull(bitmap);
        _spriteSheet?.Dispose();
        _spriteSheet = new Bitmap(bitmap);
    }

    public void DrawDungeonFloorMap()
    {
        if (_dungeonFloor is null || _mapGraphics is null || _mapBuffer is null)
            throw new InvalidOperationException($"Map not initialized. Call {nameof(Initialize)} first.");
        _mapGraphics.Clear(Color.Black);
        DrawMapBorders();
        for (int x = MapWidth - 1; x >= 0; x--)
        {
            for (int y = MapHeight - 1; y >= 0; y--)
            {
                DrawTileToBuffer(x, y, _dungeonFloor.Tiles[x, y]);
            }
        }
        MapUpdated?.Invoke(this, EventArgs.Empty);
    }

    public Bitmap? GetMapSnapshot()
    {
        if (_mapBuffer is not null)
            return new Bitmap(_mapBuffer);
        return null;
    }

    public bool UpdateTile(int x, int y, DungeonTileFlag newTileData)
    {
        long tileData = (long)newTileData;
        if (_dungeonFloor is null || x is < 0 or >= MapWidth || y is < 0 or >= MapHeight)
            return false;
        if (_dungeonFloor.Tiles[x, y] == tileData)
            return false;
        _dungeonFloor.Tiles[x, y] = tileData;
        DrawDungeonFloorMap();
        HighlightTile(x, y);
        return true;
    }

    private void DrawMapBorders()
    {
        Pen pen = new(Color.White, 1);
        if (_mapBuffer == null || _mapGraphics == null)
            return;
        int x = _mapBuffer.Width;
        int y = _mapBuffer.Height;
        _mapGraphics.DrawLine(pen, 0, 0, 0, y);
        _mapGraphics.DrawLine(pen, 0, 0, x, 0);
        _mapGraphics.DrawLine(pen, x - 1, y - 1, x - 1, 0);
        _mapGraphics.DrawLine(pen, x - 1, y - 1, 0, y - 1);
    }

    private void DrawTileToBuffer(int tileX, int tileY, long bitmask)
    {
        if (_spriteSheet is null)
            throw new InvalidOperationException("Sprite sheet has not been loaded.");
        if (_mapGraphics is null)
            return;
        int imageTileY = MapHeight - 1 - tileY;
        int baseX = tileX * TileSize;
        int baseY = imageTileY * TileSize;
        _mapGraphics.FillRectangle(Brushes.Black, baseX, baseY, TileSize, TileSize);
        for (int i = 0; i <= 22; i++)
        {
            if ((bitmask & (1 << i)) != 0)
            {
                DrawSpriteLayer(_mapGraphics, i, baseX, baseY);
            }
        }
    }

    private void DrawSpriteLayer(Graphics graphics, int spriteIndex, int baseX, int baseY)
    {
        if (_spriteSheet is null)
            return;
        (int offsetX, int offsetY) = spriteIndex switch
        {
            0 or 2 or 4 => (7, 0), // east walls/doors
            1 or 3 or 5 => (0, -7), // north walls/doors
            _ => (0, 0) // everything else
        };
        const int spriteTile = TileSize + 1;
        var sourceRect = new Rectangle(spriteIndex * spriteTile, 0, spriteTile, spriteTile);
        var destRect = new Rectangle(baseX + offsetX, baseY + offsetY, spriteTile, spriteTile);
        graphics.DrawImage(_spriteSheet, destRect, sourceRect, GraphicsUnit.Pixel);
    }

    public void HighlightTile(int tileX, int tileY)
    {
        if (_mapGraphics is null)
            return;
        int screenTileY = MapHeight - 1 - tileY;
        int baseX = tileX * TileSize;
        int baseY = screenTileY * TileSize;
        Brush brush = new SolidBrush(Color.FromArgb(120, Color.Magenta));
        Pen pen = new(brush, 2);
        _mapGraphics.DrawRectangle(pen, baseX, baseY, TileSize, TileSize);
        MapUpdated?.Invoke(this, EventArgs.Empty);
    }

    public void RemoveHighlight()
    {
        DrawDungeonFloorMap();
    }

    public void Dispose()
    {
        _spriteSheet?.Dispose();
        _dungeonFloor?.Dispose();
        GC.SuppressFinalize(this);
    }
}
