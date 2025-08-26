using System.Globalization;
using System.Windows.Data;

namespace Mordorings.Controls;

public class StringToIntConverter : IValueConverter
{
    public int EmptyStringValue { get; set; }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            null => null,
            string => value,
            int i when i == EmptyStringValue => string.Empty,
            _ => value.ToString()
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string s)
            return value;
        if (string.IsNullOrWhiteSpace(s))
            return EmptyStringValue;
        if ((targetType == typeof(int) || targetType == typeof(int?)) && int.TryParse(s, out int i))
            return i;
        if ((targetType == typeof(short) || targetType == typeof(short?)) && short.TryParse(s, out short j))
            return j;
        return EmptyStringValue;
    }
}
