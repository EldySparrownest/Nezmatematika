using System;
using System.Windows;
using System.Windows.Input;

namespace Nezmatematika.ViewModel.Commands
{
    public class DisplayNewCourseSelectionCommand : ICommand
    {
        public MainMenuVM MMVM { get; set; }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public DisplayNewCourseSelectionCommand(MainMenuVM vm)
        {
            MMVM = vm;
        }

        public bool CanExecute(object parameter)
        {
            if (MMVM.CurrentUser != null && MMVM.IsInStudentMode == true)
            {
                return true;
            }
            return false;
        }

        public void Execute(object parameter)
        {
            MMVM.BackToMainMenu();
            MMVM.ClearCurrentValuesExceptUser();
            MMVM.GetListOfNewCoursesToStart();
            MMVM.StudentNewCourseSelVis = Visibility.Visible;
        }
    }
}
