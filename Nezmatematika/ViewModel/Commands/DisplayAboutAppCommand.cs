using System;
using System.Windows;
using System.Windows.Input;

namespace Nezmatematika.ViewModel.Commands
{
    public class DisplayAboutAppCommand : ICommand
    {
        public MainMenuVM MMVM { get; set; }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public DisplayAboutAppCommand(MainMenuVM vm)
        {
            MMVM = vm;
        }

        public bool CanExecute(object parameter)
        {
            if (MMVM.CurrentUser != null)
            {
                return true;
            }
            return false;
        }

        public void Execute(object parameter)
        {
            MMVM.BackToMainMenu();
            MMVM.AboutAppVis = Visibility.Visible;
        }
    }
}
