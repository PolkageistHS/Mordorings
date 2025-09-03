using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Mordorings.Controls;

public static class AutomapEventConversion
{
    /// <summary>
    /// Returns array-based (0-29) X/Y coordinates.
    /// </summary>
    public static Tile GetMapCoordinatesFromEvent(object? eventParameter)
    {
        if (eventParameter is not MouseEventArgs { Source: Image image } args)
            return new Tile(0, 0);
        double scale = image.ActualWidth / 30;
        Point pos = args.GetPosition(image);
        int x = Math.Clamp((int)(pos.X / scale), 0, 29);
        int y = Math.Clamp(29 - (int)(pos.Y / scale), 0, 29);
        return new Tile(x, y);
    }
}
