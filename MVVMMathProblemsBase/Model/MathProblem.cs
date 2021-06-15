using System;
using System.Collections.Generic;
using System.Collections.ObjectModel; // for ObservableCollection
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows; // for Visibility


namespace MVVMMathProblemsBase.Model
{
    public class SolutionStep
    {
        public string StepText { get; set; }
        public Visibility StepVisibility { get; set; }

        public SolutionStep(string text)
        {
            StepText = text;
            StepVisibility = Visibility.Hidden;
        }
        public SolutionStep(string text, Visibility stepVis)
        {
            StepText = text;
            StepVisibility = stepVis;
        }
    }
    public class MathProblem
    {
        public string ProblemText { get; set; }
        public string ProblemQuestion { get; set; }
        public List<string> CorrectAnswers { get; set; }
        public ObservableCollection<SolutionStep> SolutionSteps { get; set; }

        public MathProblem(string text, string question, List<string> answers, List<SolutionStep> steps)
        {
            ProblemText = text;
            ProblemQuestion = question;
            CorrectAnswers = answers;
            SolutionSteps = new ObservableCollection<SolutionStep>(steps);
        }

        public int FindLastVisibleStep()
        {
            int i = 0;
            for ( ; i < SolutionSteps.Count; i++)
            {
                if (SolutionSteps[i].StepVisibility != Visibility.Visible)
                {
                    break;
                }
            }
            return i;
        }

        public void SetVisibilityOfStepOnIndex(int stepindex, Visibility newVis)
        {
            if (SolutionSteps.Count > stepindex)
            {
                SolutionSteps[stepindex].StepVisibility = newVis;
            }
        }
    }
}
