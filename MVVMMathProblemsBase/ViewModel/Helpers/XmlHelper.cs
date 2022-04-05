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
        public static void Save<T>(string fullFilePath, Type type, T item)
        {
            using (StreamWriter sw = new StreamWriter(fullFilePath))
            {
                XmlSerializer xmls = new XmlSerializer(type);
                xmls.Serialize(sw, item);
            }
        }
    }
}
