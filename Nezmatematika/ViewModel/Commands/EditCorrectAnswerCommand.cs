using System;
using System.Windows.Input;

namespace Nezmatematika.ViewModel.Commands
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
            if (App.WhereInApp != WhereInApp.CourseEditor)
                return false;
            return MMVM.CurrentAnswer != null && !String.IsNullOrWhiteSpace(MMVM.TempCorrectAnswer) && MMVM.CurrentAnswer != MMVM.TempCorrectAnswer;
        }

        public void Execute(object parameter)
        {
            MMVM.ReplaceInCurrentProblemCorrectAnswers(MMVM.CurrentAnswer, MMVM.TempCorrectAnswer);
            MMVM.PopulateTempAnswersFromCurrentMathProblem();
            MMVM.CurrentAnswer = MMVM.TempCorrectAnswer;
        }
    }
}
