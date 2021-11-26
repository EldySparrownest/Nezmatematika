using MVVMMathProblemsBase.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MVVMMathProblemsBase.ViewModel.Commands
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
            if (Directory.Exists(courseToDelete.DirPath))
            {
                var mathProblemFilePaths = Directory.GetFiles(courseToDelete.DirPath);
                foreach (var path in mathProblemFilePaths)
                {
                    File.Delete(path);
                }
                Directory.Delete(courseToDelete.DirPath);
            }
            if (File.Exists(courseToDelete.FilePath))
            {
                File.Delete(courseToDelete.FilePath);
            }
            MMVM.GetListOfTeacherCoursesToContinue();
            if (MMVM.CoursesToContinueList.Count == 0)
            {
                MMVM.UpdateAbilityToContinueCourse();
                MMVM.BackToMainMenu();
            }
        }
    }
}
