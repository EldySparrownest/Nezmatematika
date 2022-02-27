using Nezmatematika.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Nezmatematika.ViewModel.Commands
{
    public class RemoveSolutionStepCommand : ICommand
    {
        public MainMenuVM MMVM { get; set; }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RemoveSolutionStepCommand(MainMenuVM vm)
        {
            MMVM = vm;
        }

        public bool CanExecute(object parameter)
        {
            return MMVM.CurrentSolutionStepText != null && MMVM.CurrentSolutionStepText == parameter?.ToString();
        }

        public void Execute(object parameter)
        {
            MMVM.TempSolutionStepsTexts.Remove(MMVM.CurrentSolutionStepText);
            MMVM.ReloadSolutionSteps();
        }
    }
}
