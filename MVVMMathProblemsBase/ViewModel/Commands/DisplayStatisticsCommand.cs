using Nezmatematika.Model;
using System;
using System.Windows;
using System.Windows.Input;

namespace Nezmatematika.ViewModel.Commands
{
    public class DisplayStatisticsCommand : ICommand
    {
        public MainMenuVM MMVM { get; set; }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public DisplayStatisticsCommand(MainMenuVM vm)
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
            MMVM.ReloadCurrentUserStats();
            App.WhereInApp = WhereInApp.Statistics;

            if (MMVM.IsInStudentMode == true)
                MMVM.StudentStatsVis = Visibility.Visible;
            else
                MMVM.TeacherStatsVis = Visibility.Visible;

            MMVM.StatsVis = Visibility.Visible;
        }
    }
}
