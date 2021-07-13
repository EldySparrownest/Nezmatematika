using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MVVMMathProblemsBase.ViewModel.Commands
{
    public class BackToMainMenuFromAnywhereCommand : ICommand
    {
        public MainMenuVM MMVM { get; set; }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public BackToMainMenuFromAnywhereCommand(MainMenuVM vm)
        {
            MMVM = vm;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            MMVM.ClearTempValues();
            MMVM.EditCourseVis = Visibility.Collapsed;
            MMVM.EditUserVis = Visibility.Collapsed;
            MMVM.NewCourseVis = Visibility.Collapsed;
            MMVM.NewUserVis = Visibility.Collapsed;
            MMVM.SettingsVis = Visibility.Collapsed;
            MMVM.UserSelVis = Visibility.Collapsed;

            if (MMVM.IsInStudentMode == true)
            {
                MMVM.StudentVis = Visibility.Visible;
                MMVM.TeacherVis = Visibility.Collapsed;
            }
            else
            {
                MMVM.StudentVis = Visibility.Collapsed;
                MMVM.TeacherVis = Visibility.Visible;
            }
        }
    }
}
