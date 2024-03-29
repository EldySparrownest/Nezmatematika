﻿using System;
using System.Windows.Input;

namespace Nezmatematika.ViewModel.Commands
{
    public class AddNewProblemCommand : ICommand
    {
        public MainMenuVM MMVM { get; set; }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public AddNewProblemCommand(MainMenuVM vm)
        {
            MMVM = vm;
        }

        public bool CanExecute(object parameter)
        {
            return App.WhereInApp == WhereInApp.CourseEditor && MMVM.CurrentCourse != null;
        }

        public void Execute(object parameter)
        {
            var index = MMVM.CurrentMathProblem.Index;
            MMVM.CurrentCourse.AddNewMathProblem(MMVM.CurrentSettings.CapitalisationMatters);
            MMVM.PopulateTempMathProblemsFromCurrentCourse();
            MMVM.CurrentMathProblem = MMVM.CurrentCourse.Problems[index];
        }
    }
}
