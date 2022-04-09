using Nezmatematika.Model;
using System;
using System.Windows.Input;

namespace Nezmatematika.ViewModel.Commands
{
    public class DeleteCourseCommand : ICommand
    {
        public MainMenuVM MMVM { get; set; }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public DeleteCourseCommand(MainMenuVM vm)
        {
            MMVM = vm;
        }

        public bool CanExecute(object parameter)
        {
            if (MMVM.IsInStudentMode == true)
                return false;
            Course selectedCourse = parameter as Course;
            if (selectedCourse != null)
            {
                return true;
            }
            return false;
        }

        public void Execute(object parameter)
        {
            Course courseToDelete = parameter as Course;
            if (courseToDelete.Version != 0)
            {
                Course.ArchiveCourse(courseToDelete.Id, courseToDelete.Version);
            }

            courseToDelete.Delete();

            MMVM.GetListOfTeacherCoursesToContinue();
            if (MMVM.TeacherCoursesToContinueList.Count == 0)
            {
                MMVM.UpdateAbilityToContinueCourse();
                MMVM.BackToMainMenu();
            }
        }
    }
}
