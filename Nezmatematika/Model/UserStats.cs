﻿using Nezmatematika.ViewModel.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Nezmatematika.Model
{
    public class UserStats
    {
        //student only
        [XmlIgnore]
        public TimeSpan TimeTakingCourses { get; set; }

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

        public int AnswersSentTotal { get; set; }
        public int ProblemsSolvedTotal { get; set; }
        public int ProblemsSolvedFirstTry { get; set; }
        public int ProblemsSolvedFirstTryNoHints { get; set; }
        public int HintsDisplayed { get; set; }
        public int CoursesStarted { get; set; }
        public int CoursesCompleted { get; set; }

        //teacher only
        public int ProblemsPublished { get; set; }
        public int CoursesCreated { get; set; }
        public int VersionsPublished { get; set; }
        public int UniqueCoursesPublished { get; set; }

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
            XmlHelper.Save(fullFilePath, this);
        }
        public static UserStats Read(string fullFilePath)
        {
            XmlHelper.TryDeserialiaze<UserStats>(fullFilePath, out var userStats);
            return userStats;
        }
    }
}
