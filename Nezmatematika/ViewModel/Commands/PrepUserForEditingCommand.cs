using Nezmatematika.Model;
using System;
using System.Windows;
using System.Windows.Input;

namespace Nezmatematika.ViewModel.Commands
{
    public class PrepUserForEditingCommand : ICommand
    {
        public MainMenuVM MMVM { get; set; }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public PrepUserForEditingCommand(MainMenuVM vm)
        {
            MMVM = vm;
        }

        public bool CanExecute(object parameter)
        {
            UserBase userBase = parameter as UserBase;
            if (userBase != null)
                return true;

            return false;

        }

        public void Execute(object parameter)
        {
            UserBase user = parameter as UserBase;
            MMVM.BackToMainMenu();
            MMVM.UserSelVis = Visibility.Visible;
            MMVM.NewUserVis = Visibility.Collapsed;

            MMVM.PopulateUserTempValues(user);

            MMVM.EditUserVis = Visibility.Visible;
        }
    }
}
