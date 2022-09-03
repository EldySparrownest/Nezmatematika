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

        public User(UserBase userBase, string coursesDataFullfilePath, string settingsFullFilePath, string statsFullFilePath)
        {
            UserBase = userBase;
            ReadCoursesData(coursesDataFullfilePath);
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
            XmlHelper.Save(coursesDataFilename, CoursesData);
        }
        public void SaveSettings(string fullFilePath)
        {
            XmlHelper.Save(fullFilePath, UserSettings);
        }
        public void SaveStats(string statsFilename)
        {
            XmlHelper.Save(statsFilename, UserStats);
        }

        public void ReadCoursesData(string coursesDataFullFilePath)
        {
            XmlHelper.TryDeserialiaze<List<UserCourseData>>(coursesDataFullFilePath, out var coursesData);
            CoursesData = coursesData;
        }
        public void ReadSettings(string fullFilePath)
        {
            UserSettings = File.Exists(fullFilePath) ? UserSettings.Read(fullFilePath) : new UserSettings();
        }
        public void ReadStats(string statsFullFilePath)
        {
            XmlHelper.TryDeserialiaze<UserStats>(statsFullFilePath, out var userStats);
            UserStats = userStats;
        }
    }
}
