using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml.Serialization;

namespace MVVMMathProblemsBase.Model
{
    public class MySettings
    {
        public User ThisUser { get; set; }
        public bool HasCourseToContinue { get; set; }
        public Color MainBackgroundColour { get; set; }
        public Color SecondaryBackgroundColour { get; set; }
        public List<string> Setting2 { get; set; }

        //Student only section
        public bool RequeueOnMistake { get; set; }

        //Teacher only section
        public bool AutosaveProblemWhenSwitching { get; set; }
        public bool AutosaveCourseBeforePublishing { get; set; }

        public void Save(string filename)
        {
            using (StreamWriter sw = new StreamWriter(filename))
            {
                XmlSerializer xmls = new XmlSerializer(typeof(MySettings));
                xmls.Serialize(sw, this);
            }
        }
        public MySettings Read(string filename)
        {
            using (StreamReader sr = new StreamReader(filename))
            {
                XmlSerializer xmls = new XmlSerializer(typeof(MySettings));
                return xmls.Deserialize(sr) as MySettings;
            }
        }
    }
}
