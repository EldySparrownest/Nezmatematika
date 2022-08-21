using System;
using System.Windows;
using System.Windows.Input;

namespace Nezmatematika.ViewModel.Commands
{
    public class SwitchBetweenUserAndCodeModeCommand : ICommand
    {
        public MainMenuVM MMVM { get; set; }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public SwitchBetweenUserAndCodeModeCommand(MainMenuVM vm)
        {
            MMVM = vm;
        }

        public bool CanExecute(object parameter)
        {
            return App.WhereInApp == WhereInApp.CourseEditor;
        }

        public void Execute(object parameter)
        {
            if (MMVM.CourseEditorMathProblemUserModeVis == Visibility.Visible)
            {
                MMVM.CourseEditorMathProblemUserModeVis = Visibility.Collapsed;
                MMVM.CourseEditorMathProblemCodeModeVis = Visibility.Visible;
            }
            else
            {

                MMVM.CourseEditorMathProblemCodeModeVis = Visibility.Collapsed;
                MMVM.CourseEditorMathProblemUserModeVis = Visibility.Visible;
            }
        }
    }
}
