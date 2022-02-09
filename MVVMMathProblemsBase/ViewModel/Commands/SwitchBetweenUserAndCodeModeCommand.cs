using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            return MMVM.EditCourseVis == Visibility.Visible;
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
