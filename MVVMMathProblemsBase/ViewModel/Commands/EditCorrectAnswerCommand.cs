using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MVVMMathProblemsBase.ViewModel.Commands
{
    public class EditCorrectAnswerCommand : ICommand
    {
        public MainMenuVM MMVM { get; set; }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public EditCorrectAnswerCommand(MainMenuVM vm)
        {
            MMVM = vm;
        }

        public bool CanExecute(object parameter)
        {
            return MMVM.CurrentAnswer != null && !String.IsNullOrWhiteSpace(MMVM.TempCorrectAnswer) && MMVM.CurrentAnswer != MMVM.TempCorrectAnswer;
        }

        public void Execute(object parameter)
        {
            MMVM.ReplaceInTempAnswers(MMVM.CurrentAnswer, MMVM.TempCorrectAnswer);
            MMVM.ReloadCorrectAnswers();
            MMVM.CurrentAnswer = MMVM.TempCorrectAnswer;
        }
    }
}
