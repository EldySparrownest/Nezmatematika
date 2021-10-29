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
        public int CourseStarted { get; set; }
        public int CourseFinished { get; set; }
        public TimeSpan NetCourseTime { get; set; }
        public int CurrentMathProblemIndex { get; set; }
    }
}
