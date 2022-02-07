using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMMathProblemsBase.Model
{
    public class UserStats
    {
        public TimeSpan TimeSpentInCourses;
        public int ProblemsSolvedTotal;
        public int ProblemsSolvedFirstTry;
        public int CoursesCompleted;

        public UserStats()
        {
            TimeSpentInCourses = TimeSpan.Zero;
            ProblemsSolvedTotal = 0;
            ProblemsSolvedFirstTry = 0;
            CoursesCompleted = 0;
        }
    }
}
