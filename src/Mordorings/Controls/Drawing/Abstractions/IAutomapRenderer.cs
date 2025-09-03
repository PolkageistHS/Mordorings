using System.Drawing;

namespace Mordorings.Controls;

public interface IAutomapRenderer : IMapRendererBase, IDisposable
{
    event EventHandler? MapUpdated;

    void Initialize(DungeonFloor floor);

    void LoadSpriteSheet(string? filePath);

    void LoadSpriteSheet(Bitmap? bitmap);

    void DrawDungeonFloorMap();

    bool UpdateTile(Tile tile, DungeonTileFlag newTileData);

    void HighlightTile(Tile tile);

    void RemoveHighlight();
}
