using System.Windows.Input;
using Image = System.Windows.Controls.Image;
using Point = System.Windows.Point;

namespace Mordorings.Controls;

public static class AutomapEventConversion
{
    /// <summary>
    /// Returns array-based (0-29) X/Y coordinates.
    /// </summary>
    public static Tile GetMapCoordinatesFromEvent(object? eventParameter)
    {
        if (eventParameter is not MouseEventArgs { Source: Image image } args)
            return new Tile(-1, -1);
        double imageScale = image.ActualWidth / MapRendererBase.ImagePixelSize.Width;
        Point pos = args.GetPosition(image);
        double scaledtileSize = imageScale * MapRendererBase.TileSize;
        double scaledGutterSize = imageScale * MapRendererBase.LeftGutterWidth;
        int x;
        double startX = pos.X - scaledGutterSize;
        if (startX < 0)
        {
            x = -1;
        }
        else
        {
            x = Math.Clamp((int)(startX / scaledtileSize), -1, 29);
        }
        int y = Math.Clamp(29 - (int)(pos.Y / scaledtileSize), -1, 29);
        return new Tile(x, y);
    }
}
