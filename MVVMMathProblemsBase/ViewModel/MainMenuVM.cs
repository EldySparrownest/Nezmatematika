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
using System.Windows.Documents;
using System.Windows.Media;
using System.Xml.Serialization;

namespace MVVMMathProblemsBase.ViewModel
{
    public class MainMenuVM : INotifyPropertyChanged
    {
        public const string CourseFilename = "Course.xml";
        public const string UserSettingsFilename = "Settings.xml";
        public string _MySettingsDirectoryPath = System.IO.Path.Combine(Environment.CurrentDirectory, "Settings");
        //_UserListPath bude obsahovat seznam uživatelů
        //na indexu [0] bude vždy poslední použitý student (pokud existuje)
        //na indexu [Count-1] bude vždy poslední použitý učitel (pokud existuje)
        public string _UserListPath = System.IO.Path.Combine(Environment.CurrentDirectory, "Settings", "UserList.xml");
        public string _DefaultSettingsPath = System.IO.Path.Combine(Environment.CurrentDirectory, "Settings", $"Default{UserSettingsFilename}");
        public string _UserSettingsPath = System.IO.Path.Combine(Environment.CurrentDirectory, "Settings", $"Default{UserSettingsFilename}");
        public string _UserCoursesDirPath() => System.IO.Path.Combine(Environment.CurrentDirectory, "Courses", CurrentUser.Id);

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

        private ObservableCollection<Course> coursesToContinueList;
        public ObservableCollection<Course> CoursesToContinueList
        {
            get { return coursesToContinueList; }
            private set
            {
                coursesToContinueList = value;
                OnPropertyChanged("CoursesToContinueList");
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
        private Course currentCourse;

        public Course CurrentCourse
        {
            get { return currentCourse; }
            set
            {
                currentCourse = value;
                OnPropertyChanged("CurrentCourse");
            }
        }

        private MathProblem currentMathProblem;

        public MathProblem CurrentMathProblem
        {
            get { return currentMathProblem; }
            set
            {
                if (value != null)
                {
                    CurrentMathProblemAboutToChangeToNotNull?.Invoke(this, new EventArgs());
                }
                currentMathProblem = value;
                OnPropertyChanged("CurrentMathProblem");
                CurrentMathProblemChanged?.Invoke(this, new EventArgs());
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
        private string tempCourseTitle;

        public string TempCourseTitle
        {
            get { return tempCourseTitle; }
            set
            {
                tempCourseTitle = value;
                OnPropertyChanged("TempCourseTitle");
            }
        }
        private string tempCourseDesc;

        public string TempCourseDesc
        {
            get { return tempCourseDesc; }
            set
            {
                tempCourseDesc = value;
                OnPropertyChanged("TempCourseDesc");
            }
        }
        private ObservableCollection<string> tempCourseTags;

        public ObservableCollection<string> TempCourseTags
        {
            get { return tempCourseTags; }
            set
            {
                tempCourseTags = value;
                OnPropertyChanged("TempCourseTags");
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
        private Visibility newCourseVis;
        public Visibility NewCourseVis
        {
            get { return newCourseVis; }
            set
            {
                newCourseVis = value;
                OnPropertyChanged("NewCourseVis");
            }
        }
        private Visibility editCourseVis;
        public Visibility EditCourseVis
        {
            get { return editCourseVis; }
            set
            {
                editCourseVis = value;
                OnPropertyChanged("EditCourseVis");
            }
        }

        private Visibility coursesToContinueVis;
        public Visibility CoursesToContinueVis
        {
            get { return coursesToContinueVis; }
            set
            {
                coursesToContinueVis = value;
                OnPropertyChanged("CoursesToContinueVis");
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

        private Color selectedTextColour;
        public Color SelectedTextColour
        {
            get { return selectedTextColour; }
            set
            {
                selectedTextColour = value;
                OnPropertyChanged("SelectedTextColour");
            }
        }

        public AddNewProblemCommand AddNewProblemCommand { get; set; }
        public ApplyNewSettingsCommand ApplyNewSettingsCommand { get; set; }
        public BackToMainMenuFromAnywhereCommand BackToMainMenuFromAnywhereCommand { get; set; }
        public BackToMainMenuWithoutSavingSettingsCommand BackToMainMenuWithoutSavingSettingsCommand { get; set; }
        public CreateNewCourseCommand CreateNewCourseCommand { get; set; }
        public CreateNewUserCommand CreateNewUserCommand { get; set; }
        public DeleteUserCommand DeleteUserCommand { get; set; }
        public DisplayCourseToEditSelectionCommand DisplayCourseToEditSelectionCommand { get; set; }
        public DisplayNewCourseCreationCommand DisplayNewCourseCreationCommand { get; set; }
        public DisplayProfileSelectionCommand DisplayProfileSelectionCommand { get; set; }
        public DisplaySettingsCommand DisplaySettingsCommand { get; set; }
        public EditCourseCommand EditCourseCommand { get; set; }
        public EditUserCommand EditUserCommand { get; set; }
        public PrepUserForEditingCommand PrepUserForEditingCommand { get; set; }
        public RestoreDefaultSettingsCommand RestoreDefaultSettingsCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler CurrentMathProblemAboutToChangeToNotNull;
        public event EventHandler CurrentMathProblemChanged;
        public MainMenuVM()
        {
            IsInStudentMode = App.IsInStudentMode;
            LoadDefaultSettings();
            AllUsersList = new ObservableCollection<User>();
            TempCourseTags = new ObservableCollection<string>();
            GetListOfAllUsers();
            SetLastUsedUserFromAllUsers();
            SetCurrentUserToLastUsedUser();
            LoadSettingsForUser(CurrentUser);
            EditUserVis = Visibility.Collapsed;
            UserSelVis = Visibility.Collapsed;
            NewCourseVis = Visibility.Collapsed;
            EditCourseVis = Visibility.Collapsed;
            CoursesToContinueVis = Visibility.Collapsed;
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
            UsersOfTypeList = new ObservableCollection<User>();
            GetUsersOfTypeList();
            
            CoursesToContinueList = new ObservableCollection<Course>();

            SelectedTextColour = Colors.Black;

            AddNewProblemCommand = new AddNewProblemCommand(this);
            ApplyNewSettingsCommand = new ApplyNewSettingsCommand(this);
            BackToMainMenuFromAnywhereCommand = new BackToMainMenuFromAnywhereCommand(this);
            BackToMainMenuWithoutSavingSettingsCommand = new BackToMainMenuWithoutSavingSettingsCommand(this);
            CreateNewCourseCommand = new CreateNewCourseCommand(this);
            CreateNewUserCommand = new CreateNewUserCommand(this);
            DeleteUserCommand = new DeleteUserCommand(this);
            DisplayCourseToEditSelectionCommand = new DisplayCourseToEditSelectionCommand(this);
            DisplayNewCourseCreationCommand = new DisplayNewCourseCreationCommand(this);
            DisplayProfileSelectionCommand = new DisplayProfileSelectionCommand(this);
            DisplaySettingsCommand = new DisplaySettingsCommand(this);
            EditCourseCommand = new EditCourseCommand(this);
            EditUserCommand = new EditUserCommand(this);
            PrepUserForEditingCommand = new PrepUserForEditingCommand(this);
            RestoreDefaultSettingsCommand = new RestoreDefaultSettingsCommand(this);
        }

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
            var usersNewSettings = new MySettings().Read(_DefaultSettingsPath);
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
        public void ClearUserTempValues()
        {
            TempFirstName = string.Empty;
            TempLastName = string.Empty;
            TempSchoolName = string.Empty;
            TempClassName = string.Empty;
        }
        
        public void ClearCourseTempValues()
        {
            TempCourseTitle = string.Empty;
            TempCourseDesc = string.Empty;
            TempCourseTags.Clear();
        }

        public void ClearCurrentValuesExceptUser()
        {
            CurrentMathProblem = null;
            CurrentCourse = null;
        }

        public void BackToMainMenu()
        {
            ClearUserTempValues();
            ClearCourseTempValues();
            EditCourseVis = Visibility.Collapsed;
            EditUserVis = Visibility.Collapsed;
            NewCourseVis = Visibility.Collapsed;
            NewUserVis = Visibility.Collapsed;
            SettingsVis = Visibility.Collapsed;
            UserSelVis = Visibility.Collapsed;
            CoursesToContinueVis = Visibility.Collapsed;

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
        }

        public void CreateNewUser(string fName, string lName, string sName, string cName)
        {
            CurrentUser = new User()
            {
                Id = NullBoolToModeNameStringCovnerter.Convert(IsInStudentMode)
                    + string.Join(string.Empty, (DateTime.Now.ToString("yyyyMMddHHmmssffffff")).Split(" .:".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)),
                FirstName = fName,
                LastName = lName,
                SchoolName = sName,
                ClassName = cName,
                UserType = NullBoolToModeNameStringCovnerter.Convert(IsInStudentMode)
            };
            this.Settings.ThisUser = CurrentUser;
            this.Settings.HasCourseToContinue = false;
            RestoreDefaultSettingsForCurrentUser();
            SaveUserSettings();
            AllUsersList.Add(CurrentUser);
            SaveUserList();
            UsersOfTypeList.Add(CurrentUser);
            ClearUserTempValues();
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
        }

        public void GetUsersOfTypeList()
        {
            UsersOfTypeList.Clear();
            var resultList = new ObservableCollection<User>();
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
            SaveUserList();
            ClearUserTempValues();
        }
        public void CreateNewCourse()
        {
            CurrentCourse = new Course(CurrentUser, TempCourseTitle, TempCourseDesc, TempCourseTags);
            CurrentCourse.AddNewMathProblem(new MathProblem());
            CurrentMathProblem = CurrentCourse.Problems[0];

            Directory.CreateDirectory(CurrentCourse.DirPath);
            CoursesToContinueList.Add(CurrentCourse);
            UpdateAbilityToContinueCourse();
        }

        public void StartEditingCurrentCourse()
        {
            CurrentCourse.LastOpened = DateTime.Now;
            CurrentMathProblem = CurrentCourse.Problems[0];
            EditCourseVis = Visibility.Visible;
        }

        public void SaveCurrentCourse()
        {
            CurrentCourse.Save(CurrentCourse.FilePath);
        }

        public void UpdateAbilityToContinueCourse()
        {
            var oldValue = Settings.HasCourseToContinue;
            Settings.HasCourseToContinue = CoursesToContinueList.Count > 0;
            if (oldValue != Settings.HasCourseToContinue)
            {
                SaveUserSettings();
            }
        }
        public void GetListOfTeacherCoursesToContinue()
        {
            CoursesToContinueList.Clear();
            List<Course> resultList = new List<Course>();
            if (Directory.Exists(_UserCoursesDirPath()))
            {
                var files = Directory.EnumerateFiles(_UserCoursesDirPath());

                foreach (var file in files)
                {
                    resultList.Add(Course.Read(file));
                }
            }
            resultList.OrderBy(c => c.LastEdited);
            CoursesToContinueList = new ObservableCollection<Course>(resultList);
        }

        public void SaveMathProblem(MathProblem mathProblem, TextRange textRange)
        {
            mathProblem.FileLocation(CurrentCourse.DirPath);

            FileStream fileStream = new FileStream(mathProblem.FilePath, FileMode.Create);
            textRange.Save(fileStream, DataFormats.Rtf);
            fileStream.Close();
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
