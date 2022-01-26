using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MVVMMathProblemsBase.ViewModel.Commands
{
    public class SwitchToNextProblemCommand : ICommand
    {
        public MainMenuVM MMVM { get; set; }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public SwitchToNextProblemCommand(MainMenuVM vm)
        {
            MMVM = vm;
        }

        public bool CanExecute(object parameter)
        {
            return MMVM.CurrentMathProblem != null && MMVM.CurrentMathProblem.Index < MMVM.CurrentCourse.Problems.Count - 1;
        }

        public void Execute(object parameter)
        {
            var newIndex = MMVM.CurrentMathProblem.Index + 1;
            MMVM.CurrentMathProblem = MMVM.CurrentCourse.Problems[newIndex];
        }
    }
}
