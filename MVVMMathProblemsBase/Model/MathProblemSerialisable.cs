using System.Collections.Generic;

namespace Nezmatematika.Model
{
    public class MathProblemSerialisable
    {
        public string Id { get; set; }
        public string RelDirPath { get; set; }
        public string RelFilePath { get; set; }
        public int Index { get; set; }
        public string OrderLabel { get; set; }
        public string ProblemText { get; set; }
        public string ProblemQuestion { get; set; }
        public bool CapitalisationMatters { get; set; }
        public List<string> CorrectAnswers { get; set; }
        public List<string> SolutionSteps { get; set; }
    }
}
