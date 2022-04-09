using System;
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
            if (App.WhereInApp != WhereInApp.CourseEditor)
                return false; 
            
            return MMVM.CurrentSolutionStepText != null && MMVM.CurrentSolutionStepText == parameter?.ToString();
        }

        public void Execute(object parameter)
        {
            MMVM.CurrentMathProblem.SolutionSteps.Remove(MMVM.CurrentSolutionStepText);
            MMVM.PopulateTempSolutionStepsFromCurrentMathProblem();
        }
    }
}
