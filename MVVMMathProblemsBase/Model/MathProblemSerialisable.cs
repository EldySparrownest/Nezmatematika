using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMMathProblemsBase.Model
{
    public class MathProblemSerialisable
    {
        public string Id { get; set; }
        public string FilePath { get; set; }
        public string OrderLabel { get; set; }
        public string ProblemText { get; set; }
        public string ProblemQuestion { get; set; }
        public List<string> CorrectAnswers { get; set; }
        public List<SolutionStepSerialisable> SolutionSteps { get; set; }

        public MathProblemSerialisable()
        {
            CorrectAnswers = new List<string>();
            SolutionSteps = new List<SolutionStepSerialisable>();
        }

        public MathProblemSerialisable(MathProblem mathProblem)
        {
            Id = mathProblem.Id;
            FilePath = mathProblem.FilePath;
            
            OrderLabel = mathProblem.OrderLabel;
            ProblemText = mathProblem.ProblemText;
            ProblemQuestion = mathProblem.ProblemQuestion;
            CorrectAnswers = mathProblem.CorrectAnswers;
            SolutionSteps = new List<SolutionStepSerialisable>();

            foreach (SolutionStep step in mathProblem.SolutionSteps)
            {
                SolutionSteps.Add(new SolutionStepSerialisable(step));
            }
        }
    }
}
