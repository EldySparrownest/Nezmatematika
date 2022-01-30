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
        public string UserType { get; set; }
        public string TitleBefore { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string TitleAfter { get; set; }
        public string SchoolName { get; set; }
        public string ClassName { get; set; }
        public string DisplayName { get; set; }
        public UserStats UserStats { get; set; }
        public List<UserCourseData> CoursesData { get; set; }

        public void UpdateDisplayName()
        {
            DisplayName = string.Empty;
            if (!String.IsNullOrWhiteSpace(TitleBefore))
                DisplayName += $"{TitleBefore} ";
            DisplayName += $"{FirstName} {LastName}";
            if (!String.IsNullOrWhiteSpace(TitleAfter))
                DisplayName += $", {TitleAfter}";
        }
    }
}
