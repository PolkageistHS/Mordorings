using System.Windows;
using System.Windows.Data;

namespace Mordorings.Controls;

[ValueConversion(typeof(object), typeof(Visibility))]
public class NullToHiddenVisibilityConverter : IValueConverter
{
    public static readonly NullToHiddenVisibilityConverter Instance = new();

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value == null ? Visibility.Hidden : Visibility.Visible;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
}
