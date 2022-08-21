using System;
using System.IO;
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
