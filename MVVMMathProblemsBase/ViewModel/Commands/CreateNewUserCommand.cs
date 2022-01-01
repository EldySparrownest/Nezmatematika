using MVVMMathProblemsBase.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input; // for ICommand

namespace MVVMMathProblemsBase.ViewModel.Commands
{
    public class CreateNewUserCommand : ICommand
    {
        public MainMenuVM MMVM { get; set; }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public CreateNewUserCommand(MainMenuVM vm)
        {
            MMVM = vm;
        }

        public bool CanExecute(object parameter)
        {
            if (string.IsNullOrEmpty(MMVM.TempFirstName))
                return false;
            if (string.IsNullOrEmpty(MMVM.TempLastName))
                return false;
            if (string.IsNullOrEmpty(MMVM.TempSchoolName))
                return false;
            if (string.IsNullOrEmpty(MMVM.TempClassName))
                return false;
            return true;
        }

        public void Execute(object parameter)
        {
            MMVM.CreateNewUser(MMVM.TempFirstName, MMVM.TempLastName, MMVM.TempSchoolName, MMVM.TempClassName);
            MMVM.BackToMainMenu();
            MMVM.GetUsersOfTypeList();
        }
    }
}

