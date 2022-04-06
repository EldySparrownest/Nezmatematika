using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Nezmatematika.Model
{
    class MathProblemFactory
    {
        public MathProblem Create(Course course, bool capitalisationMatters)
        {
            var id = NewMathProblemId();
            var mathProblem = new MathProblem
            {
                Id = id,
                RelDirPath = course.RelDirPath,
                RelFilePath = System.IO.Path.Combine(course.RelDirPath, $"{id}.rtf"),
                Index = course.Problems.Count,
                CapitalisationMatters = capitalisationMatters,
                CorrectAnswers = new ObservableCollection<string>(),
                SolutionSteps = new ObservableCollection<string>()
            };
            mathProblem.SetSimplifiedOrderLabel();
            return mathProblem;
        }

        public MathProblem CreateFromSerialised(MathProblemSerialisable serialisedMathProblem)
        {
            var mathProblem = new MathProblem();
            mathProblem.Id = serialisedMathProblem.Id;
            mathProblem.RelDirPath = serialisedMathProblem.RelDirPath;
            mathProblem.RelFilePath = serialisedMathProblem.RelFilePath;

            mathProblem.Index = serialisedMathProblem.Index;
            mathProblem.OrderLabel = serialisedMathProblem.OrderLabel;
            mathProblem.ProblemText = serialisedMathProblem.ProblemText;
            mathProblem.ProblemQuestion = serialisedMathProblem.ProblemQuestion;
            mathProblem.CapitalisationMatters = serialisedMathProblem.CapitalisationMatters;
            mathProblem.CorrectAnswers = new ObservableCollection<string>(serialisedMathProblem.CorrectAnswers);
            mathProblem.SolutionSteps = new ObservableCollection<string>(serialisedMathProblem.SolutionSteps);
            return mathProblem;
        }

        public MathProblemSerialisable CreateFromMathProblem(MathProblem mathProblem)
        {
            var serialisedMathProblem = new MathProblemSerialisable();
            serialisedMathProblem.Id = mathProblem.Id;
            serialisedMathProblem.RelDirPath = mathProblem.RelDirPath;
            serialisedMathProblem.RelFilePath = mathProblem.RelFilePath;

            serialisedMathProblem.Index = mathProblem.Index;
            serialisedMathProblem.OrderLabel = mathProblem.OrderLabel;
            serialisedMathProblem.ProblemText = mathProblem.ProblemText;
            serialisedMathProblem.ProblemQuestion = mathProblem.ProblemQuestion;
            serialisedMathProblem.CapitalisationMatters = mathProblem.CapitalisationMatters;
            serialisedMathProblem.CorrectAnswers = mathProblem.CorrectAnswers.ToList();
            serialisedMathProblem.SolutionSteps = mathProblem.SolutionSteps.ToList();
            return serialisedMathProblem;
        }

        private static string NewMathProblemId()
            => string.Join("", Convert.ToString(DateTime.Now.ToString("yyyyMMddHHmmssffffff")).Split(" .:".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
    }
}
