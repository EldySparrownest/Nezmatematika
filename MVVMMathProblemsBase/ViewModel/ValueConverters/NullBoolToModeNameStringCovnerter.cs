using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Nezmatematika.ViewModel.ValueConverters
{
    public class NullBoolToModeNameStringCovnerter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool? isInStudentMode = (bool?)value;
            if (isInStudentMode == true)
                return "student";
            if (isInStudentMode == false)
                return "teacher";
            return String.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string modeName = (string)value;
            if (modeName == "student")
                return true;
            if (modeName == "teacher")
                return false;
            return null;
        }

        public static string Convert(object value)
        {
            bool? isInStudentMode = (bool?)value;
            if (isInStudentMode == true)
                return "student";
            if (isInStudentMode == false)
                return "teacher";
            return String.Empty;
        }

        public static bool? ConvertBack(object value)
        {
            string modeName = (string)value;
            if (modeName == "student")
                return true;
            if (modeName == "teacher")
                return false;
            return null;
        }
    }
}
