using Nezmatematika.ViewModel.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Nezmatematika.Model
{
    public class CourseSerialisable
    {
        public string Id { get; set; }
        public int Version { get; set; }
        public string RelDirPath { get; set; }
        public string RelFilePath { get; set; }
        public UserBase Author { get; set; }
        public DateTime Created { get; set; }
        public PublishingStatus PublishingStatus { get; set; }
        public DateTime LastPublished { get; set; }
        public DateTime LastEdited { get; set; }
        public DateTime LastOpened { get; set; }
        public TimeSpan TimeSpentEditing { get; set; }
        public string CourseTitle { get; set; }

        public List<MathProblemSerialisable> Problems;

        public CourseSerialisable()
        {
            Created = DateTime.Now;
            LastOpened = DateTime.Now;
            LastEdited = DateTime.Now;
            TimeSpentEditing = TimeSpan.Zero;
            Problems = new List<MathProblemSerialisable>();
        }

        public CourseSerialisable(Course course)
        {
            RelDirPath = course.RelDirPath;
            RelFilePath = course.RelFilePath;
            Author = course.Author;
            Id = course.Id;
            Version = course.Version;
            Created = course.Created;
            LastOpened = course.LastOpened;
            LastEdited = course.LastEdited;
            TimeSpentEditing = course.TimeSpentEditing;
            LastPublished = course.LastPublished;
            TimeSpentEditing = course.TimeSpentEditing;
            PublishingStatus = course.PublishingStatus;
            CourseTitle = course.CourseTitle;
            Problems = new List<MathProblemSerialisable>();

            var factory = new MathProblemFactory();
            foreach (MathProblem problem in course.Problems)
            {
                Problems.Add(factory.CreateFromMathProblem(problem));
            }
        }

        public void Save()
        {
            XmlHelper.Save(Path.Combine(App.MyBaseDirectory, RelFilePath), typeof(CourseSerialisable), this);
        }
        public static CourseSerialisable Read(string filePath)
        {
            filePath = Path.Combine(App.MyBaseDirectory, filePath);
            using (StreamReader sw = new StreamReader(filePath))
            {
                XmlSerializer xmls = new XmlSerializer(typeof(CourseSerialisable));
                return xmls.Deserialize(sw) as CourseSerialisable;
            }
        }
    }
}
