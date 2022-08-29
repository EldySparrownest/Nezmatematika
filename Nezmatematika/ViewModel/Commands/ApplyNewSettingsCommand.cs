using System;
using System.Windows.Input;

namespace Nezmatematika.ViewModel.Commands
{
    public class ApplyNewSettingsCommand : ICommand
    {
        public MainMenuVM MMVM { get; set; }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public ApplyNewSettingsCommand(MainMenuVM vm)
        {
            MMVM = vm;
        }

        public bool CanExecute(object parameter)
        {
            return App.WhereInApp == WhereInApp.Settings && MMVM.CurrentUser != null;
        }

        public void Execute(object parameter)
        {
            MMVM.SaveCurrentSettingsForCurrentUser();
        }
    }
}
