using Nezmatematika.Model;
using System;
using System.Windows.Input;

namespace Nezmatematika.ViewModel.Commands
{
    public class AddNewCorrectAnswerCommand : ICommand
    {
        public MainMenuVM MMVM { get; set; }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public AddNewCorrectAnswerCommand(MainMenuVM vm)
        {
            MMVM = vm;
        }

        public bool CanExecute(object parameter)
        {
            return App.WhereInApp == WhereInApp.CourseEditor
                && MMVM.CurrentMathProblem != null
                && !String.IsNullOrWhiteSpace(parameter?.ToString());
        }

        public void Execute(object parameter)
        {
            MMVM.CurrentMathProblem.CorrectAnswers.Add(parameter.ToString());
            MMVM.ReloadTempAnswers();
            MMVM.TempCorrectAnswer = String.Empty;
        }
    }
}
