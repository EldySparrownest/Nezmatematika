using Nezmatematika.ViewModel.Helpers;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Nezmatematika.Model
{
    public class User
    {
        public UserBase UserBase { get; set; }
        public UserSettings UserSettings {get; set;}
        public UserStats UserStats { get; set; }
        public List<UserCourseData> CoursesData { get; set; }

        public User()
        {
            UserBase = new UserBase();
            CoursesData = new List<UserCourseData>();
            UserStats = new UserStats();
        }

        public User(string titBef, string fName, string lName, string titAft, string sName, string cName)
        {
            UserBase = new UserBase(titBef, fName, lName, titAft, sName, cName);
            CoursesData = new List<UserCourseData>();
            UserSettings = new UserSettings();
            UserStats = new UserStats();
        }

        public User(UserBase userBase, string coursesDataFilename, string settingsFullFilePath, string statsFullFilePath)
        {
            UserBase = userBase;
            ReadCoursesData(coursesDataFilename);
            ReadSettings(settingsFullFilePath);
            ReadStats(statsFullFilePath);
        }

        public void SaveDataAndStats(string coursesDataFilename, string statsFilename)
        {
            SaveCoursesData(coursesDataFilename);
            SaveStats(statsFilename);
        }

        public void SaveCoursesData(string coursesDataFilename)
        {
            XmlHelper.Save(coursesDataFilename, typeof(List<UserCourseData>), CoursesData);
        }
        public void SaveSettings(string fullFilePath)
        {
            XmlHelper.Save(fullFilePath, typeof(UserSettings), UserSettings);
        }
        public void SaveStats(string statsFilename)
        {
            XmlHelper.Save(statsFilename, typeof(UserStats), UserStats);
        }

        public void ReadCoursesData(string coursesDataFilename)
        {
            if (File.Exists(coursesDataFilename))
            {
                using (StreamReader sw = new StreamReader(coursesDataFilename))
                {
                    XmlSerializer xmls = new XmlSerializer(typeof(List<UserCourseData>));
                    CoursesData = xmls.Deserialize(sw) as List<UserCourseData>;
                }
            }
            else CoursesData = new List<UserCourseData>();
        }
        public void ReadSettings(string fullFilePath)
        {
            UserSettings = File.Exists(fullFilePath) ? UserSettings.Read(fullFilePath) : new UserSettings();
        }
        public void ReadStats(string statsFilename)
        {
            if (File.Exists(statsFilename))
            {
                using (StreamReader sw = new StreamReader(statsFilename))
                {
                    XmlSerializer xmls = new XmlSerializer(typeof(UserStats));
                    UserStats = xmls.Deserialize(sw) as UserStats;
                }
            }
            else UserStats = new UserStats();
        }
    }
}
