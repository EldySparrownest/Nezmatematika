using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data; // for IValueConverter

namespace MVVMMathProblemsBase.ViewModel.ValueConverters
{
    public class NullBoolToFeedackConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool? isCorrect = (bool?)value;
            if (isCorrect == true)
                return "SPRÁVNĚ!";
            if (isCorrect == false)
                return "Ještě to není správně - zkus to znovu.";
            return String.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string isCorrect = (string)value;
            if(isCorrect == "SPRÁVNĚ")
                return true;
            if (isCorrect == String.Empty)
                return null;
            return false;
        }
    }
}
