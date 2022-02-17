using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Nezmatematika.ViewModel.Commands
{
    public class PublishCourseCommand : ICommand
    {
        public MainMenuVM MMVM { get; set; }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public PublishCourseCommand(MainMenuVM vm)
        {
            MMVM = vm;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var publish = true;
            int faultyProblemOrder = 0;
            
            for (int i = 0; i < MMVM.CurrentCourse.Problems.Count; i++)
            {
                var problem = MMVM.CurrentCourse.Problems[i];
                if (problem.CorrectAnswers.Count == 0)
                {
                    publish = false;
                    faultyProblemOrder = i + 1;
                }
            }

            if (publish)
                MMVM.CurrentCourse.Publish(MMVM._CoursesDirPath(), MMVM._CoursesArchivedDirPath());
            else
            {
                MessageBox.Show("Aby kurz mohl být zveřejněn, musí mít každá úloha alespoň 1 správnou odpověď.\n" +
                                $"Doplňte prosím správnou odpověď k {faultyProblemOrder}. úloze.");
            }
        }
    }
}
