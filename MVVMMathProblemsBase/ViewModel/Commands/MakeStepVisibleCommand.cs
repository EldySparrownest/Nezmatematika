using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input; // for ICommand
using System.Windows; // for Visibility

namespace Nezmatematika.ViewModel.Commands
{
    public class MakeStepVisibleCommand : ICommand
    {
        public MainMenuVM MMVM { get; set; }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        public MakeStepVisibleCommand(MainMenuVM vm)
        {
            MMVM = vm;
        }

        public bool CanExecute(object parameter)
        {
            if (App.WhereInApp != WhereInApp.CourseForStudent)
                return false;

            if (MMVM.CurrentMathProblem == null || MMVM.CurrentUserCourseData == null
                || MMVM.CurrentUserCourseData.VisibleStepsCounts == null
                || MMVM.SolutionStepsShownToStudent == null)
                return false;
            
            if (!(MMVM.CurrentUserCourseData.VisibleStepsCounts.Count > MMVM.CurrentMathProblemIndex))
                return false;

            return MMVM.SolutionStepsShownToStudent.Count < MMVM.CurrentMathProblem.SolutionSteps.Count;
        }

        public void Execute(object parameter)
        {
            MMVM.CurrentUserCourseData.RecordStepReveal(MMVM.CurrentMathProblemIndex);
            MMVM.SaveUCD();
            MMVM.ReloadShownSolutionSteps();
        }
    }
}
