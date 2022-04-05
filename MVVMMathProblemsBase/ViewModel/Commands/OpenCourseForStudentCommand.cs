using Nezmatematika.Model;
using System;
using System.Windows;
using System.Windows.Input;

namespace Nezmatematika.ViewModel.Commands
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
            if (MMVM.CurrentUser == null)
                return false;
            if (!MMVM.IsInStudentMode)
                return false;

            if (MMVM.CurrentCourse != null)
                return true;

            if (parameter != null)
            {
                var ucd = parameter as UserCourseData;
                if (ucd != null)
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
                {
                    MMVM.CurrentUserCourseData = ucd;
                    MMVM.CurrentCourse = MMVM.AllPublishedCoursesList.Find(c => c.Id == ucd.CourseId && ucd.Version == c.Version);

                    if (MMVM.CurrentCourse == null)
                        MMVM.CurrentCourse = MMVM.AllArchivedCoursesList.Find(c => c.Id == ucd.CourseId && ucd.Version == c.Version);
                }
            }

            MMVM.BackToMainMenu();

            if (MMVM.CurrentCourse == null)
                MessageBox.Show("Vybraný kurz se nepodařilo načíst. Vyberte si, prosím, jiný.");
            else
            {
                MMVM.MainMenuVis = Visibility.Collapsed;
                MMVM.OpenCurrentCourseForStudent();
            }
        }
    }
}
