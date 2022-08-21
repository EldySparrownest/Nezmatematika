using Nezmatematika.Model;
using System;
using System.Windows.Input;

namespace Nezmatematika.ViewModel.Commands
{
    public class DeleteUserCommand : ICommand
    {
        public MainMenuVM MMVM { get; set; }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public DeleteUserCommand(MainMenuVM vm)
        {
            MMVM = vm;
        }

        public bool CanExecute(object parameter)
        {
            UserBase selectedUserBase = parameter as UserBase;
            if (selectedUserBase != null && MMVM.UserBasesOfTypeList.Count > 1)
            {
                return true;
            }
            return false;
        }

        public void Execute(object parameter)
        {
            UserBase userBaseToDelete = parameter as UserBase;
            MMVM.DeleteUser(userBaseToDelete);
        }
    }
}
