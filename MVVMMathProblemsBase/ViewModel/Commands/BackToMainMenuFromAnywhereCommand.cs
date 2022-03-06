using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Nezmatematika.ViewModel.Commands
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
            if (MMVM.IsInStudentMode == true && App.WhereInApp == WhereInApp.CourseForStudent)
            {
                MMVM.CurrentUserCourseData.UpdateAtSessionEnd(out TimeSpan sessionDuration);
                MMVM.CurrentUser.UserStats.SessionEndUpdate(sessionDuration);
                MMVM.SaveDataAndStats();
            }
            
            MMVM.BackToMainMenu();
            MMVM.ClearCurrentValuesExceptUser();
        }
    }
}
