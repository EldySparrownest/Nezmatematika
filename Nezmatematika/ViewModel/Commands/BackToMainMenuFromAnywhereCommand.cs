﻿using Nezmatematika.Model;
using System;
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
            if (App.WhereInApp == WhereInApp.Settings)
                MMVM.LoadCurrentSettingsForCurrentUser();
            
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
