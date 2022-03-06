using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace Nezmatematika.Model
{
    public class Course
    {
        public string Id { get; set; }
        public UserBase Author { get; set; }
        public DateTime Created { get; set; }
        public PublishingStatus PublishingStatus { get; set; }
        public DateTime LastPublished { get; set; }
        public int PublishedProblemCount { get; set; }
        public int Version { get; private set; }
        public DateTime LastOpened { get; set; }
        public DateTime LastEdited { get; set; }
        public TimeSpan TimeSpentEditing { get; set; }
        public string CourseTitle { get; set; }
        public string CourseDesc { get; set; }
        public string DirPath { get; set; }
        public string FilePath { get; set; }

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
            Version = serialisedCourse.Version;
            Created = serialisedCourse.Created;
            LastPublished = serialisedCourse.LastPublished;
            LastOpened = serialisedCourse.LastOpened;
            LastEdited = serialisedCourse.LastEdited;
            TimeSpentEditing = serialisedCourse.TimeSpentEditing;
            PublishingStatus = serialisedCourse.PublishingStatus;
            CourseTitle = serialisedCourse.CourseTitle;
            CourseDesc = serialisedCourse.CourseDesc;
            Problems = new ObservableCollection<MathProblem>();

            var factory = new MathProblemFactory();
            foreach (var problem in serialisedCourse.Problems)
            {
                Problems.Add(factory.CreateFromSerialised(problem));
            }
        }
        public Course(UserBase author, string title, string desc)
        {
            Author = author;
            Id = NewCourseId(author.Id);
            Version = 0;
            DirPath = CourseDirPath();
            FilePath = CourseFilePath();
            CourseTitle = title;
            CourseDesc = desc;
            Problems = new ObservableCollection<MathProblem>();
            Created = DateTime.Now;
            LastOpened = DateTime.Now;
            LastEdited = DateTime.Now;
            PublishingStatus = PublishingStatus.NotPublished;
            PublishedProblemCount = 0;
        }

        public void AddNewMathProblem()
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

        private void ReorderProblemIndexes()
        {
            for (int i = Problems.Count - 1; i >= 0; i--)
            {
                Problems[i].Index = i;
            }
        }

        private static string NewCourseId(string authorID)
            => string.Join("_", authorID,
                    string.Join("", (Convert.ToString(DateTime.Now.ToString("yyyyMMddHHmmssffffff"))).Split(" .:".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)));

        private string CourseDirPath()
            => DirPath = Path.Combine(App.MyBaseDirectory, "Courses", Author.Id, Id);

        private string CourseFilePath()
            => Path.Combine(App.MyBaseDirectory, "Courses", Author.Id, $"{Id}{GlobalValues.CourseFilename}");

        public void Save()
        {
            ReorderProblemIndexes();
            TimeSpentEditing = TimeSpentEditing + ((LastOpened > LastEdited ? LastOpened : LastEdited).Subtract(DateTime.Now));
            LastEdited = DateTime.Now;
            CourseSerialisable cs = new CourseSerialisable(this);

            cs.Save();
        }

        public void Publish(string publishedCoursesDirPath, string archivedCoursesDirPath, out int problemCountChange)
        {
            var prevVerDirName = $"{Id}_{Version}"; //adresářové jméno poslední publikované verze

            var prevStatus = PublishingStatus;
            var prevPublished = LastPublished;
            var prevPublishedProblemCount = PublishedProblemCount;
            problemCountChange = 0;

            try
            {
                var teacherCoursesDirPath = publishedCoursesDirPath.Replace("Published", Author.Id);
                var prevVersion = Version;
                Version++;
                PublishingStatus = PublishingStatus.PublishedUpToDate;
                LastPublished = DateTime.Now;
                PublishedProblemCount = Problems.Count;
                Save();

                var prevVersionCourseDir = Path.Combine(publishedCoursesDirPath, prevVerDirName); //adresářová cesta poslední publikované verze
                var archive = prevVersion != 0 && Directory.Exists(prevVersionCourseDir);

                if (archive)
                    Course.ArchiveCourse(Id, prevVersion, publishedCoursesDirPath, archivedCoursesDirPath);

                problemCountChange = PublishedProblemCount - prevPublishedProblemCount;
                PublishCourse(Id, Version, teacherCoursesDirPath, publishedCoursesDirPath);
            }
            catch (Exception e)
            {
                PublishingStatus = prevStatus;
                LastPublished = prevPublished;
                PublishedProblemCount = prevPublishedProblemCount;
                Save();
                MessageBox.Show(e.Message);
            }
        }

        public static void ArchiveCourse(string courseId, int courseVersion, string publishedCoursesDirPath, string archivedCoursesDirPath)
        {
            var problemsDirName = $"{courseId}_{courseVersion}";
            var publishedFilePath = Path.Combine(publishedCoursesDirPath, $"{problemsDirName}{GlobalValues.CourseFilename}");
            var course = Course.Read(publishedFilePath);

            if (course == null)
                return;

            course.UpdatePathsToAdjustForMovingFiles(publishedCoursesDirPath, archivedCoursesDirPath);
            course.Save(); //hlavní kurzový soubor se rovnou uloží kam má

            //přesun adresáře s RTF soubory úloh
            Directory.Move(Path.Combine(publishedCoursesDirPath, problemsDirName), Path.Combine(archivedCoursesDirPath, problemsDirName));

            //výmaz hlavního souboru z publikovaných
            File.Delete(publishedFilePath);
        }

        private void PublishCourse(string courseId, int courseVersion, string teacherCoursesDirPath, string publishedCoursesDirPath)
        {
            var problemsDirName = $"{courseId}_{courseVersion}";
            var teacherFilePath = Path.Combine(teacherCoursesDirPath, $"{courseId}{GlobalValues.CourseFilename}");
            var course = Course.Read(teacherFilePath);

            if (course == null)
                return;

            course.UpdatePathsToAdjustForMovingFiles(teacherCoursesDirPath, publishedCoursesDirPath);
            course.Save(); //hlavní kurzový soubor se rovnou uloží kam má (a případně si i dovytvoří adresáře)

            //kopírování adresáře s RTF soubory úloh
            var courseDirPublished = Path.Combine(publishedCoursesDirPath, problemsDirName);
            Directory.CreateDirectory(courseDirPublished);
            if (Directory.Exists(DirPath) && Directory.Exists(courseDirPublished))
            {
                var newFiles = Directory.EnumerateFiles(DirPath);
                foreach (var file in newFiles)
                {
                    var newFilePath = file.Replace(DirPath, courseDirPublished);
                    File.Copy(file, newFilePath, true);
                }
            }
        }

        public void UpdatePathsToAdjustForMovingFiles(string oldCoursesDir, string newCoursesDir, bool addVersionSuffix = false)
        {
            var versionSuffix = $"_{Version}";
            //DirPath = DirPath.Replace(oldCoursesDir, newCoursesDir) + versionSuffix;
            DirPath = Path.Combine(newCoursesDir, String.Join("", Id, versionSuffix));
            FilePath = Path.Combine(newCoursesDir, String.Join("",Id,versionSuffix, GlobalValues.CourseFilename));

            foreach (var mathProblem in Problems)
            {
                mathProblem.DirPath = DirPath;
                mathProblem.FilePath = Path.Combine(DirPath, $"{mathProblem.Id}.rtf");
            }
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
