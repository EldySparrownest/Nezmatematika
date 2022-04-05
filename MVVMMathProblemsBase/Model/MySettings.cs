using Nezmatematika.ViewModel.Helpers;
using System.IO;
using System.Windows.Media;
using System.Xml.Serialization;

namespace Nezmatematika.Model
{
    public class MySettings
    {
        public UserBase ThisUser { get; set; }
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
            XmlHelper.Save(fullFilePath, typeof(MySettings), this);
        }
        public static MySettings Read(string filename)
        {
            using (StreamReader sr = new StreamReader(filename))
            {
                XmlSerializer xmls = new XmlSerializer(typeof(MySettings));
                return xmls.Deserialize(sr) as MySettings;
            }
        }
    }
}
