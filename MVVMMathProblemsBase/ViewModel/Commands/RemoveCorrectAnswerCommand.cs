using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Nezmatematika.ViewModel.Commands
{
    public class RemoveCorrectAnswerCommand : ICommand
    {
        public MainMenuVM MMVM { get; set; }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RemoveCorrectAnswerCommand(MainMenuVM vm)
        {
            MMVM = vm;
        }

        public bool CanExecute(object parameter)
        {
            return MMVM.CurrentAnswer != null && MMVM.CurrentAnswer == parameter?.ToString();
        }

        public void Execute(object parameter)
        {
            MMVM.CurrentMathProblem.CorrectAnswers.Remove(parameter.ToString());
            MMVM.ReloadTempAnswers();
        }
    }
}
