﻿using System;
using System.Windows;
using System.Windows.Input;

namespace Nezmatematika.ViewModel.Commands
{
    public class DisplayStudentCoursesToContinue : ICommand
    {
        public MainMenuVM MMVM { get; set; }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public DisplayStudentCoursesToContinue(MainMenuVM vm)
        {
            MMVM = vm;
        }

        public bool CanExecute(object parameter)
        {
            if (MMVM.CurrentUser != null && MMVM.IsInStudentMode == true && MMVM.CurrentSettings.HasCourseToContinue)
            {
                return true;
            }
            return false;
        }

        public void Execute(object parameter)
        {
            MMVM.BackToMainMenu();
            MMVM.ClearCurrentValuesExceptUser();
            MMVM.GetListOfStudentCoursesToContinue();
            MMVM.StudentCoursesToContinueVis = Visibility.Visible;
        }
    }
}
