using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data; // for IVAlueConverter
using System.Windows.Media; // for Color

namespace Nezmatematika.ViewModel.ValueConverters
{
    public class NullableColorToSolidColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color? colour = (Color?)value;
            if (colour == null)
                return new SolidColorBrush(SystemColors.WindowColor);
            else
                return new SolidColorBrush((Color)colour);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SolidColorBrush brush = (SolidColorBrush)value;
            Color? colour = brush.Color;
            return colour;
        }

        public static Brush Convert(object value)
        {
            Color? color = (Color?)value;
            if (color == null)
                return new SolidColorBrush(SystemColors.WindowColor);
            else
                return new SolidColorBrush((Color)value);
        }

        public static Color? ConvertBack(object value)
        {
            SolidColorBrush solidColorBrush = (SolidColorBrush)value;
            return solidColorBrush.Color;

        }
    }
}
