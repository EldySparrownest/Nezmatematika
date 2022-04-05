using System;
using System.Collections.ObjectModel; // for ObservableCollection
using System.ComponentModel;
using System.IO;
using System.Windows; // for Visibility
using System.Windows.Documents;

namespace Nezmatematika.Model
{
    public class MathProblem : IMathProblem, INotifyPropertyChanged
    {
        public string Id { get; set; }
        public string DirPath { get; set; }
        public string RelFilePath { get; set; }
        public int Index { get; set; }
        public string OrderLabel { get; set; }
        public string ProblemText { get; set; }
        public string ProblemQuestion { get; set; }
        public bool CapitalisationMatters { get; set; }

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
        private ObservableCollection<string> solutionSteps { get; set; }
        public ObservableCollection<string> SolutionSteps
        {
            get { return solutionSteps; }
            set
            {
                solutionSteps = value;
                OnPropertyChanged("SolutionSteps");
            }
        }

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
        public void SetSimplifiedOrderLabel() => OrderLabel = $"{Index + 1}.";

        public void TrimAndPruneCorrectAnswers()
        {
            var trimmedAndPruned = new ObservableCollection<string>();

            for (int i = 0; i < CorrectAnswers.Count; i++)
            {
                var answer = CorrectAnswers[i].Trim();
                if (!String.IsNullOrEmpty(answer) && !trimmedAndPruned.Contains(answer))
                    trimmedAndPruned.Add(answer);
            }

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

        public bool CheckAnswerIsCorrect(string answerToCheck)
        {
            if (CapitalisationMatters)
                return CorrectAnswers.Contains(answerToCheck.Trim());

            foreach (string correctAnswer in CorrectAnswers)
            {
                if (answerToCheck.ToLower().Equals(correctAnswer.Trim().ToLower()))
                    return true;
            }
            return false;
        }

        public string GetCorrectAnswersInOneString() => string.Join("   ", CorrectAnswers);

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
