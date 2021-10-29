using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
        public SolutionStep(SolutionStepSerialisable serialisedStep)
        {
            StepText = serialisedStep.StepText;
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
}
