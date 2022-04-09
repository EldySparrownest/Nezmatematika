using System;
using System.Windows;

namespace Nezmatematika
{
    public enum WhereInApp { ModeSelection, MainMenu, CourseForStudent, CourseEditor, Statistics, Settings };
    public enum AppMode { Student, Teacher, Unselected };

    /// <summary>
    /// Interakční logika pro App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static string myBaseDirectory = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "HopefulSparrow", "Nezmatematika");
        public static string MyBaseDirectory { get { return myBaseDirectory; } }

        private static AppMode appMode;
        public static AppMode AppMode
        {
            get { return appMode; }
            set { appMode = value; }
        }

        private static WhereInApp whereInApp;
        public static WhereInApp WhereInApp
        {
            get { return whereInApp; }
            set { whereInApp = value; }
        }
    }
}
