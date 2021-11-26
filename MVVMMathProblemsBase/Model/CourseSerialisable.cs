using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MVVMMathProblemsBase.Model
{
    public class CourseSerialisable
    {
        public string Id { get; set; }
        public string DirPath { get; set; }
        public string FilePath { get; set; }
        public User Author { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastEdited { get; set; }
        public DateTime LastOpened { get; set; }
        public TimeSpan TimeSpentEditing { get; set; }
        public string CourseTitle { get; set; }
        public string CourseDesc { get; set; }
        public List<string> Tags { get; set; }

        public List<MathProblemSerialisable> Problems;

        public CourseSerialisable()
        {
            Created = DateTime.Now;
            LastOpened = DateTime.Now;
            LastEdited = DateTime.Now;
            TimeSpentEditing = TimeSpan.Zero;
            Tags = new List<string>();
            Problems = new List<MathProblemSerialisable>();
        }

        public CourseSerialisable(Course course)
        {
            DirPath = course.DirPath;
            FilePath = course.FilePath;
            Author = course.Author;
            Id = course.Id;
            Created = course.Created;
            LastOpened = course.LastOpened;
            LastEdited = course.LastEdited;
            TimeSpentEditing = course.TimeSpentEditing;
            CourseTitle = course.CourseTitle;
            CourseDesc = course.CourseDesc;
            Tags = course.Tags.ToList();
            Problems = new List<MathProblemSerialisable>();

            var factory = new MathProblemFactory();
            foreach (MathProblem problem in course.Problems)
            {
                Problems.Add(factory.CreateFromMathProblem(problem));
            }
        }

        public void Save(string filename)
        {
            using (StreamWriter sw = new StreamWriter(filename))
            {
                XmlSerializer xmls = new XmlSerializer(typeof(CourseSerialisable));
                xmls.Serialize(sw, this);
            }
        }
        public static CourseSerialisable Read(string filename)
        {
            using (StreamReader sw = new StreamReader(filename))
            {
                XmlSerializer xmls = new XmlSerializer(typeof(CourseSerialisable));
                return xmls.Deserialize(sw) as CourseSerialisable;
            }
        }
    }
}
