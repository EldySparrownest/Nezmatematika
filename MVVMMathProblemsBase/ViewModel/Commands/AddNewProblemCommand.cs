using System;
using System.Windows.Input;

namespace Nezmatematika.ViewModel.Commands
{
    public class AddNewProblemCommand : ICommand
    {
        public MainMenuVM MMVM { get; set; }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public AddNewProblemCommand(MainMenuVM vm)
        {
            MMVM = vm;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            MMVM.CurrentCourse.AddNewMathProblem(MMVM.Settings.CapitalisationMatters);
        }
    }
}
