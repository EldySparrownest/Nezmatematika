using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MVVMMathProblemsBase.ViewModel.Commands
{
    public class CreateNewCourseCommand : ICommand
    {
        public MainMenuVM MMVM { get; set; }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public CreateNewCourseCommand(MainMenuVM vm)
        {
            MMVM = vm;
        }

        public bool CanExecute(object parameter)
        {
            return (string.IsNullOrEmpty(MMVM.TempCourseTitle) == false);
        }

        public void Execute(object parameter)
        {
            MMVM.CreateNewCourse();
            MMVM.TeacherVis = Visibility.Collapsed;
            MMVM.NewCourseVis = Visibility.Collapsed;
            MMVM.EditCourseVis = Visibility.Visible;
            MMVM.CourseEditorMathProblemUserModeVis = Visibility.Visible;
            App.WhereInApp = WhereInApp.CourseEditor;
        }
    }
}
