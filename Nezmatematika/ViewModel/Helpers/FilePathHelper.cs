﻿using Nezmatematika.Model;
using System;
using System.IO;

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
                    if (userBase == null)
                        throw new ArgumentNullException(nameof(userBase));
                    return Path.Combine(App.MyBaseDirectory, _TeacherCoursesRelDirPath(userBase));
                case FullPathOptions.DirExports:
                    return Path.Combine(App.MyBaseDirectory, _ExportsDirName());
                case FullPathOptions.DirUsers:
                    return Path.Combine(App.MyBaseDirectory, _UsersDirName());
                case FullPathOptions.FileUserCoursesData:
                    if (userBase == null)
                        throw new ArgumentNullException(nameof(userBase));
                    return Path.Combine(App.MyBaseDirectory, _UserCoursesDataRelFilePath(userBase));
                case FullPathOptions.FileUserSettings:
                    if (userBase == null)
                        throw new ArgumentNullException(nameof(userBase));
                    return Path.Combine(App.MyBaseDirectory, _UserSettingsRelFilePath(userBase));
                case FullPathOptions.FileUserStats:
                    if (userBase == null)
                        throw new ArgumentNullException(nameof(userBase));
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
        public static string _CoursesDirName() => "Courses";
        public static string _ExportsDirName() => "Exports";
        public static string _UsersDirName() => "Users";

        public static string _UserListFullPath() => Path.Combine(App.MyBaseDirectory, _UsersDirName(), "UserList.xml");
        public static string _DefaultSettingsFullPath => Path.Combine(App.MyBaseDirectory, _UsersDirName(), $"Default{UserSettingsFilename}");

        public static string _CoursesArchivedRelDirPath() => Path.Combine(_CoursesDirName(), "Archived");
        public static string _CoursesPublishedRelDirPath() => Path.Combine(_CoursesDirName(), "Published");
        public static string _TeacherCoursesRelDirPath(UserBase userBase) => Path.Combine(_CoursesDirName(), userBase.Id);
        public static string _UserCoursesDataRelFilePath(UserBase userBase) => Path.Combine(_UsersDirName(), $"{userBase.Id}{UserCoursesDataFilename}");
        public static string _UserSettingsRelFilePath(UserBase userBase) => Path.Combine(_UsersDirName(), $"{userBase.Id}{UserSettingsFilename}");
        public static string _UserStatsRelFilePath(UserBase userBase) => Path.Combine(_UsersDirName(), $"{userBase.Id}{UserStatsFilename}");
    }
}

public enum FullPathOptions
{
    DirCourses,
    DirCoursesArchived,
    DirCoursesPublished,
    DirCoursesTeacher,
    DirExports,
    DirUsers,
    FileUserCoursesData,
    FileUserSettings,
    FileUserStats
}
