using System;
using System.Collections.Generic;
using System.Collections.ObjectModel; // for ObservableCollection
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows; // for Visibility
using System.Windows.Documents;

namespace MVVMMathProblemsBase.Model
{
    public class MathProblem : IMathProblem, INotifyPropertyChanged
    {
        public string Id { get; set; }
        public string DirPath { get; set; }
        public string FilePath { get; set; }
        public int Index { get; set; }
        public string OrderLabel { get; set; }
        public string ProblemText { get; set; }
        public string ProblemQuestion { get; set; }

        private ObservableCollection<string> correctAnswers { get; set; }
        public ObservableCollection<string> CorrectAnswers
        {
            get { return correctAnswers; }
            set
            { 
                correctAnswers = value;
                OnPropertyChanged("CorrectAnswers");
            }
        }
        public ObservableCollection<SolutionStep> SolutionSteps { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

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

        public void TrimAndPruneCorrectAnswers()
        {
            var trimmedAndPruned = new ObservableCollection<string>();
            
            for (int i =  0; i < CorrectAnswers.Count; i++)
            {
                var answer = CorrectAnswers[i].Trim();
                if (!String.IsNullOrEmpty(answer) && !trimmedAndPruned.Contains(answer))
                    trimmedAndPruned.Add(answer);
            }

            if (trimmedAndPruned.Count == 0)
                trimmedAndPruned.Add("");

            CorrectAnswers.Clear();
            CorrectAnswers = trimmedAndPruned;
        }

        public bool ValidateMathProblem()
        {
            var pocetSpravnychOdpovedi = CorrectAnswers.Count;
            
            if (pocetSpravnychOdpovedi == 0)
                return false;
            
            if (pocetSpravnychOdpovedi == 1 && String.IsNullOrEmpty(CorrectAnswers[0].Trim()))
                return false;
            
            return true;
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

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
