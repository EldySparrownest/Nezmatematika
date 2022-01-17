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
        public string DirPath { get; set; }
        public string FilePath { get; set; }
        public int Index { get; set; }
        public string OrderLabel { get; set; }
        public string ProblemText { get; set; }
        public string ProblemQuestion { get; set; }
        public List<string> CorrectAnswers { get; set; }
        public List<SolutionStepSerialisable> SolutionSteps { get; set; }

    }
}
