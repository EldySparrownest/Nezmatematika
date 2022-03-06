using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Nezmatematika.ViewModel.Commands
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
            if (App.WhereInApp != WhereInApp.CourseForStudent)
                return false;
            return MMVM.CurrentMathProblem != null && MMVM.CurrentProblemSolved && !MMVM.IsThisProblemTheLastOne();
        }

        public void Execute(object parameter)
        {
            MMVM.ResetAnswerFeedbackVisibility();
            MMVM.CurrentMathProblemIndex++;
            MMVM.SetCurrentMathProblemFromCurrentIndex();
            MMVM.SaveDataAndStats();
        }
    }
}
