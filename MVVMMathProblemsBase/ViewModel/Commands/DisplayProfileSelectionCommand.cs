using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MVVMMathProblemsBase.ViewModel.Commands
{
    public class DisplayProfileSelectionCommand : ICommand
    {
        public MainMenuVM MMVM { get; set; }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public DisplayProfileSelectionCommand(MainMenuVM vm)
        {
            MMVM = vm;
        }

        public bool CanExecute(object parameter)
        {
            if (MMVM.CurrentUser != null && MMVM.UserSelVis != Visibility.Visible)
            {
                return true;
            }
            return false;
        }

        public void Execute(object parameter)
        {
            MMVM.EditUserVis = Visibility.Collapsed;
            MMVM.SettingsVis = Visibility.Collapsed;

            MMVM.NewUserVis = Visibility.Visible;
            MMVM.UserSelVis = Visibility.Visible;
        }
    }
}
