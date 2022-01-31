using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMMathProblemsBase.Model
{
    [Serializable]
    public class UserCourseData
    {
        public string CourseId { get; set; }
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
        public List <int> RequeuedProblems { get; set; }

        public UserCourseData()
        {
        }

        public UserCourseData(Course course, string userId, DateTime startTime)
        {
            CourseId = course.Id;
            CourseTitle = course.CourseTitle;
            CourseAuthor = course.Author.DisplayName;
            CourseProblemCount = course.Problems.Count;
            UserId = userId;
            Mistakes = 0;
            Completed = false;
            CourseStarted = startTime;
            ResumeOnIndex = 0;
            SolvedProblemsCount = 0;
            RequeuedProblems = new List<int>();
        }

        public int GetIndexToResumeOn()
        {
            var index = ResumeOnIndex;
            if (index >= CourseProblemCount)
            {
                index = index - CourseProblemCount;
                if (index < RequeuedProblems.Count)
                    throw new Exception("Došlo k překročení počtu příkladů v kurzu.");
            }
            return index;
        }
    }
}
