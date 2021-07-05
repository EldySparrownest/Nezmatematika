using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows; // for Visibility
using System.Windows.Data; // for IVAlueConverter

namespace MVVMMathProblemsBase.ViewModel.ValueConverters
{
    public class NullBoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //true = Visible, false = Hidden, null = Collapsed           
            bool? visible = (bool?) value;
            if (visible == true)
                return Visibility.Visible;
            if (visible == false)
                return Visibility.Hidden;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility visibility = (Visibility)value;
            if (visibility == Visibility.Visible)
                return true;
            if (visibility == Visibility.Hidden)
                return false;
            return null;

        }

        public static Visibility Convert(object value)
        {
            bool? visible = (bool?)value;
            if (visible == true)
                return Visibility.Visible;
            if (visible == false)
                return Visibility.Hidden;
            return Visibility.Collapsed;
        }

        public static bool? ConvertBack(object value)
        {
            Visibility visibility = (Visibility)value;
            if (visibility == Visibility.Visible)
                return true;
            if (visibility == Visibility.Hidden)
                return false;
            return null;

        }
    }
}
