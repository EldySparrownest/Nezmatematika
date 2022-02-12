using System;
using System.Windows;
using System.Windows.Input;

namespace Nezmatematika.ViewModel.Commands
{
    public class FinishTakingCourseCommand : ICommand
    {
        public MainMenuVM MMVM { get; set; }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public FinishTakingCourseCommand(MainMenuVM vm)
        {
            MMVM = vm;
        }

        public bool CanExecute(object parameter)
        {
            if (App.WhereInApp != WhereInApp.CourseForStudent)
                return false;
            return MMVM.CurrentMathProblem != null && MMVM.BtnFinishCourseVis == Visibility.Visible;
        }

        public void Execute(object parameter)
        {
            MMVM.CurrentUserCourseData.UpdateWhenCourseCompleted();
            MMVM.BackToMainMenu();
        }
    }
}
