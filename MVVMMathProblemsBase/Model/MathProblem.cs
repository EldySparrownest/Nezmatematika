using System;
using System.Collections.Generic;
using System.Collections.ObjectModel; // for ObservableCollection
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows; // for Visibility
using System.Windows.Documents;

namespace MVVMMathProblemsBase.Model
{
    public class MathProblem : IMathProblem
    {
        public string Id { get; set; }
        public string DirPath { get; set; }
        public string FilePath { get; set; }
        public string OrderLabel { get; set; }
        public string ProblemText { get; set; }
        public string ProblemQuestion { get; set; }
        public List<string> CorrectAnswers { get; set; }
        public ObservableCollection<SolutionStep> SolutionSteps { get; set; }

        public void SaveContents(TextRange contents, string filePath, string dirPath)
        {
            try
            {
                Directory.CreateDirectory(dirPath);
                FileStream fileStream = new FileStream(filePath, FileMode.Create);
                contents.Save(fileStream, DataFormats.Rtf);
                fileStream.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
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
