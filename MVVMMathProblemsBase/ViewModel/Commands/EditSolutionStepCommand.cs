using System;
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
            MMVM.ReplaceInCurrentProblemSolutionSteps(MMVM.CurrentSolutionStepText, MMVM.TempSolutionStepText);
            MMVM.ReloadTempSolutionSteps();
            MMVM.CurrentSolutionStepText = MMVM.TempSolutionStepText;
        }
    }
}
