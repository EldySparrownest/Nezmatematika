using Nezmatematika.ViewModel.Helpers;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;

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
        public string RelDirPath { get; set; }
        public string RelFilePath { get; set; }

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
            RelDirPath = serialisedCourse.RelDirPath;
            RelFilePath = serialisedCourse.RelFilePath;
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
            Problems = new ObservableCollection<MathProblem>();

            var factory = new MathProblemFactory();
            foreach (var problem in serialisedCourse.Problems)
            {
                Problems.Add(factory.CreateFromSerialised(problem));
            }
        }
        public Course(UserBase author, string title)
        {
            Author = author;
            Id = NewCourseId(author.Id);
            Version = 0;
            RelDirPath = Path.Combine(FilePathHelper._TeacherCoursesRelDirPath(Author), Id);
            RelFilePath = Path.Combine(FilePathHelper._TeacherCoursesRelDirPath(Author), $"{Id}{GlobalValues.CourseFilename}");
            CourseTitle = title;
            Problems = new ObservableCollection<MathProblem>();
            Created = DateTime.Now;
            LastOpened = DateTime.Now;
            LastEdited = DateTime.Now;
            PublishingStatus = PublishingStatus.NotPublished;
            PublishedProblemCount = 0;
        }

        public void AddNewMathProblem(bool capitalisationMatters)
        {
            var mathProblemFactory = new MathProblemFactory();
            Problems.Add((MathProblem)mathProblemFactory.Create(this, capitalisationMatters, "základní"));
        }

        private void UpdateProblemIndexesAndOrderLabels()
        {
            for (int i = Problems.Count - 1; i >= 0; i--)
            {
                Problems[i].Index = i;
                Problems[i].SetSimplifiedOrderLabel();
            }
        }

        private static string NewCourseId(string authorID)
            => string.Join("_", authorID,
                    string.Join("", Convert.ToString(DateTime.Now.ToString("yyyyMMddHHmmssffffff")).Split(" .:".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)));

        public void Save()
        {
            UpdateProblemIndexesAndOrderLabels();
            TimeSpentEditing = TimeSpentEditing + (LastOpened > LastEdited ? LastOpened : LastEdited).Subtract(DateTime.Now);
            LastEdited = DateTime.Now;

            new CourseSerialisable(this).Save();
        }

        public void PublishCourse(string publishedCoursesRelDirPath, string archivedCoursesRelDirPath, out int problemCountChange)
        {
            var prevVerDirName = $"{Id}_{Version}"; //adresářové jméno poslední publikované verze

            var prevStatus = PublishingStatus;
            var prevPublished = LastPublished;
            var prevPublishedProblemCount = PublishedProblemCount;
            problemCountChange = 0;

            try
            {
                var teacherCoursesDirPath = FilePathHelper._TeacherCoursesRelDirPath(Author);
                var prevVersion = Version;
                Version++;
                PublishingStatus = PublishingStatus.PublishedUpToDate;
                LastPublished = DateTime.Now;
                PublishedProblemCount = Problems.Count;
                Save();

                var prevVersionCourseRelDirPath = Path.Combine(publishedCoursesRelDirPath, prevVerDirName); //adresářová cesta poslední publikované verze
                var archive = prevVersion != 0 && Directory.Exists(Path.Combine(App.MyBaseDirectory, prevVersionCourseRelDirPath));

                if (archive)
                    Course.ArchiveCourse(Id, prevVersion, publishedCoursesRelDirPath, archivedCoursesRelDirPath);

                problemCountChange = PublishedProblemCount - prevPublishedProblemCount;
                CopyAllCourseFiles(Id, Version, teacherCoursesDirPath, publishedCoursesRelDirPath); // obě cesty v parametrech musejí být relativní!
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
        private void CopyAllCourseFiles(string courseId, int courseVersion, string originalParentDirPath, string newParentDirPath)
        {
            var problemsDirName = $"{courseId}_{courseVersion}";
            var fullOriginalFilePath = Path.Combine(App.MyBaseDirectory, originalParentDirPath, $"{courseId}{GlobalValues.CourseFilename}");
            var course = Course.Read(fullOriginalFilePath);

            if (course == null)
                return;

            course.UpdatePathsToAdjustForMovingFiles(newParentDirPath);
            course.Save(); //hlavní kurzový soubor se rovnou uloží kam má (a případně si i dovytvoří adresáře)

            //kopírování adresáře s RTF soubory úloh
            var fullNewDirPath = Path.Combine(App.MyBaseDirectory, newParentDirPath, problemsDirName);
            var fullOldProblemsDirPath = Path.Combine(App.MyBaseDirectory, originalParentDirPath, courseId);
            Directory.CreateDirectory(fullNewDirPath);
            if (Directory.Exists(fullOldProblemsDirPath) && Directory.Exists(fullNewDirPath))
            {
                var newFiles = Directory.EnumerateFiles(fullOldProblemsDirPath);
                foreach (var file in newFiles)
                {
                    var newFilePath = file.Replace(fullOldProblemsDirPath, fullNewDirPath);
                    File.Copy(file, newFilePath, true);
                }
            }
        }

        public static void ArchiveCourse(string courseId, int courseVersion)
        {
            ArchiveCourse(courseId, courseVersion, FilePathHelper._CoursesPublishedRelDirPath(), FilePathHelper._CoursesArchivedRelDirPath());
        }
        public static void ArchiveCourse(string courseId, int courseVersion, string publishedCoursesRelDirPath, string archivedCoursesRelDirPath)
        {
            var problemsDirName = $"{courseId}_{courseVersion}";
            var archivedCoursesFullDirPath = Path.Combine(App.MyBaseDirectory, archivedCoursesRelDirPath);
            var publishedCoursesFullDirPath = Path.Combine(App.MyBaseDirectory, publishedCoursesRelDirPath);
            var publishedFullFilePath = Path.Combine(App.MyBaseDirectory, publishedCoursesRelDirPath, $"{problemsDirName}{GlobalValues.CourseFilename}");
            var course = Course.Read(publishedFullFilePath);

            if (course == null)
                return;

            course.UpdatePathsToAdjustForMovingFiles(archivedCoursesRelDirPath);
            course.Save(); //hlavní kurzový soubor se rovnou uloží kam má

            //přesun adresáře s RTF soubory úloh
            Directory.Move(Path.Combine(publishedCoursesFullDirPath, problemsDirName), Path.Combine(archivedCoursesFullDirPath, problemsDirName));

            //výmaz hlavního souboru z publikovaných
            File.Delete(publishedFullFilePath);
        }

        public void PrepForExport(string courseId, int courseVersion, string sourceRelDir, string targetRelDir)
        {
            var idVersionString = $"{courseId}_{courseVersion}";
            var originalFileFullPath = Path.Combine(App.MyBaseDirectory, sourceRelDir, $"{idVersionString}{GlobalValues.CourseFilename}");
            var fileForExportFullPath = Path.Combine(App.MyBaseDirectory, targetRelDir, $"{idVersionString}{GlobalValues.CourseFilename}");
            File.Copy(originalFileFullPath, fileForExportFullPath, true);

            //kopírování adresáře s RTF soubory úloh
            var newFullDirPath = Path.Combine(App.MyBaseDirectory, targetRelDir, idVersionString);
            var oldFullDirPath = Path.Combine(App.MyBaseDirectory, RelDirPath);
            Directory.CreateDirectory(newFullDirPath);
            if (Directory.Exists(oldFullDirPath) && Directory.Exists(newFullDirPath))
            {
                var newFiles = Directory.EnumerateFiles(oldFullDirPath);
                foreach (var file in newFiles)
                {
                    var newFilePath = file.Replace(oldFullDirPath, newFullDirPath);
                    File.Copy(file, newFilePath, true);
                }
            }
        }

        public void UpdatePathsToAdjustForMovingFiles(string newCoursesDir)
        {
            var versionSuffix = $"_{Version}";
            var courseDirName = String.Join("", Id, versionSuffix);
            var fileName = String.Join("", Id, versionSuffix, GlobalValues.CourseFilename);
            RelDirPath = Path.Combine(newCoursesDir, courseDirName);
            RelFilePath = Path.Combine(newCoursesDir, fileName);

            foreach (var mathProblem in Problems)
            {
                mathProblem.DirPath = RelDirPath;
                mathProblem.RelFilePath = Path.Combine(newCoursesDir, courseDirName, $"{mathProblem.Id}.rtf");
            }
        }

        public void Delete()
        {
            var fullDirPath = Path.Combine(App.MyBaseDirectory, RelDirPath);
            var fullFilePath = Path.Combine(App.MyBaseDirectory, RelFilePath);
            if (Directory.Exists(fullDirPath))
            {
                var mathProblemFilePaths = Directory.GetFiles(fullDirPath);
                foreach (var path in mathProblemFilePaths)
                {
                    File.Delete(path);
                }
                Directory.Delete(fullDirPath);
            }
            if (File.Exists(fullFilePath))
            {
                File.Delete(fullFilePath);
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
