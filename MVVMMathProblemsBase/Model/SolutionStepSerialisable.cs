using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nezmatematika.Model
{
    public class SolutionStepSerialisable
    {
        public string StepText { get; set; }

        public SolutionStepSerialisable()
        {
            
        }
        public SolutionStepSerialisable(SolutionStep step)
        {
            StepText = step.StepText;
        }
    }
}
