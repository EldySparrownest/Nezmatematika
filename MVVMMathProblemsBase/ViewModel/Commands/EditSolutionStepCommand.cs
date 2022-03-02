﻿using Nezmatematika.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Nezmatematika.ViewModel.Commands
{
    public class EditSolutionStepCommand : ICommand
    {
        public MainMenuVM MMVM { get; set; }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public EditSolutionStepCommand(MainMenuVM vm)
        {
            MMVM = vm;
        }

        public bool CanExecute(object parameter)
        {
            return MMVM.CurrentSolutionStepText != null
                && !String.IsNullOrWhiteSpace(MMVM.TempSolutionStepText) 
                && MMVM.CurrentSolutionStepText != MMVM.TempSolutionStepText;
        }

        public void Execute(object parameter)
        {
            MMVM.ReplaceInTempSolutionSteps(MMVM.CurrentSolutionStepText, MMVM.TempSolutionStepText);
            MMVM.ReloadSolutionSteps();
            MMVM.CurrentSolutionStepText = MMVM.TempSolutionStepText;
        }
    }
}