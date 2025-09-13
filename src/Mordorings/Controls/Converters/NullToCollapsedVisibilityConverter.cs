using System.Windows;
using System.Windows.Data;

namespace Mordorings.Controls;

[ValueConversion(typeof(object), typeof(Visibility))]
public class NullToCollapsedVisibilityConverter : IValueConverter
{
    public static readonly NullToCollapsedVisibilityConverter Instance = new();

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value == null ? Visibility.Collapsed : Visibility.Visible;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
}
