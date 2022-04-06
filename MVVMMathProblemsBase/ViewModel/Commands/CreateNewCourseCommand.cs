using Nezmatematika.Model;
using System;
using System.Windows;
using System.Windows.Input;

namespace Nezmatematika.ViewModel.Commands
{
    public class CreateNewCourseCommand : ICommand
    {
        public MainMenuVM MMVM { get; set; }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public CreateNewCourseCommand(MainMenuVM vm)
        {
            MMVM = vm;
        }

        public bool CanExecute(object parameter)
        {
            return (string.IsNullOrEmpty(MMVM.TempCourseTitle) == false);
        }

        public void Execute(object parameter)
        {
            MMVM.CreateNewCourse();
            MMVM.MainMenuVis = Visibility.Collapsed;
            MMVM.NewCourseVis = Visibility.Collapsed;
            MMVM.EditCourseVis = Visibility.Visible;
            MMVM.CourseEditorMathProblemUserModeVis = Visibility.Visible;
            App.WhereInApp = WhereInApp.CourseEditor;
        }
    }
}
