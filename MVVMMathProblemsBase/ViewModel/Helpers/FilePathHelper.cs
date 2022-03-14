using Nezmatematika.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nezmatematika.ViewModel.Helpers
{
    public static class FilePathHelper
    {
        public static string CourseFilename = "Course.xml";
        public static string UserSettingsFilename = "Settings.xml";
        public static string UserStatsFilename = "Stats.xml";
        public static string UserCoursesDataFilename = "CoursesData.xml";
        public static string UserListFilename = "UserList.xml";
        //_UserListPath bude obsahovat seznam uživatelů
        //na indexu [0] bude vždy poslední použitý student (pokud existuje)
        //na indexu [Count-1] bude vždy poslední použitý učitel (pokud existuje)
        public static string _SettingsDirPath() => Path.Combine(App.MyBaseDirectory, "Settings");
        public static string _UserListPath() => Path.Combine(_SettingsDirPath(), "UserList.xml");
        public static string _DefaultSettingsPath => Path.Combine(_SettingsDirPath(), $"Default{UserSettingsFilename}");

        public static string _CoursesDirPath() => Path.Combine(App.MyBaseDirectory, "Courses");
        public static string _CoursesArchivedDirPath() => Path.Combine(_CoursesDirPath(), "Archived");
        public static string _CoursesPublishedDirPath() => Path.Combine(_CoursesDirPath(), "Published");

        public static string _UserCoursesDataPath(UserBase userBase) => Path.Combine(_SettingsDirPath(), $"{userBase.Id}{UserCoursesDataFilename}");
        public static string _UserSettingsPath(UserBase userBase) => Path.Combine(_SettingsDirPath(), $"{userBase.Id}{UserSettingsFilename}");
        public static string _UserStatsPath(UserBase userBase) => Path.Combine(_SettingsDirPath(), $"{userBase.Id}{UserStatsFilename}");
        public static string _TeacherCoursesDirPath(UserBase userBase) => Path.Combine(_CoursesDirPath(), userBase.Id);

        public static string _ExportsDirPath() => Path.Combine(App.MyBaseDirectory, "Exports");
    }
}
