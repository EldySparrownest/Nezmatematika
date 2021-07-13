using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MVVMMathProblemsBase.Model
{
    [Serializable]
    public class UserCourseData
    {
        public string CourseId { get; set; }
        public string UserId { get; set; }
        public int Mistakes { get; set; }
        public int CourseStarted { get; set; }
        public int CourseFinished { get; set; }
        public TimeSpan NetCourseTime { get; set; }
        public int ProblemBookmark { get; set; }
    }
    [Serializable]
    public class Course
    {
        public string Id { get; set; }
        public User Author { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastEdited { get; set; }
        public TimeSpan TimeSpentEditing { get; set; }
        public string CourseTitle { get; set; }
        [field: NonSerialized] 
        public ObservableCollection<string> Tags { get; set; }

        private List<MathProblem> problemsList;
        [field: NonSerialized]
        private ObservableCollection<MathProblem> problems;
        [field: NonSerialized] 
        public ObservableCollection<MathProblem> Problems 
        { 
            get { return problems; } 
            set
            {
                problems = value;
                //OnPropertyChanged("Problems");
            }
        }

        public void AddNewMathProblem(MathProblem mathProblem)
        {
            if (Problems.Count == 0)
            {
                mathProblem.OrderLabel = "1.";
            }
            else
            {
                string lastLabel = Problems[Problems.Count - 1].OrderLabel;
                int dotIndex = lastLabel.IndexOf('.');
                string[] lastLabelArray = lastLabel.Split(".".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                int newLabelNbr;
                if (lastLabelArray.Length == 1
                    && Int32.TryParse(lastLabelArray[0], out newLabelNbr))
                {
                    newLabelNbr++;
                    mathProblem.OrderLabel = String.Concat(Convert.ToString(newLabelNbr), lastLabel.Substring(dotIndex));
                }
                else
                {
                    mathProblem.OrderLabel = Problems[Problems.Count - 1].OrderLabel + ".";
                }
            }
            Problems.Add(mathProblem);
        }
        public void Save(string filename)
        {
            problemsList = Problems.ToList<MathProblem>();
            using (StreamWriter sw = new StreamWriter(filename))
            {
                XmlSerializer xmls = new XmlSerializer(typeof(Course));
                xmls.Serialize(sw, this);
            }
        }
        public Course Read(string filename)
        {
            using (StreamReader sw = new StreamReader(filename))
            {
                XmlSerializer xmls = new XmlSerializer(typeof(Course));
                return xmls.Deserialize(sw) as Course;
            }
        }
    }
}
