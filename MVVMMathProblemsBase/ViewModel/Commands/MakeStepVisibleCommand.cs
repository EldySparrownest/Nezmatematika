using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input; // for ICommand
using System.Windows; // for Visibility

namespace MVVMMathProblemsBase.ViewModel.Commands
{
    public class MakeStepVisibleCommand : ICommand
    {
        public MathProblemVM MPVM { get; set; }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        public MakeStepVisibleCommand(MathProblemVM vm)
        {
            MPVM = vm;
        }

        public bool CanExecute(object parameter)
        {
            int lastvis = MPVM.CurrentMathProblem.FindLastVisibleStep();
            if (lastvis < MPVM.CurrentMathProblem.SolutionSteps.Count)
            {
                return true;
            }
            return false;
        }

        public void Execute(object parameter)
        {
            int lastvis = MPVM.CurrentMathProblem.FindLastVisibleStep();
            MPVM.VisibleSteps.Add(MPVM.CurrentMathProblem.SolutionSteps[lastvis]);
            MPVM.CurrentMathProblem.SetVisibilityOfStepOnIndex(lastvis, Visibility.Visible);
        }
    }
}
