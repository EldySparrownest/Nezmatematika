using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MVVMMathProblemsBase.Model
{
    public class Course
    {
        public string Id { get; set; }
        public User Author { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastOpened { get; set; }
        public DateTime LastEdited { get; set; }
        public TimeSpan TimeSpentEditing { get; set; }
        public string CourseTitle { get; set; }
        public string CourseDesc { get; set; }
        public string DirPath { get; set; }
        public string FilePath { get; set; }
        public ObservableCollection<string> Tags { get; set; }

        private ObservableCollection<MathProblem> problems;
        public ObservableCollection<MathProblem> Problems 
        { 
            get { return problems; } 
            set
            {
                problems = value;
                OnPropertyChanged("Problems");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Course(CourseSerialisable serialisedCourse)
        {
            DirPath = serialisedCourse.DirPath;
            FilePath = serialisedCourse.FilePath;
            Author = serialisedCourse.Author;
            Id = serialisedCourse.Id;
            Created = serialisedCourse.Created;
            LastOpened = serialisedCourse.LastOpened;
            LastEdited = serialisedCourse.LastEdited;
            TimeSpentEditing = serialisedCourse.TimeSpentEditing;
            CourseTitle = serialisedCourse.CourseTitle;
            CourseDesc = serialisedCourse.CourseDesc;
            Tags = new ObservableCollection<string>(serialisedCourse.Tags);
            Problems = new ObservableCollection<MathProblem>();

            var factory = new MathProblemFactory();
            foreach (var problem in serialisedCourse.Problems)
            {
                Problems.Add(factory.CreateFromSerialised(problem));
            }
        }
        public Course(User author, string title, string desc, ObservableCollection<string> tags)
        {
            Author = author;
            Id = NewCourseId(author.Id);
            DirPath = CourseDirPath();
            FilePath = CourseFilePath();
            CourseTitle = title;
            CourseDesc = desc;
            Tags = tags;
            Problems = new ObservableCollection<MathProblem>();
            Created = DateTime.Now;
            LastOpened = DateTime.Now;
            LastEdited = DateTime.Now;
        }

        public void AddNewMathProblem(MathProblem mathProblem)
        {
            string precedingLabel;
            if (Problems.Count == 0)
            {
                precedingLabel = "0";
            }
            else
            {
                precedingLabel = Problems[Problems.Count - 1].OrderLabel;
            }

            var mathProblemFactory = new MathProblemFactory();
            Problems.Add((MathProblem)mathProblemFactory.Create(this, "základní", precedingLabel));
        }

        private static string NewCourseId(string authorID)
            => string.Join("_", authorID,
                    string.Join("", (Convert.ToString(DateTime.Now.ToString("yyyyMMddHHmmssffffff"))).Split(" .:".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)));

        private string CourseDirPath()
            => DirPath = System.IO.Path.Combine(Environment.CurrentDirectory, "Courses", Author.Id, Id);

        private string CourseFilePath()
            => System.IO.Path.Combine(Environment.CurrentDirectory, "Courses", Author.Id, $"{Id}{GlobalValues.CourseFilename}");

        public void Save()
        {
            TimeSpentEditing = TimeSpentEditing + ((LastOpened > LastEdited ? LastOpened : LastEdited) - DateTime.Now);
            LastEdited = DateTime.Now;
            CourseSerialisable cs = new CourseSerialisable(this);

            cs.Save();
        }

        public static Course Read(string filename)
        {
            var cs = CourseSerialisable.Read(filename);
            return new Course(cs);
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
