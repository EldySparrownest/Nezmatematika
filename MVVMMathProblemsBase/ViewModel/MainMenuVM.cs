using MVVMMathProblemsBase.Model;
using MVVMMathProblemsBase.ViewModel.Commands;
using MVVMMathProblemsBase.ViewModel.ValueConverters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;

namespace MVVMMathProblemsBase.ViewModel
{
    public class MainMenuVM : INotifyPropertyChanged
    {
        public const string UserSettingsFilename = "Settings.xml";
        public string _MySettingsDirectoryPath = System.IO.Path.Combine(Environment.CurrentDirectory, "Settings");
        //_UserListPath bude obsahovat seznam uživatelů
        //na indexu [0] bude vždy poslední použitý student (pokud existuje)
        //na indexu [Count-1] bude vždy poslední použitý učitel (pokud existuje)
        public string _UserListPath = System.IO.Path.Combine(Environment.CurrentDirectory, "Settings", "UserList.xml");
        public string _DefaultSettingsPath = System.IO.Path.Combine(Environment.CurrentDirectory, "Settings", $"Default{UserSettingsFilename}");
        public string _UserSettingsPath = System.IO.Path.Combine(Environment.CurrentDirectory, "Settings", $"Default{UserSettingsFilename}");

        private MySettings settings;
        public MySettings Settings
        {
            get { return settings; }
            private set
            {
                settings = value;
                OnPropertyChanged("Settings");
            }
        }

        private ObservableCollection<User> allUsersList;
        public ObservableCollection<User> AllUsersList
        {
            get { return allUsersList; }
            set
            {
                allUsersList = value;
                OnPropertyChanged("AllUsersList");
            }
        }

        private ObservableCollection<User> usersOfTypeList;
        public ObservableCollection<User> UsersOfTypeList
        {
            get { return usersOfTypeList; }
            private set
            {
                usersOfTypeList = value;
                OnPropertyChanged("UsersOfTypeList");
            }
        }

        private User lastStudentUser;

        public User LastStudentUser
        {
            get { return lastStudentUser; }
            set 
            { 
                lastStudentUser = value;
                OnPropertyChanged("LastStudentUser");
            }
        }
        private User lastTeacherUser;

        public User LastTeacherUser
        {
            get { return lastTeacherUser; }
            set 
            { 
                lastTeacherUser = value;
                OnPropertyChanged("LastTeacherUser");
            }
        }


        private User currentUser;
        public User CurrentUser
        {
            get { return currentUser; }
            set
            {
                currentUser = value;

                if (value != null)
                {
                    if (IsInStudentMode == true && CurrentUser.UserType == "student")
                    {
                        LastStudentUser = CurrentUser;
                    }
                    else if (IsInStudentMode == false && CurrentUser.UserType == "teacher")
                    {
                        LastTeacherUser = CurrentUser;
                    }
                    ArchiveLastUsedUsers();

                    _UserSettingsPath = System.IO.Path.Combine(Environment.CurrentDirectory, "Settings", $"{CurrentUser.Id}{UserSettingsFilename}");
                }
                else
                {
                    _UserSettingsPath = _DefaultSettingsPath;
                }
                LoadSettingsForUser(CurrentUser);
                OnPropertyChanged("CurrentUser");
            }
        }
        private string tempFirstName;
        public string TempFirstName
        {
            get
            {
                return tempFirstName;
            }
            set
            {
                tempFirstName = value;
                OnPropertyChanged("TempFirstName");
            }
        }
        private string tempLastName;
        public string TempLastName
        {
            get
            {
                return tempLastName;
            }
            set
            {
                tempLastName = value;
                OnPropertyChanged("TempLastName");
            }
        }
        private string tempClassName;
        public string TempClassName
        {
            get
            {
                return tempClassName;
            }
            set
            {
                tempClassName = value;
                OnPropertyChanged("TempClassName");
            }
        }

        private string tempSchoolName;

        public string TempSchoolName
        {
            get { return tempSchoolName; }
            set
            {
                tempSchoolName = value;
                OnPropertyChanged("TempSchoolName");
            }
        }

        private bool? isInStudentMode;
        public bool? IsInStudentMode
        {
            get { return isInStudentMode; }
            set
            {
                isInStudentMode = value;
                if (isInStudentMode == true)
                {
                    StudentVis = Visibility.Visible;
                    TeacherVis = Visibility.Collapsed;
                }
                else
                {
                    StudentVis = Visibility.Collapsed;
                    TeacherVis = Visibility.Visible;
                }
                OnPropertyChanged("IsInStudentMode");
            }
        }

        private Visibility studentVis;
        public Visibility StudentVis
        {
            get { return studentVis; }
            set
            {
                studentVis = value;
                OnPropertyChanged("StudentVis");
            }
        }

        private Visibility teacherVis;
        public Visibility TeacherVis
        {
            get { return teacherVis; }
            set
            {
                teacherVis = value;
                OnPropertyChanged("TeacherVis");
            }
        }

        private Visibility newUserVis;
        public Visibility NewUserVis
        {
            get { return newUserVis; }
            set
            {
                newUserVis = value;
                OnPropertyChanged("NewUserVis");
            }
        }
        private Visibility editUserVis;
        public Visibility EditUserVis
        {
            get { return editUserVis; }
            set
            {
                editUserVis = value;
                OnPropertyChanged("EditUserVis");
            }
        }

        private Visibility userSelVis;
        public Visibility UserSelVis
        {
            get { return userSelVis; }
            set
            {
                userSelVis = value;
                OnPropertyChanged("UserSelVis");
            }
        }
        private Visibility settingsVis;
        public Visibility SettingsVis
        {
            get { return settingsVis; }
            set
            {
                settingsVis = value;
                OnPropertyChanged("SettingsVis");
            }
        }

        private Brush selectedColour;
        public Brush SelectedColour
        {
            get { return selectedColour; }
            set
            {
                selectedColour = value;
                OnPropertyChanged("SelectedColour");
            }
        }

        public CreateNewUserCommand CreateNewUserCommand { get; set; }
        public DeleteUserCommand DeleteUserCommand { get; set; }
        public DisplayProfileSelectionCommand DisplayProfileSelectionCommand { get; set; }
        public PrepUserForEditingCommand PrepUserForEditingCommand { get; set; }
        public EditUserCommand EditUserCommand { get; set; }
        public DisplaySettingsCommand DisplaySettingsCommand { get; set; }
        public RestoreDefaultSettingsCommand RestoreDefaultSettingsCommand { get; set; }
        public BackToMainMenuWithoutSavingSettingsCommand BackToMainMenuWithoutSavingSettingsCommand { get; set; }
        public ApplyNewSettingsCommand ApplyNewSettingsCommand { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;


        public MainMenuVM()
        {
            IsInStudentMode = App.IsInStudentMode;
            LoadDefaultSettings();
            AllUsersList = new ObservableCollection<User>();
            GetListOfAllUsers();
            SetLastUsedUserFromAllUsers();
            SetCurrentUserToLastUsedUser();
            LoadSettingsForUser(CurrentUser);
            EditUserVis = Visibility.Collapsed;
            UserSelVis = Visibility.Collapsed;
            SettingsVis = Visibility.Collapsed;
            if (IsInStudentMode == true)
            {
                StudentVis = Visibility.Visible;
                TeacherVis = Visibility.Collapsed;
            }
            else
            {
                StudentVis = Visibility.Collapsed;
                TeacherVis = Visibility.Visible;
            }

            if (CurrentUser == null)
            {
                NewUserVis = Visibility.Visible;
            }
            else
            {
                NewUserVis = Visibility.Collapsed;
            }
            //IsInStudentMode = null;
            //CurrentUser = App.AppUser; //na začátku bude null
            //StudentVis = Visibility.Collapsed;
            //TeacherVis = Visibility.Collapsed;
            //bool isInDesignMode = DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject());
            //if (isInDesignMode)
            //{
            //    StudentVis = Visibility.Visible;
            //}
            UsersOfTypeList = new ObservableCollection<User>();
            GetUsersOfTypeList();

            CreateNewUserCommand = new CreateNewUserCommand(this);
            DeleteUserCommand = new DeleteUserCommand(this);
            DisplayProfileSelectionCommand = new DisplayProfileSelectionCommand(this);
            PrepUserForEditingCommand = new PrepUserForEditingCommand(this);
            EditUserCommand = new EditUserCommand(this);
            DisplaySettingsCommand = new DisplaySettingsCommand(this);
            RestoreDefaultSettingsCommand = new RestoreDefaultSettingsCommand(this);
            BackToMainMenuWithoutSavingSettingsCommand = new BackToMainMenuWithoutSavingSettingsCommand(this);
            ApplyNewSettingsCommand = new ApplyNewSettingsCommand(this);
        }

        //public MainMenuVM(bool inStudentMode) // NOT USED AND NOT UPDATED
        //{
        //    IsInStudentMode = inStudentMode;
        //    CurrentUser = App.AppUser; //na začátku bude null
        //    UserSelVis = Visibility.Collapsed;
        //    if (inStudentMode)
        //    {
        //        StudentVis = Visibility.Visible;
        //        TeacherVis = Visibility.Collapsed;
        //    }
        //    else
        //    {
        //        StudentVis = Visibility.Collapsed;
        //        TeacherVis = Visibility.Visible;
        //    }

        //    if (CurrentUser == null)
        //    {
        //        NewUserVis = Visibility.Visible;
        //    }
        //    else
        //    {
        //        NewUserVis = Visibility.Collapsed;
        //    }
        //}

        public void LoadDefaultSettings()
        {
            if (File.Exists(_DefaultSettingsPath))
            {
                this.Settings = new MySettings().Read(_DefaultSettingsPath);
            }
        }
        public void SetLastUsedUserFromAllUsers()
        {
            if (AllUsersList.Count >= 1)
            {
                if (this.IsInStudentMode == true)
                {
                    if (AllUsersList[0].UserType == "student")
                    {
                        LastStudentUser = AllUsersList[0];
                    }
                    else
                    {
                        LastStudentUser = null;
                    }
                }
                else
                {
                    if (AllUsersList[AllUsersList.Count - 1].UserType == "teacher")
                    {
                        LastTeacherUser = AllUsersList[AllUsersList.Count - 1];
                    }
                    else
                    {
                        LastTeacherUser = null;
                    }
                }
            }
            else
            {
                LastStudentUser = null;
                LastTeacherUser = null;
            }
            SaveUserList();
        }
        public void SetCurrentUserToLastUsedUser()
        {
            if (IsInStudentMode == true)
            {
                CurrentUser = LastStudentUser;
            }
            else
            {
                CurrentUser = LastTeacherUser;
            }
        }
        public void LoadSettingsForUser(User user)
        {
            if (user != null)
            {
                string settingsPath = System.IO.Path.Combine(Environment.CurrentDirectory, "Settings", $"{user.Id}{UserSettingsFilename}");
                if (File.Exists(settingsPath))
                {
                    this.Settings = new MySettings().Read(settingsPath);
                }
            }
            else
            {
                if (File.Exists(_DefaultSettingsPath))
                {
                    this.Settings = new MySettings().Read(_DefaultSettingsPath);
                }
            }
        }
        public void SaveUserSettings()
        {
            Settings.Save(_UserSettingsPath);
        }
        public void RestoreDefaultSettingsForCurrentUser()
        {
            MySettings usersNewSettings = new MySettings().Read(_DefaultSettingsPath);
            usersNewSettings.HasCourseToContinue = this.Settings.HasCourseToContinue;
            usersNewSettings.Save(_UserSettingsPath);
            LoadSettingsForUser(CurrentUser);
        }
        public void RestoreUserSettings()
        {
            LoadSettingsForUser(CurrentUser);
        }
        public void ArchiveLastUsedUsers()
        {
            if (CurrentUser != null && AllUsersList.Count >= 1)
            {
                int index;
                if (IsInStudentMode == true)
                    index = 0;
                else
                    index = AllUsersList.Count - 1;
                for (int i = 0; i < AllUsersList.Count; i++)
                {
                    if (AllUsersList[i] == CurrentUser)
                    {
                        AllUsersList.RemoveAt(i);
                        AllUsersList.Insert(index, CurrentUser);
                        SaveUserList();
                        return;
                    }
                }
            }
        }
        public void SaveUserList()
        {
            using (StreamWriter sw = new StreamWriter(_UserListPath))
            {
                XmlSerializer xmls = new XmlSerializer(typeof(List<User>));
                xmls.Serialize(sw, new List<User>(this.AllUsersList));
            }
        }
        public void ClearTempValues()
        {
            TempFirstName = string.Empty;
            TempLastName = string.Empty;
            TempSchoolName = string.Empty;
            TempClassName = string.Empty;
        }

        public void CreateNewUser(string fName, string lName, string sName, string cName)
        {
            CurrentUser = new User()
            {
                Id = NullBoolToModeNameStringCovnerter.Convert(IsInStudentMode)
                    + string.Join(string.Empty, (Convert.ToString(DateTime.Now)).Split(" .:".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)),
                FirstName = fName,
                LastName = lName,
                SchoolName = sName,
                ClassName = cName,
                UserType = NullBoolToModeNameStringCovnerter.Convert(IsInStudentMode)
            };
            this.Settings.ThisUser = CurrentUser;
            SaveUserSettings();
            AllUsersList.Add(CurrentUser);
            SaveUserList();
            UsersOfTypeList.Add(CurrentUser);
            ClearTempValues();
        }

        public void GetListOfAllUsers()
        {
            AllUsersList.Clear();
            List<User> resultList = new List<User>();
            if (File.Exists(_UserListPath))
            {
                using (StreamReader sw = new StreamReader(_UserListPath))
                {
                    XmlSerializer xmls = new XmlSerializer(typeof(List<User>));
                    resultList = xmls.Deserialize(sw) as List<User>;
                }
            }
            AllUsersList = new ObservableCollection<User>(resultList);
            //resultList.OrderBy(User)
        }

        public void GetUsersOfTypeList()
        {
            UsersOfTypeList.Clear();
            ObservableCollection<User> resultList = new ObservableCollection<User>();
            string modeName = NullBoolToModeNameStringCovnerter.Convert(IsInStudentMode);
            foreach (User user in AllUsersList)
            {
                if (user.UserType == modeName)
                {
                    resultList.Add(user);
                }
            }
            UsersOfTypeList = resultList;
        }

        public void DeleteUser(User userToDelete)
        {
            string filePath = System.IO.Path.Combine(Environment.CurrentDirectory, "Settings", $"{userToDelete.Id}{UserSettingsFilename}");
            File.Delete(filePath);
            UsersOfTypeList.Remove(userToDelete);
            AllUsersList.Remove(userToDelete);
            SaveUserList();
            CurrentUser = UsersOfTypeList[0];
        }
        public void EditUser(string fName, string lName, string sName, string cName)
        {
            CurrentUser.FirstName = fName;
            CurrentUser.LastName = lName;
            CurrentUser.SchoolName = sName;
            CurrentUser.ClassName = cName;
            SaveUserSettings();
            //AllUsersList.Add(CurrentUser);
            SaveUserList();
            //UsersOfTypeList.Add(CurrentUser);
            ClearTempValues();
        }
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
