using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nezmatematika.Model
{
    interface IMathProblem
    {
        string Id { get; set; }
        string DirPath { get; set; }
        string FilePath { get; set; }
        int Index { get; set; }
        string OrderLabel { get; set; }
        string ProblemText { get; set; }
        string ProblemQuestion { get; set; }
        ObservableCollection<string> CorrectAnswers { get; set; }
        ObservableCollection<string> SolutionSteps { get; set; }

        void SetSimplifiedOrderLabel();
    }
}
