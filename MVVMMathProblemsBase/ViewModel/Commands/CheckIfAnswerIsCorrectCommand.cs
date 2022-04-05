using System;
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
            string answer = (parameter as string).Trim();
            MMVM.CurrentUserCourseData.RecordStudentAnswer(answer);
            MMVM.CurrentProblemSolved = true;

            var isCorrect = MMVM.CheckIfAnswerIsCorrect(answer);
            MMVM.UpdateUCDAndStatsAfterAnswer(isCorrect);

            MMVM.DisplayAnswerFeedback(isCorrect);
            if (MMVM.IsThisProblemTheLastOne())
                MMVM.BtnNextToBtnFinish();
        }
    }
}
