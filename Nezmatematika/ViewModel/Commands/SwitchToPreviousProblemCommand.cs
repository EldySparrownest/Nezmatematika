﻿using Nezmatematika.Model;
using System;
using System.Windows;
using System.Windows.Input;

namespace Nezmatematika.ViewModel.Commands
{
    public class SwitchToPreviousProblemCommand : ICommand
    {
        public MainMenuVM MMVM { get; set; }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public SwitchToPreviousProblemCommand(MainMenuVM vm)
        {
            MMVM = vm;
        }

        public bool CanExecute(object parameter)
        {
            if (App.WhereInApp != WhereInApp.CourseForStudent)
                return false;
            return MMVM.CurrentMathProblem != null && MMVM.CurrentMathProblemIndex > 0;
        }

        public void Execute(object parameter)
        {
            MMVM.ResetAnswerFeedbackVisibility();
            MMVM.BtnNextProblemVis = Visibility.Visible;
            MMVM.CurrentMathProblemIndex--;
            MMVM.SetCurrentMathProblemFromCurrentIndex();
        }
    }
}
