using Nezmatematika.Model;
using System;
using System.IO;
using System.Windows.Input;

namespace Nezmatematika.ViewModel.Commands
{
    public class RemoveProblemCommand : ICommand
    {
        public MainMenuVM MMVM { get; set; }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RemoveProblemCommand(MainMenuVM vm)
        {
            MMVM = vm;
        }

        public bool CanExecute(object parameter)
        {
            if (App.WhereInApp != WhereInApp.CourseEditor)
                return false;

            return MMVM.CurrentMathProblem != null && MMVM.CurrentCourse.Problems.Count > 1;
        }

        public void Execute(object parameter)
        {
            var index = MMVM.CurrentMathProblem.Index;
            File.Delete(Path.Combine(App.MyBaseDirectory, MMVM.CurrentMathProblem.RelFilePath));
            MMVM.CurrentCourse.Problems.RemoveAt(index);
            MMVM.CurrentCourse.Save();
            if (index == MMVM.CurrentCourse.Problems.Count)
                index--;
            MMVM.CurrentMathProblem = MMVM.CurrentCourse.Problems[index];
        }
    }
}
