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

        public SolutionStep()
        {
            StepVisibility = Visibility.Hidden;
        }
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
        public string OrderLabel { get; set; }
        public string ProblemText { get; set; }
        public string ProblemQuestion { get; set; }
        public List<string> CorrectAnswers { get; set; }
        public ObservableCollection<SolutionStep> SolutionSteps { get; set; }

        public MathProblem()
        {
            ProblemText = "Zde bude zadání.";
            ProblemQuestion = "Zde bude otázka.";
        }
        public MathProblem(string text, string question, List<string> answers, List<SolutionStep> steps)
        {
            ProblemText = text;
            ProblemQuestion = question;
            CorrectAnswers = answers;
            SolutionSteps = new ObservableCollection<SolutionStep>(steps);
        }

        public static string GetNextOrderLabel(string precedingLabel)
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

        public int FindLastVisibleStep()
        {
            int i = 0;
            for (; i < SolutionSteps.Count; i++)
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
