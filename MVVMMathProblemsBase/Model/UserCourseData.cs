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
        public string UserId { get; set; }
        public int Mistakes { get; set; }
        public DateTime CourseStarted { get; set; }
        public DateTime CourseFinished { get; set; }
        public DateTime LastSessionStarted { get; set; }
        public DateTime LastSessionEnded { get; set; }
        public TimeSpan NetCourseTime { get; set; }
        public int CurrentMathProblemIndex { get; set; }
        public List <int> RequeudProblems { get; set; }
    }
}
