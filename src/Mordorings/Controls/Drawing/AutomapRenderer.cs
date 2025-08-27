using System.Drawing;
using Mordorings.Models;

namespace Mordorings.Controls;

public class AutomapRenderer : MapRendererBase, IAutomapRenderer
{
    private DungeonFloor? _dungeonFloor;
    private Bitmap? _spriteSheet;

    public event EventHandler? MapUpdated;

    public void Initialize(DungeonFloor? dungeonFloor)
    {
        ArgumentNullException.ThrowIfNull(dungeonFloor);
        _dungeonFloor?.Dispose();
        _dungeonFloor = dungeonFloor;
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
        if (_dungeonFloor is null || MapGraphics is null || MapBuffer is null)
            throw new InvalidOperationException($"Map not initialized. Call {nameof(Initialize)} first.");
        MapGraphics.Clear(Color.Black);
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
        if (MapBuffer is not null)
            return new Bitmap(MapBuffer);
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

    public void HighlightTile(int tileX, int tileY)
    {
        if (MapGraphics is null)
            return;
        int screenTileY = MapHeight - 1 - tileY;
        int baseX = tileX * TileSize;
        int baseY = screenTileY * TileSize;
        Brush brush = new SolidBrush(Color.FromArgb(120, Color.Magenta));
        Pen pen = new(brush, 2);
        MapGraphics.DrawRectangle(pen, baseX, baseY, TileSize, TileSize);
        MapUpdated?.Invoke(this, EventArgs.Empty);
    }

    public void RemoveHighlight()
    {
        DrawDungeonFloorMap();
    }

    private void DrawMapBorders()
    {
        Pen pen = new(Color.White, 1);
        if (MapBuffer == null || MapGraphics == null)
            return;
        int x = MapBuffer.Width;
        int y = MapBuffer.Height;
        MapGraphics.DrawLine(pen, 0, 0, 0, y);
        MapGraphics.DrawLine(pen, 0, 0, x, 0);
        MapGraphics.DrawLine(pen, x - 1, y - 1, x - 1, 0);
        MapGraphics.DrawLine(pen, x - 1, y - 1, 0, y - 1);
    }

    private void DrawTileToBuffer(int tileX, int tileY, long bitmask)
    {
        if (_spriteSheet is null)
            throw new InvalidOperationException("Sprite sheet has not been loaded.");
        if (MapGraphics is null)
            return;
        int imageTileY = MapHeight - 1 - tileY;
        int baseX = tileX * TileSize;
        int baseY = imageTileY * TileSize;
        MapGraphics.FillRectangle(Brushes.Black, baseX, baseY, TileSize, TileSize);
        for (int i = 0; i <= 22; i++)
        {
            if ((bitmask & (1 << i)) != 0)
            {
                DrawSpriteLayer(MapGraphics, i, baseX, baseY);
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

    protected override void Dispose(bool disposing)
    {
        base.Dispose();
        _spriteSheet?.Dispose();
        _dungeonFloor?.Dispose();
    }
}
