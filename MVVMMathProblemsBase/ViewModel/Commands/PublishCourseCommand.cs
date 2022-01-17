using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MVVMMathProblemsBase.ViewModel.Commands
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
            foreach (var problem in MMVM.CurrentCourse.Problems)
            {
                if (problem.CorrectAnswers.Count == 0)
                    publish = false;
            }
            if (publish)
                MMVM.CurrentCourse.Publish(MMVM._CoursesDirPath());
        }
    }
}
