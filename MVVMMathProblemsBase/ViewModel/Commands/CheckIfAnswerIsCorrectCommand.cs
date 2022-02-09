using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input; // for ICommand

namespace Nezmatematika.ViewModel.Commands
{
    public class CheckIfAnswerIsCorrectCommand : ICommand
    {
        public MainMenuVM MMVM { get; set; }
        
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public CheckIfAnswerIsCorrectCommand(MainMenuVM vm)
        {
            MMVM = vm;
        }

        public bool CanExecute(object parameter)
        {
            string answer = parameter as string;
            if (string.IsNullOrWhiteSpace(answer) || MMVM.CurrentProblemSolved)
                return false;
            return true;
        }

        public void Execute(object parameter)
        {
            string answer = parameter as string;
            var isCorrect = MMVM.CheckIfAnswerIsCorrect(answer);
            MMVM.RespondToAnswer(isCorrect);
        }
    }
}
