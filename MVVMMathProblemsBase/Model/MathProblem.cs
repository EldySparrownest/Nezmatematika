using System;
using System.Collections.Generic;
using System.IO;
using System.Windows; // for Visibility
using System.Windows.Documents;

namespace Nezmatematika.Model
{
    public class MathProblem
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

        public MathProblem()
        {
            Id = NewMathProblemId();
            CorrectAnswers = new List<string>();
            SolutionSteps = new List<string>();
        }

        public MathProblem(Course course, bool capitalisationMatters)
        {
            Id = NewMathProblemId();
            RelDirPath = course.RelDirPath;
            RelFilePath = System.IO.Path.Combine(course.RelDirPath, $"{Id}.rtf");
            Index = course.Problems.Count;
            CapitalisationMatters = capitalisationMatters;
            CorrectAnswers = new List<string>();
            SolutionSteps = new List<string>();
            SetSimplifiedOrderLabel();
        }

        private static string NewMathProblemId()
            => string.Join("", Convert.ToString(DateTime.Now.ToString("yyyyMMddHHmmssffffff")).Split(" .:".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));

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
            var trimmedAndPruned = new List<string>();

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
    }
}
