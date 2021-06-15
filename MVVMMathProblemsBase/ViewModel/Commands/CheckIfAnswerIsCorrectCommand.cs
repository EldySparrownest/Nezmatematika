using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input; // for ICommand

namespace MVVMMathProblemsBase.ViewModel.Commands
{
    public class CheckIfAnswerIsCorrectCommand : ICommand
    {
        public MathProblemVM MPVM { get; set; }
        public string AnswerToCheck { get; set; }
        
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public CheckIfAnswerIsCorrectCommand(MathProblemVM vm)
        {
            MPVM = vm;
        }

        public bool CanExecute(object parameter)
        {
            string answer = parameter as string;
            if (string.IsNullOrEmpty(answer))
                return false;
            return true;
        }

        public void Execute(object parameter)
        {
            MPVM.IsAnswerCorrect();
        }
    }
}
