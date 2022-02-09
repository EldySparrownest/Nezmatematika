using Nezmatematika.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            //if (MMVM.CurrentUser != null)
            User user = parameter as User;
            if (user != null)
            {
                return true;
            }
            return false;
            
        }

        public void Execute(object parameter)
        {
            User user = parameter as User;
            MMVM.BackToMainMenu();
            MMVM.UserSelVis = Visibility.Visible;
            MMVM.NewUserVis = Visibility.Collapsed;

            MMVM.PopulateUserTempValues(user);

            MMVM.EditUserVis = Visibility.Visible;
        }
    }
}
