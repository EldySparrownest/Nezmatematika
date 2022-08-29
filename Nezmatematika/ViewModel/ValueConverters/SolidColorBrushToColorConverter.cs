using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data; // for IVAlueConverter
using System.Windows.Media; // for Color

namespace Nezmatematika.ViewModel.ValueConverters
{
    public class SolidColorBrushToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SolidColorBrush brush = (SolidColorBrush)value;
            if (brush == null)
            {
                return SystemColors.WindowColor;
            }
            Color colour = brush.Color;
            return colour;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color? colour = (Color?)value;
            if (colour == null)
            {
                colour = SystemColors.WindowColor;
            }
            return new SolidColorBrush((Color)colour);
        }

        public static Color Convert(object value)
        {
            SolidColorBrush solidColorBrush = (SolidColorBrush)value;
            return solidColorBrush.Color;
        }

        public static Brush ConvertBack(object value)
        {
            Color color = (Color)value;
            return new SolidColorBrush((Color)value);
        }
    }
}
