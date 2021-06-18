using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace MVVMMathProblemsBase.ViewModel.ValueConverters
{
    class NullBoolToSolidColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool? isCorrect = (bool?)value;
            SolidColorBrush scb;

            if (isCorrect == true)
            {
                scb = new SolidColorBrush(Colors.LawnGreen);
            } 
            else if (isCorrect == false)
            {
                scb = new SolidColorBrush(Colors.OrangeRed);
            }
            else
            {
                scb = new SolidColorBrush(Colors.Transparent);
            }
            return scb;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SolidColorBrush scb = (SolidColorBrush)value;
            if (scb.Color == Colors.OrangeRed)
            {
                return false;
            }
            else if (scb.Color == Colors.LawnGreen)
            {
                return true;
            }
            return null;
        }
    }
}
