using MVVMMathProblemsBase.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MVVMMathProblemsBase.ViewModel.Commands
{
    public class OpenCourseForStudentCommand : ICommand
    {
        public MainMenuVM MMVM { get; set; }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public OpenCourseForStudentCommand(MainMenuVM vm)
        {
            MMVM = vm;
        }

        public bool CanExecute(object parameter)
        {
            if (MMVM.CurrentUser != null && MMVM.IsInStudentMode == true && (MMVM.CurrentCourse != null || MMVM.CurrentUserCourseData != null))
            {
                return true;
            }
            return false;
        }

        public void Execute(object parameter = null)
        {
            if (parameter != null)
            {
                var ucd = parameter as UserCourseData;
                if (ucd != null)
                    MMVM.CurrentCourse = MMVM.StudentDirCourseList.Find(c => c.Id == ucd.CourseId);
            }
            
            MMVM.BackToMainMenu();
            MMVM.StudentVis = Visibility.Collapsed;
            MMVM.OpenCurrentCourseForStudent();
        }
    }
}
