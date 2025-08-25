using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Mordorings.Controls;

public static class AutomapEventConverters
{
    public static (int X, int Y) GetCoordinatesFromEvent(object? eventParameter)
    {
        if (eventParameter is not MouseButtonEventArgs { Source: Image image } args)
            return (X: 0, Y: 0);
        double scale = image.ActualWidth / 421;
        Point pos = args.GetPosition(image);
        return (X: (int)(pos.X / scale / 14), Y: 29 - (int)(pos.Y / scale / 14));
    }
}
