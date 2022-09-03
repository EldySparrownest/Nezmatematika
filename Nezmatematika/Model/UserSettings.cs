using Nezmatematika.ViewModel.Helpers;
using System.IO;
using System.Windows.Media;
using System.Xml.Serialization;

namespace Nezmatematika.Model
{
    public class UserSettings
    {
        public bool HasCourseToContinue { get; set; }
        public Color MainBackgroundColour { get; set; }
        public Color SecondaryBackgroundColour { get; set; }

        //Student only section
        public bool RequeueOnMistake { get; set; }

        //Teacher only section
        public bool AutosaveProblemWhenSwitching { get; set; }
        public bool AutosaveCourseBeforePublishing { get; set; }
        public bool CapitalisationMatters { get; set; }
        public string DefaultFontFamily { get; set; }
        public int DefaultFontSize { get; set; }

        public void Save(string fullFilePath)
        {
            XmlHelper.Save(fullFilePath, this);
        }
        public static UserSettings Read(string fullFilePath)
        {
            XmlHelper.TryDeserialiaze<UserSettings>(fullFilePath, out var userSettings);
            return userSettings;
        }
    }
}
