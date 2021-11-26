using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMMathProblemsBase.Model
{
    interface IMathProblem
    {
        string Id { get; set; }
        string FilePath { get; set; }
        string OrderLabel { get; set; }
        string ProblemText { get; set; }
        string ProblemQuestion { get; set; }
        List<string> CorrectAnswers { get; set; }
    }
}
