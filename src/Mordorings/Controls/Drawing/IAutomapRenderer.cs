using System.Drawing;

namespace Mordorings.Controls;

public interface IAutomapRenderer : IDisposable
{
    event EventHandler? MapUpdated;

    void Initialize(DungeonFloor floor);

    void LoadSpriteSheet(string? filePath);

    void LoadSpriteSheet(Bitmap? bitmap);

    Bitmap? GetMapSnapshot();

    void DrawDungeonFloorMap();

    bool UpdateTile(int x, int y, DungeonTileFlag newTileData);
}
