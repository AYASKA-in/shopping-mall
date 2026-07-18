using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ShoppingMall.Client.Converters;

public class BoolToBrushConverter : IValueConverter
{
    public Brush? TrueBrush { get; set; }
    public Brush? FalseBrush { get; set; }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var b = value is bool bv && bv;
        if (parameter is string s && s.Equals("False", StringComparison.OrdinalIgnoreCase))
            b = !b;
        return b ? (TrueBrush ?? Brushes.Transparent) : (FalseBrush ?? Brushes.Transparent);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => DependencyProperty.UnsetValue;
}
