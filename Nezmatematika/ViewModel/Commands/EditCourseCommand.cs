﻿using Nezmatematika.Model;
using System;
using System.Windows;
using System.Windows.Input;

namespace Nezmatematika.ViewModel.Commands
{
    public class EditCourseCommand : ICommand
    {
        public MainMenuVM MMVM { get; set; }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public EditCourseCommand(MainMenuVM vm)
        {
            MMVM = vm;
        }

        public bool CanExecute(object parameter)
        {
            if (MMVM.CurrentUser != null && MMVM.IsInStudentMode == false && MMVM.CurrentCourse != null)
            {
                return true;
            }
            return false;
        }

        public void Execute(object parameter)
        {
            MMVM.BackToMainMenu();
            MMVM.MainMenuVis = Visibility.Collapsed;
            App.WhereInApp = WhereInApp.CourseEditor;
            MMVM.StartEditingCurrentCourse();
        }
    }
}
