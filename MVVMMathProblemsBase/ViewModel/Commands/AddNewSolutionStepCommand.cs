using Nezmatematika.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Nezmatematika.ViewModel.Commands
{
    public class AddNewSolutionStepCommand : ICommand
    {
        public MainMenuVM MMVM { get; set; }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public AddNewSolutionStepCommand(MainMenuVM vm)
        {
            MMVM = vm;
        }

        public bool CanExecute(object parameter)
        {
            return App.WhereInApp == WhereInApp.CourseEditor 
                && MMVM.CurrentMathProblem != null
                && !String.IsNullOrWhiteSpace(MMVM.TempSolutionStepText);
        }

        public void Execute(object parameter)
        {
            MMVM.TempSolutionStepsTexts.Add(parameter.ToString());
            MMVM.ReloadSolutionSteps();
            MMVM.TempSolutionStepText = string.Empty;
        }
    }
}
