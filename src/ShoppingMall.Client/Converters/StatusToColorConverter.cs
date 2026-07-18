using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ShoppingMall.Client.Converters;

public class StatusToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string s && !string.IsNullOrEmpty(s))
        {
            if (s.StartsWith("Error", StringComparison.OrdinalIgnoreCase) ||
                s.StartsWith("Failed", StringComparison.OrdinalIgnoreCase) ||
                s.Contains("error:", StringComparison.OrdinalIgnoreCase))
                return new SolidColorBrush(Color.FromRgb(220, 38, 38));
        }
        return new SolidColorBrush(Color.FromRgb(107, 114, 128));
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
