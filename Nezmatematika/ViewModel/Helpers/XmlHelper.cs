using System;
using System.IO;
using System.Xml.Serialization;

namespace Nezmatematika.ViewModel.Helpers
{
    public static class XmlHelper
    {
        public static void Save<T>(string fullFilePath, T item)
        {
            using (StreamWriter sw = new StreamWriter(fullFilePath))
            {
                XmlSerializer xmls = new XmlSerializer(typeof(T));
                xmls.Serialize(sw, item);
            }
        }

        public static bool TryDeserialiaze<T>(string fullFilePath, out T item)
        {
            item = default;
            if (File.Exists(fullFilePath))
            {
                using (StreamReader sw = new StreamReader(fullFilePath))
                {
                    XmlSerializer xmls = new XmlSerializer(typeof(T));
                    item = (T)xmls.Deserialize(sw);
                    return true;
                }
            }
            return false;
        }
    }
}
