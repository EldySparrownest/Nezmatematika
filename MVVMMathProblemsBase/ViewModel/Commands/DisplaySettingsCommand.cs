using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MVVMMathProblemsBase.ViewModel.Commands
{
    public class DisplaySettingsCommand : ICommand
    {
        public MainMenuVM MMVM { get; set; }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public DisplaySettingsCommand(MainMenuVM vm)
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
            MMVM.EditUserVis = Visibility.Collapsed;
            MMVM.NewUserVis = Visibility.Collapsed;
            MMVM.UserSelVis = Visibility.Collapsed;
            
            MMVM.SettingsVis = Visibility.Visible;
        }
    }
}
