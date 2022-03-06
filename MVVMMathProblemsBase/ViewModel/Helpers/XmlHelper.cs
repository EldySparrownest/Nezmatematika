using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Nezmatematika.ViewModel.Helpers
{
    public static class XmlHelper
    {
        public static void Save<T>(string filename, Type type, T item)
        {
            using (StreamWriter sw = new StreamWriter(filename))
            {
                XmlSerializer xmls = new XmlSerializer(type);
                xmls.Serialize(sw, item);
            }
        }

        public static object Read(string filename, Type type)
        {
            using (StreamReader sr = new StreamReader(filename))
            {
                XmlSerializer xmls = new XmlSerializer(type);
                return xmls.Deserialize(sr) as object;
            }
        }
    }
}
