using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Nezmatematika.Model
{
    [Serializable]
    public class UserCourseData
    {
        public string CourseId { get; set; }
        public int Version { get; set; }
        public string CourseTitle { get; set; }
        public string CourseAuthor { get; set; }
        public int CourseProblemCount { get; set; }
        public string UserId { get; set; }
        public int Mistakes { get; set; }
        public bool Completed { get; set; }
        public DateTime CourseStarted { get; set; }
        public DateTime CourseFinished { get; set; }
        public DateTime LastSessionStarted { get; set; }
        public DateTime LastSessionEnded { get; set; }
        public TimeSpan NetCourseTime { get; set; }
        public int ResumeOnIndex { get; set; }
        public int SolvedProblemsCount { get; set; }
        public int SolvedCorrectlyCount { get; set; }
        public List<int> RequeuedProblems { get; set; }
        public List<int> VisibleStepsCounts { get; set; }
        public List<string> StudentAnswers { get; set; }

        public UserCourseData()
        {
            RequeuedProblems = new List<int>();
            VisibleStepsCounts = new List<int>();
            StudentAnswers = new List<string>();
        }

        public UserCourseData(Course course, string userId, DateTime startTime)
        {
            CourseId = course.Id;
            Version = course.Version;
            CourseTitle = course.CourseTitle;
            CourseAuthor = course.Author.DisplayName;
            CourseProblemCount = course.Problems.Count;
            UserId = userId;
            Mistakes = 0;
            Completed = false;
            CourseStarted = startTime;
            ResumeOnIndex = 0;
            SolvedProblemsCount = 0;
            SolvedCorrectlyCount = 0;
            RequeuedProblems = new List<int>();
            VisibleStepsCounts = new List<int>();
            StudentAnswers = new List<string>();
        }

        public void RecordStudentAnswer(string answer)
        {
            if (ResumeOnIndex <= StudentAnswers.Count)
                StudentAnswers.Add(answer);
        }

        public void UpdateAtSessionStart()
        {
            LastSessionStarted = DateTime.Now;
        }

        public void UpdateAfterCorrectAnswer(out bool completed)
        {
            SolvedProblemsCount++;
            SolvedCorrectlyCount++;
            ResumeOnIndex++;

            completed = (SolvedProblemsCount == CourseProblemCount + RequeuedProblems.Count);
        }

        public void UpdateAfterIncorrectAnswer(int problemIndex, bool requeue)
        {
            SolvedProblemsCount++;
            ResumeOnIndex++;
            Mistakes++;
            if (requeue)
                RequeuedProblems.Add(problemIndex);
        }

        public void UpdateAtSessionEnd(out TimeSpan sessionDuration)
        {
            LastSessionEnded = DateTime.Now;
            sessionDuration = (!Completed && LastSessionEnded < LastSessionStarted) ? LastSessionEnded.Subtract(LastSessionStarted) : TimeSpan.Zero;
            if (!Completed)
                NetCourseTime = NetCourseTime.Add(sessionDuration);
        }

        public void UpdateWhenCourseCompleted(out TimeSpan sessionDuration)
        {
            UpdateAtSessionEnd(out sessionDuration);
            CourseFinished = DateTime.Now;
            Completed = true;
            ResumeOnIndex = 0;
        }
        
        public int GetIndexToResumeOn()
        {
            var index = ResumeOnIndex;
            if (index >= CourseProblemCount)
            {
                if (index - CourseProblemCount >= RequeuedProblems.Count)
                    throw new Exception("Došlo k překročení počtu příkladů v kurzu.");
            }
            return index;
        }

        public bool GetIsProblemSolved(int currentMathProblemIndex) => StudentAnswers.Count > currentMathProblemIndex;

        public void AddNewVisibleStepsCounter()
        {
            VisibleStepsCounts.Add(0);
        }

        public void RecordStepReveal(int problemIndex)
        {
            VisibleStepsCounts[problemIndex]++;
        }

        public void Save(string filename)
        {
            using (StreamWriter sw = new StreamWriter(filename))
            {
                XmlSerializer xmls = new XmlSerializer(typeof(UserCourseData));
                xmls.Serialize(sw, this);
            }
        }
        public UserCourseData Read(string filename)
        {
            using (StreamReader sr = new StreamReader(filename))
            {
                XmlSerializer xmls = new XmlSerializer(typeof(UserCourseData));
                return xmls.Deserialize(sr) as UserCourseData;
            }
        }
    }
}
