using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMMathProblemsBase.Model
{
    public class User : IComparable, INotifyPropertyChanged
    {

        //nested class for ascending sort by last name, first name, class
        private class sortLNameFNameClassSchoolDescendingHelper : IComparer
        {
            int IComparer.Compare(object a, object b)
            {
                User u1 = (User)a;
                User u2 = (User)b;
                int res = String.Compare(u2.LastName, u1.LastName);
                if (res != 0)
                    return res;
                res = String.Compare(u2.FirstName, u1.FirstName);
                if (res != 0)
                    return res;
                res = String.Compare(u2.ClassName, u1.ClassName);
                if (res != 0)
                {
                    return res;
                }
                res = String.Compare(u2.SchoolName, u1.SchoolName);
                return res;
            }
        }
        //nested class for ascending sort by class, last name, first name
        private class sortSchoolClassLNameFNameAscendingHelper : IComparer
        {
            int IComparer.Compare(object a, object b)
            {
                User u1 = (User)a;
                User u2 = (User)b;
                int res = String.Compare(u1.SchoolName, u2.SchoolName);
                if (res != 0)
                {
                    return res;
                }
                res = String.Compare(u1.ClassName, u2.ClassName);
                if (res != 0)
                    return res;
                res = String.Compare(u1.LastName, u2.LastName);
                if (res != 0)
                    return res;
                res = String.Compare(u1.FirstName, u2.FirstName);
                return res;
            }
        }

        //nested class for descending sort by class, last name, first name
        private class sortSchoolClassLNameFNameDescendingHelper : IComparer
        {
            int IComparer.Compare(object a, object b)
            {
                User u1 = (User)a;
                User u2 = (User)b;
                int res = String.Compare(u2.SchoolName, u1.SchoolName);
                if (res != 0)
                    return res;
                res = String.Compare(u2.ClassName, u1.ClassName);
                if (res != 0)
                    return res;
                res = String.Compare(u2.LastName, u1.LastName);
                if (res != 0)
                    return res;
                res = String.Compare(u2.FirstName, u1.FirstName);
                return res;
            }
        }
        private string id;
        public string Id
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged("Id");
            }
        }
        private string firstName;
        public string FirstName
        {
            get { return firstName; } 
            set
            {
                firstName = value;
                OnPropertyChanged("FirstName");
            }
        }
        private string lastName;
        public string LastName 
        {
            get { return lastName; } 
            set
            {
                lastName = value;
                OnPropertyChanged("LastName");
            }
        }
        private string className;
        public string ClassName
        { 
            get { return className; }
            set
            {
                className = value;
                OnPropertyChanged("ClassName");
            }
        }
        private string schoolName;
        public string SchoolName
        {
            get { return schoolName; }
            set
            {
                schoolName = value;
                OnPropertyChanged("SchoolName");
            }
        }
        private string userType;
        public string UserType
        {
            get { return userType; }
            set 
            {
                userType = value;
                OnPropertyChanged("UserType");
            } 
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public int CompareTo(object obj)
        {
            User u = (User)obj;
            int res = String.Compare(this.LastName, u.LastName);
            if (res != 0)
                return res;
            res = String.Compare(this.FirstName, u.FirstName);
            if (res != 0)
                return res;
            res = String.Compare(this.ClassName, u.ClassName);
            if (res != 0)
                return res;
            res = String.Compare(this.SchoolName, u.SchoolName);
            return res;
        }

        // Method to return IComparer object for sort helper.
        public static IComparer sortLNameFNameClassSchoolDescending()
        {
            return (IComparer)new sortLNameFNameClassSchoolDescendingHelper();
        }
        // Method to return IComparer object for sort helper.
        public static IComparer sortSchoolClassLNameFNameAscending()
        {
            return (IComparer)new sortSchoolClassLNameFNameAscendingHelper();
        }
        // Method to return IComparer object for sort helper.
        public static IComparer sortSchoolClassLNameFNameDescending()
        {
            return (IComparer)new sortSchoolClassLNameFNameDescendingHelper();
        }
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
