using Nezmatematika.ViewModel.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Nezmatematika.Model
{
    public class UserStats
    {
        //student only
        [XmlIgnore]
        public TimeSpan TimeTakingCourses;

        // XmlSerializer does not support TimeSpan, so use this property for serialization instead.
        [Browsable(false)]
        [XmlElement(DataType = "duration", ElementName = "TimeTakingCourses")]
        public string TimeTakingCoursesString
        {
            get
            {
                return XmlConvert.ToString(TimeTakingCourses);
            }
            set
            {
                TimeTakingCourses = string.IsNullOrEmpty(value) ? TimeSpan.Zero : XmlConvert.ToTimeSpan(value);
            }
        }

        public int AnswersSentTotal;
        public int ProblemsSolvedTotal;
        public int ProblemsSolvedFirstTry;
        public int ProblemsSolvedFirstTryNoHints;
        public int HintsDisplayed;
        public int CoursesStarted;
        public int CoursesCompleted;

        //teacher only
        public int ProblemsPublished;
        public int CoursesCreated;
        public int VersionsPublished;
        public int UniqueCoursesPublished;

        public UserStats()
        {
            TimeTakingCourses = TimeSpan.Zero;
            AnswersSentTotal = 0;
            ProblemsSolvedTotal = 0;
            ProblemsSolvedFirstTry = 0;
            ProblemsSolvedFirstTryNoHints = 0;
            HintsDisplayed = 0;
            CoursesStarted = 0;
            CoursesCompleted = 0;

            ProblemsPublished = 0;
            CoursesCreated = 0;
            VersionsPublished = 0;
            UniqueCoursesPublished = 0;
        }

        public void NewCourseStartedUpdate()
        {
            CoursesStarted++;
        }
        public void HintDisplayedUpdate()
        {
            HintsDisplayed++;
        }
        public void AnswerSentUpdate(bool wasCorrect, bool wasFirstTry, bool withoutHints)
        {
            AnswersSentTotal++;
            if (wasCorrect)
            {
                ProblemsSolvedTotal++;
                if (wasFirstTry)
                {
                    ProblemsSolvedFirstTry++;
                    if (withoutHints)
                        ProblemsSolvedFirstTryNoHints++;
                }
            }
        }
        public void CourseCompletedUpdate()
        {
            CoursesCompleted++;
        }
        public void SessionEndUpdate(TimeSpan sessionDuration)
        {
            TimeTakingCourses = TimeTakingCourses.Add(sessionDuration);
        }
        public void CourseCreatedUpdate()
        {
            CoursesCreated++;
        }
        public void CoursePublishedUpdate(int version, int newProblems)
        {
            ProblemsPublished += newProblems;
            VersionsPublished++;
            if (version == 1)
                UniqueCoursesPublished++;
        }

        public string GetDisplayableTimeTakingCourses()
        {
            var hoursTakingCourses = TimeTakingCourses.Days * 24 + TimeTakingCourses.Hours;
            var restOfTimeSpan = TimeTakingCourses.ToString(@"mm\:ss");
            return $"{hoursTakingCourses}:{restOfTimeSpan}";
        }

        public Dictionary<string, string> GetAsDictionary()
        {
            var dic = new Dictionary<string, string>();

            dic.Add("CurrentTimeTakingCourses", GetDisplayableTimeTakingCourses());
            dic.Add("CurrentAnswersSentTotal", AnswersSentTotal.ToString());
            dic.Add("CurrentProblemsSolvedTotal", ProblemsSolvedTotal.ToString());
            dic.Add("CurrentProblemsSolvedFirstTry", ProblemsSolvedFirstTry.ToString());
            dic.Add("CurrentProblemsSolvedFirstTryNoHints", ProblemsSolvedFirstTryNoHints.ToString());
            dic.Add("CurrentHintsDisplayed", HintsDisplayed.ToString());
            dic.Add("CurrentCoursesStarted", CoursesStarted.ToString());
            dic.Add("CurrentCoursesCompleted", CoursesCompleted.ToString());

            dic.Add("CurrentProblemsPublished", ProblemsPublished.ToString());
            dic.Add("CurrentCoursesCreated", CoursesCreated.ToString());
            dic.Add("CurrentVersionsPublished", VersionsPublished.ToString());
            dic.Add("CurrentUniqueCoursesPublished", UniqueCoursesPublished.ToString());

            return dic;
        }

        public void Save(string fullFilePath)
        {
            XmlHelper.Save(fullFilePath, typeof(UserStats), this);
        }
        public static UserStats Read(string filename)
        {
            using (StreamReader sr = new StreamReader(filename))
            {
                XmlSerializer xmls = new XmlSerializer(typeof(UserStats));
                return xmls.Deserialize(sr) as UserStats;
            }
        }
    }
}
