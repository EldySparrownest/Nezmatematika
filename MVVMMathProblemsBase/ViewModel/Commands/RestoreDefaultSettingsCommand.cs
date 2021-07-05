using MVVMMathProblemsBase.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MVVMMathProblemsBase.ViewModel.Commands
{
    public class RestoreDefaultSettingsCommand : ICommand
    {
        public MainMenuVM MMVM { get; set; }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RestoreDefaultSettingsCommand(MainMenuVM vm)
        {
            MMVM = vm;
        }

        public bool CanExecute(object parameter)
        {
            User user = parameter as User;
            if (user != null)
            {
                return true;
            }
            return false;

        }

        public void Execute(object parameter)
        {
            MMVM.RestoreDefaultSettingsForCurrentUser();
        }
    }
}
