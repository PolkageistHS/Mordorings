using System.Drawing;

namespace Mordorings.Controls;

public class AutomapRenderer : MapRendererBase, IAutomapRenderer
{
    private DungeonFloor? _dungeonFloor;
    private Bitmap? _spriteSheet;

    public event EventHandler? MapUpdated;

    public void Initialize(DungeonFloor? dungeonFloor)
    {
        ArgumentNullException.ThrowIfNull(dungeonFloor);
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
        if (_dungeonFloor is null)
            throw new InvalidOperationException($"Map not initialized. Call {nameof(Initialize)} first.");
        MapGraphics.Clear(Color.Black);
        for (int x = MapWidthInTiles - 1; x >= 0; x--)
        {
            for (int y = MapHeightInTiles - 1; y >= 0; y--)
            {
                DrawTileToBuffer(x, y, _dungeonFloor.Tiles[x, y]);
            }
        }
        FillGutters();
        DrawTileNumbers();
        DrawMapBorders();
        MapUpdated?.Invoke(this, EventArgs.Empty);
    }

    public bool UpdateTile(Tile tile, DungeonTileFlag newTileData)
    {
        long tileData = (long)newTileData;
        int x = tile.X;
        int y = tile.Y;
        if (_dungeonFloor is null || x is < 0 or >= MapWidthInTiles || y is < 0 or >= MapHeightInTiles)
            return false;
        if (_dungeonFloor.Tiles[x, y] == tileData)
            return false;
        _dungeonFloor.Tiles[x, y] = tileData;
        DrawDungeonFloorMap();
        HighlightTile(tile);
        return true;
    }

    public void HighlightTile(Tile tile)
    {
        DrawTileBorder(tile, Color.Magenta, 120);
        MapUpdated?.Invoke(this, EventArgs.Empty);
    }

    public void RemoveHighlight()
    {
        DrawDungeonFloorMap();
    }

    private void DrawTileToBuffer(int tileX, int tileY, long bitmask)
    {
        if (_spriteSheet is null)
            throw new InvalidOperationException("Sprite sheet has not been loaded.");
        int imageTileY = MapHeightInTiles - 1 - tileY;
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
        var destRect = new Rectangle(baseX + offsetX + LeftGutterWidth, baseY + offsetY, spriteTile, spriteTile);
        graphics.DrawImage(_spriteSheet, destRect, sourceRect, GraphicsUnit.Pixel);
    }

    private void DrawMapBorders()
    {
        var pen = new Pen(Color.White, 1);
        MapGraphics.DrawRectangle(pen, LeftGutterWidth, 0, ImagePixelSize.Width, ImagePixelSize.Height - BottomGutterHeight);
    }

    private void FillGutters()
    {
        Brush brush = Brushes.Silver;
        MapGraphics.FillRectangle(brush, 0, 0, LeftGutterWidth, ImagePixelSize.Height);
        MapGraphics.FillRectangle(brush, 0, ImagePixelSize.Height - BottomGutterHeight - 1, ImagePixelSize.Width, ImagePixelSize.Height);
    }

    private void DrawTileNumbers()
    {
        var font = new Font("Consolas", 9.5f, FontStyle.Bold, GraphicsUnit.Pixel);
        Brush brush = Brushes.Black;
        for (int i = 2; i <= MapWidthInTiles; i += 2)
        {
            MapGraphics.DrawString(i.ToString().PadLeft(2, ' '), font, brush, LeftGutterWidth + TileSize * (i - 1), MapPixelSize.Height - 1);
        }
        for (int i = MapHeightInTiles; i >= 2; i -= 2)
        {
            MapGraphics.DrawString(i.ToString().PadLeft(2, ' '), font, brush, 0, MapPixelSize.Height - TileSize * (i));
        }
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _spriteSheet?.Dispose();
    }
}
