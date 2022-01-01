using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMMathProblemsBase.Model
{
    class MathProblemFactory
    {
        public IMathProblem Create(Course course, string problemType = "základní", string precedingLabel = "0")
        {
            IMathProblem mathProblem;
            switch (problemType)
            {
                default:
                    mathProblem = new MathProblem();
                    ((MathProblem)mathProblem).SolutionSteps = new ObservableCollection<SolutionStep>();
                    break;
            }
            mathProblem.Id = NewMathProblemId();
            mathProblem.DirPath = course.DirPath;
            mathProblem.FilePath = System.IO.Path.Combine(course.DirPath, $"{mathProblem.Id}.rtf");
            mathProblem.OrderLabel = GetNextOrderLabel(precedingLabel);
            return mathProblem;
        }

        public MathProblem CreateFromSerialised(MathProblemSerialisable serialisedMathProblem)
        {
            var mathProblem = new MathProblem();
            mathProblem.Id = serialisedMathProblem.Id;
            mathProblem.DirPath = serialisedMathProblem.DirPath;
            mathProblem.FilePath = serialisedMathProblem.FilePath;

            mathProblem.OrderLabel = serialisedMathProblem.OrderLabel;
            mathProblem.ProblemText = serialisedMathProblem.ProblemText;
            mathProblem.ProblemQuestion = serialisedMathProblem.ProblemQuestion;
            mathProblem.CorrectAnswers = serialisedMathProblem.CorrectAnswers;
            mathProblem.SolutionSteps = new ObservableCollection<SolutionStep>();

            foreach (var step in serialisedMathProblem.SolutionSteps)
            {
                mathProblem.SolutionSteps.Add(new SolutionStep(step));
            }
            return mathProblem;
        }

        public MathProblemSerialisable CreateFromMathProblem(MathProblem mathProblem)
        {
            var serialisedMathProblem = new MathProblemSerialisable();
            serialisedMathProblem.Id = mathProblem.Id;
            serialisedMathProblem.DirPath = mathProblem.DirPath;
            serialisedMathProblem.FilePath = mathProblem.FilePath;

            serialisedMathProblem.OrderLabel = mathProblem.OrderLabel;
            serialisedMathProblem.ProblemText = mathProblem.ProblemText;
            serialisedMathProblem.ProblemQuestion = mathProblem.ProblemQuestion;
            serialisedMathProblem.CorrectAnswers = mathProblem.CorrectAnswers;
            serialisedMathProblem.SolutionSteps = new List<SolutionStepSerialisable>();

            foreach (var step in mathProblem.SolutionSteps)
            {
                serialisedMathProblem.SolutionSteps.Add(new SolutionStepSerialisable(step));
            }
            return serialisedMathProblem;
        }

        private static string NewMathProblemId()
            => string.Join("", (Convert.ToString(DateTime.Now.ToString("yyyyMMddHHmmssffffff"))).Split(" .:".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));

        private static string GetNextOrderLabel(string precedingLabel)
        {
            if (precedingLabel != "0")
            {
                int dotIndex = precedingLabel.LastIndexOf('.');
                string[] lastLabelArray = precedingLabel.Split(".".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                int newLabelNbr;
                char charToIncrement;
                if (lastLabelArray.Length == 1)
                {
                    if (Int32.TryParse(lastLabelArray[0], out newLabelNbr))
                    {
                        newLabelNbr++;
                        return String.Concat(Convert.ToString(newLabelNbr), ".");
                    }
                    else
                    {
                        charToIncrement = lastLabelArray[0][lastLabelArray[0].Length - 1];
                        charToIncrement++;
                        return String.Concat(charToIncrement, ".");
                    }
                }
                else
                {
                    if (Int32.TryParse(lastLabelArray[lastLabelArray.Length - 1], out newLabelNbr))
                    {
                        newLabelNbr++;
                        return String.Concat(precedingLabel.Substring(0, dotIndex), ".", Convert.ToString(newLabelNbr));
                    }
                    else if (lastLabelArray[lastLabelArray.Length - 1].Length == 1)
                    {
                        charToIncrement = lastLabelArray[lastLabelArray.Length - 1][0];
                        charToIncrement++;
                        return String.Concat(precedingLabel.Substring(0, dotIndex + 1), charToIncrement);
                    }
                }
            }
            return "1.";
        }
    }
}
