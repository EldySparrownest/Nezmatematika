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
        public static string GetFullPath(FullPathOptions option, UserBase userBase = null)
        {
            switch (option)
            {
                case FullPathOptions.DirCourses:
                    return Path.Combine(App.MyBaseDirectory, _CoursesDirName());
                case FullPathOptions.DirCoursesArchived:
                    return Path.Combine(App.MyBaseDirectory, _CoursesArchivedRelDirPath());
                case FullPathOptions.DirCoursesPublished:
                    return Path.Combine(App.MyBaseDirectory, _CoursesPublishedRelDirPath());
                case FullPathOptions.DirCoursesTeacher:
                    return Path.Combine(App.MyBaseDirectory, _TeacherCoursesRelDirPath(userBase));
                case FullPathOptions.DirUsers:
                    return Path.Combine(App.MyBaseDirectory, _SettingsDirName());
                case FullPathOptions.FileUserCoursesData:
                    return Path.Combine(App.MyBaseDirectory, _UserCoursesDataRelFilePath(userBase));
                case FullPathOptions.FileUserStats:
                    return Path.Combine(App.MyBaseDirectory, _UserStatsRelFilePath(userBase));
                default:
                    return App.MyBaseDirectory;
            };
        }
        
        public static string CourseFilename = "Course.xml";
        public static string UserSettingsFilename = "Settings.xml";
        public static string UserStatsFilename = "Stats.xml";
        public static string UserCoursesDataFilename = "CoursesData.xml";
        public static string UserListFilename = "UserList.xml";
        //_UserListPath bude obsahovat seznam uživatelů
        //na indexu [0] bude vždy poslední použitý student (pokud existuje)
        //na indexu [Count-1] bude vždy poslední použitý učitel (pokud existuje)
        public static string _SettingsDirName() => "Settings";
        public static string _UserListFullPath() => Path.Combine(App.MyBaseDirectory, _SettingsDirName(), "UserList.xml");
        public static string _DefaultSettingsFullPath => Path.Combine(App.MyBaseDirectory, _SettingsDirName(), $"Default{UserSettingsFilename}");

        public static string _CoursesDirName() => "Courses";
        public static string _CoursesArchivedRelDirPath() => Path.Combine(_CoursesDirName(), "Archived");
        public static string _CoursesPublishedRelDirPath() => Path.Combine(_CoursesDirName(), "Published");

        public static string _UserCoursesDataRelFilePath(UserBase userBase) => Path.Combine(_SettingsDirName(), $"{userBase.Id}{UserCoursesDataFilename}");
        public static string _UserSettingsRelFilePath(UserBase userBase) => Path.Combine(_SettingsDirName(), $"{userBase.Id}{UserSettingsFilename}");
        public static string _UserStatsRelFilePath(UserBase userBase) => Path.Combine(_SettingsDirName(), $"{userBase.Id}{UserStatsFilename}");
        public static string _TeacherCoursesRelDirPath(UserBase userBase) => Path.Combine(_CoursesDirName(), userBase.Id);

        public static string _ExportsDirName() => "Exports";
    }
}

public enum FullPathOptions
{
    DirCourses,
    DirCoursesArchived,
    DirCoursesPublished,
    DirCoursesTeacher,
    DirUsers,
    FileUserCoursesData,
    FileUserStats
}
