﻿using System;
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
        public User Author { get; set; }
        public DateTime Created { get; set; }
        public PublishingStatus PublishingStatus { get; set; }
        public DateTime LastPublished { get; set; }
        public int Version { get; private set; }
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
            Version = serialisedCourse.Version;
            Created = serialisedCourse.Created;
            LastPublished = serialisedCourse.LastPublished;
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
            Version = 0;
            DirPath = CourseDirPath();
            FilePath = CourseFilePath();
            CourseTitle = title;
            CourseDesc = desc;
            Tags = tags;
            Problems = new ObservableCollection<MathProblem>();
            Created = DateTime.Now;
            LastOpened = DateTime.Now;
            LastEdited = DateTime.Now;
            PublishingStatus = PublishingStatus.NotPublished;
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
            TimeSpentEditing = TimeSpentEditing + ((LastOpened > LastEdited ? LastOpened : LastEdited) - DateTime.Now);
            LastEdited = DateTime.Now;
            CourseSerialisable cs = new CourseSerialisable(this);

            cs.Save();
        }

        public void Publish(string publishedCoursesDirPath, string archivedCoursesPath)
        {
            var prevStatus = PublishingStatus;
            var prevPublished = LastPublished;

            try
            {
                Directory.CreateDirectory(publishedCoursesDirPath);

                var prevVerDirName = $"{Id}_{Version}";
                var prevVersionCourseDir = Path.Combine(publishedCoursesDirPath, prevVerDirName);
                var archive = Version != 0 && Directory.Exists(prevVersionCourseDir);

                Version++;
                PublishingStatus = PublishingStatus.PublishedUpToDate;
                LastPublished = DateTime.Now;
                Save();

                if (archive)
                {                   
                    Directory.CreateDirectory(Path.Combine(archivedCoursesPath, prevVerDirName));
                    var filesToArchive = Directory.EnumerateFiles(prevVersionCourseDir);

                    foreach (var file in filesToArchive)
                    {
                        var newFilePath = file.Replace(publishedCoursesDirPath, archivedCoursesPath);
                        File.Copy(file, newFilePath, true);
                    }
                    Directory.Delete(prevVersionCourseDir, true);
                    File.Delete(Path.Combine(publishedCoursesDirPath,$"{prevVerDirName}{GlobalValues.CourseFilename}"));   
                }

                var newCourseIDDirPath = Path.Combine(publishedCoursesDirPath, $"{Id}_{Version}");
                Directory.CreateDirectory(newCourseIDDirPath);
                if (Directory.Exists(DirPath) && Directory.Exists(newCourseIDDirPath))
                {
                    var newFiles = Directory.EnumerateFiles(DirPath);

                    foreach (var file in newFiles)
                    {
                        var newFilePath = file.Replace(DirPath, newCourseIDDirPath);
                        File.Copy(file, newFilePath, true);
                    }
                }
                File.Copy(FilePath, Path.Combine(publishedCoursesDirPath, $"{Id}_{Version}{GlobalValues.CourseFilename}"), true);
            }
            catch (Exception e)
            {
                PublishingStatus = prevStatus;
                LastPublished = prevPublished;
                MessageBox.Show(e.Message);
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
