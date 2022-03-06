using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nezmatematika.Model
{
    public class UserBase
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

        public UserBase()
        {
            Id = NewId();
            UserType = App.AppMode;
        }

        public UserBase(string titBef, string fName, string lName, string titAft, string sName, string cName)
        {
            Id = NewId();
            UserType = App.AppMode;
            Edit(titBef, fName, lName, titAft, sName, cName);
        }

        public void Edit(string titBef, string fName, string lName, string titAft, string sName, string cName)
        {
            TitleBefore = titBef;
            FirstName = fName;
            LastName = lName;
            TitleAfter = titAft;
            SchoolName = sName;
            ClassName = cName;
            UpdateDisplayName();
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
