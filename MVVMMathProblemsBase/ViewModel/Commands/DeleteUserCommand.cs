using Nezmatematika.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            User selectedUser = parameter as User;
            if (selectedUser != null && MMVM.UsersOfTypeList.Count > 1)
            {
                return true;
            }
            return false;
        }

        public void Execute(object parameter)
        {
            User userToDelete = parameter as User;
            MMVM.DeleteUser(userToDelete);
        }
    }
}
