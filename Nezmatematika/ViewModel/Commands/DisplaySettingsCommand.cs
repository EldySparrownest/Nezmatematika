﻿using Nezmatematika.Model;
using System;
using System.Windows;
using System.Windows.Input;

namespace Nezmatematika.ViewModel.Commands
{
    public class DisplaySettingsCommand : ICommand
    {
        public MainMenuVM MMVM { get; set; }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public DisplaySettingsCommand(MainMenuVM vm)
        {
            MMVM = vm;
        }

        public bool CanExecute(object parameter)
        {
            if (MMVM.CurrentUser != null)
            {
                return true;
            }
            return false;
        }

        public void Execute(object parameter)
        {
            MMVM.BackToMainMenu();
            App.WhereInApp = WhereInApp.Settings;

            if (MMVM.IsInStudentMode == true)
                MMVM.StudentSettingsVis = Visibility.Visible;
            else
                MMVM.TeacherSettingsVis = Visibility.Visible;

            MMVM.SettingsVis = Visibility.Visible;
        }
    }
}
