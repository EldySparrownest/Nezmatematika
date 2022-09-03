using Nezmatematika.ViewModel.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
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

        public UserCourseData(Course course, DateTime startTime)
        {
            CourseId = course.Id;
            Version = course.Version;
            CourseTitle = course.CourseTitle;
            CourseAuthor = course.Author.DisplayName;
            CourseProblemCount = course.Problems.Count;
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

            completed = SolvedProblemsCount == CourseProblemCount + RequeuedProblems.Count;
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
            sessionDuration = (!Completed && LastSessionEnded > LastSessionStarted) ? LastSessionEnded.Subtract(LastSessionStarted) : TimeSpan.Zero;
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
                    throw new Exception("Došlo k překročení počtu úloh v kurzu.");
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

        public void Save(string fullFilePath)
        {
            XmlHelper.Save(fullFilePath, this);
        }
        public UserCourseData Read(string fullFilePath)
        {
            XmlHelper.TryDeserialiaze<UserCourseData>(fullFilePath, out var userCourseData);
            return userCourseData;
        }
    }

    [Serializable]
    public class CopyOfUserCourseData
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

        public CopyOfUserCourseData()
        {
            RequeuedProblems = new List<int>();
            VisibleStepsCounts = new List<int>();
            StudentAnswers = new List<string>();
        }

        public CopyOfUserCourseData(Course course, string userId, DateTime startTime)
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

            completed = SolvedProblemsCount == CourseProblemCount + RequeuedProblems.Count;
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
            sessionDuration = (!Completed && LastSessionEnded > LastSessionStarted) ? LastSessionEnded.Subtract(LastSessionStarted) : TimeSpan.Zero;
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
                    throw new Exception("Došlo k překročení počtu úloh v kurzu.");
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

        public void Save(string fullFilePath)
        {
            XmlHelper.Save(fullFilePath, this);
        }
        public CopyOfUserCourseData Read(string fullFilePath)
        {
            XmlHelper.TryDeserialiaze<CopyOfUserCourseData>(fullFilePath, out var copyOfUserCourseData);
            return copyOfUserCourseData;
        }
    }
}
