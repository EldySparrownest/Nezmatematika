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
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ClassName { get; set; }
        public string SchoolName { get; set; }
        public string UserType { get; set; }
        public UserStats UserStats { get; set; }
        public List<UserCourseData> CoursesData { get; set; }
    }
}
