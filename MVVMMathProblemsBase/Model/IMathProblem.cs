using System.Collections.ObjectModel;

namespace Nezmatematika.Model
{
    interface IMathProblem
    {
        string Id { get; set; }
        string DirPath { get; set; }
        string RelFilePath { get; set; }
        int Index { get; set; }
        string OrderLabel { get; set; }
        string ProblemText { get; set; }
        string ProblemQuestion { get; set; }
        bool CapitalisationMatters { get; set; }
        ObservableCollection<string> CorrectAnswers { get; set; }
        ObservableCollection<string> SolutionSteps { get; set; }

        void SetSimplifiedOrderLabel();
    }
}
