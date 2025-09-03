using System.Globalization;
using System.Windows.Data;

namespace Mordorings.Controls;

[ValueConversion(typeof(object), typeof(bool))]
public class NullToBooleanConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool retValueIfNull = false;
        if (parameter is string param)
        {
            retValueIfNull = param.Equals(true.ToString(), StringComparison.InvariantCultureIgnoreCase);
        }
        if (value == null || (value is string val && string.IsNullOrEmpty(val)))
            return retValueIfNull;
        return !retValueIfNull;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
}