using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMMathProblemsBase.Model
{
    public class User
    {
        public string Id { get; set; }
        public AppMode UserType { get; set; }
        public string TitleBefore { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string TitleAfter { get; set; }
        public string SchoolName { get; set; }
        public string ClassName { get; set; }
        public string DisplayName { get; set; }
        public UserStats UserStats { get; set; }
        public List<UserCourseData> CoursesData { get; set; }

        public User()
        {
            Id = NewId();
            UserType = App.AppMode;  
            CoursesData = new List<UserCourseData>();
            UserStats = new UserStats(); 
        }

        public void UpdateDisplayName()
        {
            DisplayName = $"{TitleBefore} {FirstName} {LastName} {TitleAfter}".Trim();
        }

        private string NewId()
        {
            var modeName = App.AppMode.ToString();
            return modeName + string.Join(string.Empty, (DateTime.Now.ToString("yyyyMMddHHmmssffffff")).Split(" .:".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
        }
    }
}
