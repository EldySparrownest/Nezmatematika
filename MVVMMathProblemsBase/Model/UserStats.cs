using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nezmatematika.Model
{
    public class UserStats
    {
        //shared
        public TimeSpan TimeSpentInCourses;

        //student only
        public int ProblemsSolvedTotal;
        public int ProblemsSolvedFirstTry;
        public int ProblemsSolvedFirstTryNoHints;
        public int UniqueProblemsSolvedTotal;
        public int UniqueProblemsSolvedFirstTry;
        public int UniqueProblemsSolvedFirstTryNoHints;
        public int HintsDisplayed;
        public int CoursesStarted;
        public int CoursesCompleted;

        //teacher only
        public int CoursesCreated;
        public int VersionsPublished;
        public int UniqueCoursesPublished;

        public UserStats()
        {
            TimeSpentInCourses = TimeSpan.Zero;
            
            ProblemsSolvedTotal = 0;
            ProblemsSolvedFirstTry = 0;
            ProblemsSolvedFirstTryNoHints = 0;
            CoursesCompleted = 0;
            UniqueProblemsSolvedTotal = 0;
            UniqueProblemsSolvedFirstTry = 0;
            UniqueProblemsSolvedFirstTryNoHints = 0;
            HintsDisplayed = 0;
            CoursesStarted = 0;
            CoursesCompleted = 0;

            CoursesCreated = 0;
            VersionsPublished = 0;
            UniqueCoursesPublished = 0;
    }
    }
}
