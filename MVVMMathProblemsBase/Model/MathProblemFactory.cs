using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nezmatematika.Model
{
    class MathProblemFactory
    {
        public IMathProblem Create(Course course, string problemType = "základní")
        {
            IMathProblem mathProblem;
            switch (problemType)
            {
                default:
                    mathProblem = new MathProblem();
                    ((MathProblem)mathProblem).SolutionSteps = new ObservableCollection<string>();
                    ((MathProblem)mathProblem).CorrectAnswers = new ObservableCollection<string>();
                    break;
            }
            mathProblem.Id = NewMathProblemId();
            mathProblem.DirPath = course.DirPath;
            mathProblem.FilePath = System.IO.Path.Combine(course.DirPath, $"{mathProblem.Id}.rtf");
            mathProblem.Index = course.Problems.Count;
            mathProblem.SetSimplifiedOrderLabel();
            return mathProblem;
        }

        public MathProblem CreateFromSerialised(MathProblemSerialisable serialisedMathProblem)
        {
            var mathProblem = new MathProblem();
            mathProblem.Id = serialisedMathProblem.Id;
            mathProblem.DirPath = serialisedMathProblem.DirPath;
            mathProblem.FilePath = serialisedMathProblem.FilePath;

            mathProblem.Index = serialisedMathProblem.Index;
            mathProblem.OrderLabel = serialisedMathProblem.OrderLabel;
            mathProblem.ProblemText = serialisedMathProblem.ProblemText;
            mathProblem.ProblemQuestion = serialisedMathProblem.ProblemQuestion;
            mathProblem.CorrectAnswers = new ObservableCollection<string>(serialisedMathProblem.CorrectAnswers);
            mathProblem.SolutionSteps = new ObservableCollection<string>(serialisedMathProblem.SolutionSteps);
            return mathProblem;
        }

        public MathProblemSerialisable CreateFromMathProblem(MathProblem mathProblem)
        {
            var serialisedMathProblem = new MathProblemSerialisable();
            serialisedMathProblem.Id = mathProblem.Id;
            serialisedMathProblem.DirPath = mathProblem.DirPath;
            serialisedMathProblem.FilePath = mathProblem.FilePath;

            serialisedMathProblem.Index = mathProblem.Index;
            serialisedMathProblem.OrderLabel = mathProblem.OrderLabel;
            serialisedMathProblem.ProblemText = mathProblem.ProblemText;
            serialisedMathProblem.ProblemQuestion = mathProblem.ProblemQuestion;
            serialisedMathProblem.CorrectAnswers = mathProblem.CorrectAnswers.ToList();
            serialisedMathProblem.SolutionSteps = mathProblem.SolutionSteps.ToList();
            return serialisedMathProblem;
        }

        private static string NewMathProblemId()
            => string.Join("", Convert.ToString(DateTime.Now.ToString("yyyyMMddHHmmssffffff")).Split(" .:".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
    }
}
