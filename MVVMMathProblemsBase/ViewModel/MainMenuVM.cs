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
        public string _CoursesDirPath() => System.IO.Path.Combine(Environment.CurrentDirectory, "Courses");
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

        private ObservableCollection<(Course, UserCourseData)> studentCoursesCompleted;
        public ObservableCollection<(Course, UserCourseData)> StudentCoursesCompleted
        {
            get { return studentCoursesCompleted; }
            private set
            {
                studentCoursesCompleted = value;
                OnPropertyChanged("StudentCoursesCompleted");
            }
        }

        private ObservableCollection<(Course, UserCourseData)> studentCoursesInProgress;
        public ObservableCollection<(Course, UserCourseData)> StudentCoursesInProgress
        {
            get { return studentCoursesInProgress; }
            private set
            {
                studentCoursesInProgress = value;
                OnPropertyChanged("StudentCoursesInProgress");
            }
        }

        private ObservableCollection<Course> teacherCoursesToContinueList;
        public ObservableCollection<Course> TeacherCoursesToContinueList
        {
            get { return teacherCoursesToContinueList; }
            private set
            {
                teacherCoursesToContinueList = value;
                OnPropertyChanged("TeacherCoursesToContinueList");
            }
        }

        private ObservableCollection<Course> newCoursesToStartList;
        public ObservableCollection<Course> NewCoursesToStartList
        {
            get { return newCoursesToStartList; }
            set 
            { 
                newCoursesToStartList = value;
                OnPropertyChanged("NewCoursesToStartList");
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

                    if (EditUserVis == Visibility.Visible)
                    {
                        PopulateUserTempValues(CurrentUser);
                    }
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
                if (currentMathProblem != null || currentMathProblem != value)
                {
                    TeacherNotNullCurrentMathProblemAboutToChange?.Invoke(this, new EventArgs());
                }
                currentMathProblem = value;
                PopulateTempAnswersFromCurrentMathProblem();
                OnPropertyChanged("CurrentMathProblem");
                StudentCurrentMathProblemChanged?.Invoke(this, new EventArgs());
                TeacherCurrentMathProblemChanged?.Invoke(this, new EventArgs());
            }
        }

        private string currentAnswer;

        public string CurrentAnswer
        {
            get { return currentAnswer; }
            set
            {
                currentAnswer = value;
                TempCorrectAnswer = CurrentAnswer;
                OnPropertyChanged("CurrentAnswer");
            }
        }

        private UserCourseData currentUserCourseData;

        public UserCourseData CurrentUserCourseData
        {
            get { return currentUserCourseData; }
            set 
            { 
                currentUserCourseData = value;
                OnPropertyChanged("currentUserCourseData");
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

        private string tempCorrectAnswer;

        public string TempCorrectAnswer
        {
            get { return tempCorrectAnswer; }
            set
            {
                tempCorrectAnswer = value;
                OnPropertyChanged("TempCorrectAnswer");
            }
        }

        private ObservableCollection<string> tempAnswers;
        public ObservableCollection<string> TempAnswers
        {
            get { return tempAnswers; }
            set 
            {
                tempAnswers = value;
                OnPropertyChanged("TempAnswers");
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

        private Visibility courseEditorMathProblemUserModeVis;
        public Visibility CourseEditorMathProblemUserModeVis
        {
            get { return courseEditorMathProblemUserModeVis; }
            set
            {
                courseEditorMathProblemUserModeVis = value;
                OnPropertyChanged("CourseEditorMathProblemUserModeVis");
            }
        }

        private Visibility courseEditorMathProblemCodeModeVis;
        public Visibility CourseEditorMathProblemCodeModeVis
        {
            get { return courseEditorMathProblemCodeModeVis; }
            set
            {
                courseEditorMathProblemCodeModeVis = value;
                OnPropertyChanged("CourseEditorMathProblemCodeModeVis");
            }
        }

        private Visibility studentNewCourseSelVis;
        public Visibility StudentNewCourseSelVis
        {
            get { return studentNewCourseSelVis; }
            set 
            { 
                studentNewCourseSelVis = value;
                OnPropertyChanged("StudentNewCourseSelVis");
            }
        }

        private Visibility courseForStudentVis;
        public Visibility CourseForStudentVis
        {
            get { return courseForStudentVis; }
            set 
            { 
                courseForStudentVis = value;
                OnPropertyChanged("CourseForStudentVis");
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

        private Visibility studentSettingsVis;
        public Visibility StudentSettingsVis
        {
            get { return studentSettingsVis; }
            set
            {
                studentSettingsVis = value;
                OnPropertyChanged("StudentSettingsVis");
            }
        }

        private Visibility teacherSettingsVis;
        public Visibility TeacherSettingsVis
        {
            get { return teacherSettingsVis; }
            set
            {
                teacherSettingsVis = value;
                OnPropertyChanged("TeacherSettingsVis");
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

        public AddNewCorrectAnswerCommand AddNewCorrectAnswerCommand { get; set; }
        public AddNewProblemCommand AddNewProblemCommand { get; set; }
        public ApplyNewSettingsCommand ApplyNewSettingsCommand { get; set; }
        public BackToMainMenuFromAnywhereCommand BackToMainMenuFromAnywhereCommand { get; set; }
        public BackToMainMenuWithoutSavingSettingsCommand BackToMainMenuWithoutSavingSettingsCommand { get; set; }
        public CreateNewCourseCommand CreateNewCourseCommand { get; set; }
        public CreateNewUserCommand CreateNewUserCommand { get; set; }
        public DeleteCourseCommand DeleteCourseCommand { get; set; }
        public DeleteUserCommand DeleteUserCommand { get; set; }
        public DisplayCourseToEditSelectionCommand DisplayCourseToEditSelectionCommand { get; set; }
        public DisplayNewCourseCreationCommand DisplayNewCourseCreationCommand { get; set; }
        public DisplayNewCourseSelectionCommand DisplayNewCourseSelectionCommand { get; set; }
        public DisplayProfileSelectionCommand DisplayProfileSelectionCommand { get; set; }
        public DisplaySettingsCommand DisplaySettingsCommand { get; set; }
        public EditCorrectAnswerCommand EditCorrectAnswerCommand { get; set; }
        public EditCourseCommand EditCourseCommand { get; set; }
        public EditUserCommand EditUserCommand { get; set; }
        public OpenCourseForStudentCommand OpenCourseForStudentCommand { get; set; }
        public PrepUserForEditingCommand PrepUserForEditingCommand { get; set; }
        public PublishCourseCommand PublishCourseCommand { get; set; }
        public RemoveCorrectAnswerCommand RemoveCorrectAnswerCommand { get; set; }
        public RestoreDefaultSettingsCommand RestoreDefaultSettingsCommand { get; set; }
        public SwitchBetweenUserAndCodeModeCommand SwitchBetweenUserAndCodeModeCommand { get; set; }
        public SwitchToNextProblemCommand SwitchToNextProblemCommand { get; set; }
        public SwitchToPreviousProblemCommand SwitchToPreviousProblemCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler StudentCurrentMathProblemChanged;
        public event EventHandler TeacherNotNullCurrentMathProblemAboutToChange;
        public event EventHandler TeacherCurrentMathProblemChanged;
        public MainMenuVM()
        {
            IsInStudentMode = App.IsInStudentMode;
            LoadDefaultSettings();
            AllUsersList = new ObservableCollection<User>();
            GetListOfAllUsers();
            SetLastUsedUserFromAllUsers();
            SetCurrentUserToLastUsedUser();
            LoadSettingsForUser(CurrentUser);
            TempAnswers = new ObservableCollection<string>();
            TempCourseTags = new ObservableCollection<string>();
            EditUserVis = Visibility.Collapsed;
            UserSelVis = Visibility.Collapsed;
            NewCourseVis = Visibility.Collapsed;
            EditCourseVis = Visibility.Collapsed;
            CourseEditorMathProblemUserModeVis = Visibility.Collapsed;
            CourseEditorMathProblemCodeModeVis = Visibility.Collapsed;
            CoursesToContinueVis = Visibility.Collapsed;
            StudentNewCourseSelVis = Visibility.Collapsed;
            CourseForStudentVis = Visibility.Collapsed;
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

            NewCoursesToStartList = new ObservableCollection<Course>();
            TeacherCoursesToContinueList = new ObservableCollection<Course>();

            SelectedTextColour = Colors.Black;

            AddNewCorrectAnswerCommand = new AddNewCorrectAnswerCommand(this);
            AddNewProblemCommand = new AddNewProblemCommand(this);
            ApplyNewSettingsCommand = new ApplyNewSettingsCommand(this);
            BackToMainMenuFromAnywhereCommand = new BackToMainMenuFromAnywhereCommand(this);
            BackToMainMenuWithoutSavingSettingsCommand = new BackToMainMenuWithoutSavingSettingsCommand(this);
            CreateNewCourseCommand = new CreateNewCourseCommand(this);
            CreateNewUserCommand = new CreateNewUserCommand(this);
            DeleteCourseCommand = new DeleteCourseCommand(this);
            DeleteUserCommand = new DeleteUserCommand(this);
            DisplayCourseToEditSelectionCommand = new DisplayCourseToEditSelectionCommand(this);
            DisplayNewCourseCreationCommand = new DisplayNewCourseCreationCommand(this);
            DisplayNewCourseSelectionCommand = new DisplayNewCourseSelectionCommand(this);
            DisplayProfileSelectionCommand = new DisplayProfileSelectionCommand(this);
            DisplaySettingsCommand = new DisplaySettingsCommand(this);
            EditCorrectAnswerCommand = new EditCorrectAnswerCommand(this);
            EditCourseCommand = new EditCourseCommand(this);
            EditUserCommand = new EditUserCommand(this);
            OpenCourseForStudentCommand = new OpenCourseForStudentCommand(this);
            PrepUserForEditingCommand = new PrepUserForEditingCommand(this);
            PublishCourseCommand = new PublishCourseCommand(this);
            RemoveCorrectAnswerCommand = new RemoveCorrectAnswerCommand(this);
            RestoreDefaultSettingsCommand = new RestoreDefaultSettingsCommand(this);
            SwitchBetweenUserAndCodeModeCommand = new SwitchBetweenUserAndCodeModeCommand(this);
            SwitchToNextProblemCommand = new SwitchToNextProblemCommand(this);
            SwitchToPreviousProblemCommand = new SwitchToPreviousProblemCommand(this);
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
            var settingsPath = user != null ? Path.Combine(Environment.CurrentDirectory, "Settings", $"{user.Id}{UserSettingsFilename}") : _DefaultSettingsPath;
            if (File.Exists(settingsPath))
                this.Settings = new MySettings().Read(settingsPath);
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
            App.WhereInApp = WhereInApp.MainMenu;
            
            ClearUserTempValues();
            ClearCourseTempValues();

            //výběry údržby uživatelů
            NewUserVis = Visibility.Collapsed;
            EditUserVis = Visibility.Collapsed;
            UserSelVis = Visibility.Collapsed;

            //učitelské výběry a obrazovky kurzů
            NewCourseVis = Visibility.Collapsed;
            CoursesToContinueVis = Visibility.Collapsed;
            EditCourseVis = Visibility.Collapsed;
            CourseEditorMathProblemUserModeVis = Visibility.Collapsed;
            CourseEditorMathProblemCodeModeVis = Visibility.Collapsed;

            //studentské výběry a obrazovky kurzů
            StudentNewCourseSelVis = Visibility.Collapsed;
            CourseForStudentVis = Visibility.Collapsed;

            //výběry nastavení
            SettingsVis = Visibility.Collapsed;
            StudentSettingsVis = Visibility.Collapsed;
            TeacherSettingsVis = Visibility.Collapsed;

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
            var curUser = CurrentUser;
            UsersOfTypeList.Clear();
            var resultList = new List<User>();
            string modeName = NullBoolToModeNameStringCovnerter.Convert(IsInStudentMode);
            foreach (User user in AllUsersList)
            {
                if (user.UserType == modeName)
                {
                    resultList.Add(user);
                }
            }
            UsersOfTypeList = new ObservableCollection<User>(resultList.OrderBy(u => u.SchoolName)
                                                                        .ThenBy(u => u.ClassName)
                                                                        .ThenBy(u => u.LastName)
                                                                        .ThenBy(u => u.FirstName));
            CurrentUser = curUser;
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

        public void PopulateUserTempValues(User userToEdit)
        {
            TempFirstName = userToEdit.FirstName;
            TempLastName = userToEdit.LastName;
            TempSchoolName = userToEdit.SchoolName;
            TempClassName = userToEdit.ClassName;
        }

        public void PopulateTempAnswersFromCurrentMathProblem()
        {
            if (CurrentMathProblem != null && CurrentMathProblem.CorrectAnswers != null)
                TempAnswers = new ObservableCollection<string>(CurrentMathProblem.CorrectAnswers);
            else
                TempAnswers = new ObservableCollection<string>();
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
        public void OpenCurrentCourseForStudent()
        {
            App.WhereInApp = WhereInApp.CourseForStudent;
            var startTime = DateTime.Now;
            
            if (!CheckUserCouseDataExists())
                CreateUserCourseData(startTime);
            
            CurrentUserCourseData.LastSessionStarted = startTime;
            SaveUserList();

            UpdateAbilityToContinueCourse();

            CurrentMathProblem = CurrentCourse.Problems[0];
            CourseForStudentVis = Visibility.Visible;
        }
        private bool CheckUserCouseDataExists() => CurrentUser.CoursesData.Find(c => c.CourseId == CurrentCourse.Id) != null;
        private void CreateUserCourseData(DateTime startTime)
        {
            CurrentUserCourseData = new UserCourseData(CurrentCourse.Id, CurrentUser.Id, startTime);
            CurrentUser.CoursesData.Add(CurrentUserCourseData);
        }
        public void CreateNewCourse()
        {
            CurrentCourse = new Course(CurrentUser, TempCourseTitle, TempCourseDesc, TempCourseTags);
            CurrentCourse.AddNewMathProblem(new MathProblem());
            CurrentMathProblem = CurrentCourse.Problems[0];

            Directory.CreateDirectory(CurrentCourse.DirPath);
            TeacherCoursesToContinueList.Add(CurrentCourse);
            UpdateAbilityToContinueCourse();
        }

        public void StartEditingCurrentCourse()
        {
            App.WhereInApp = WhereInApp.CourseEditor;
            CurrentCourse.LastOpened = DateTime.Now;
            CurrentMathProblem = CurrentCourse.Problems[0];
            PopulateTempAnswersFromCurrentMathProblem();
            EditCourseVis = Visibility.Visible;
            CourseEditorMathProblemUserModeVis = Visibility.Visible;
        }

        public void ReloadCorrectAnswers()
        {
            CurrentMathProblem.CorrectAnswers.Clear();
            CurrentMathProblem.CorrectAnswers = new ObservableCollection<string>(TempAnswers);
        }

        public void ReloadTempAnswers()
        {
            TempAnswers.Clear();
            TempAnswers = new ObservableCollection<string>(CurrentMathProblem.CorrectAnswers);
        }

        public void ReplaceInTempAnswers(string oldAnswer, string newAnswer)
        {
            var replacingAt = TempAnswers.IndexOf(oldAnswer);
            if (replacingAt > -1)
                TempAnswers[replacingAt] = newAnswer;
        }

        public void PublishCurrentCourse()
        {
            var publish = true;
            int faultyProblemOrder = 0;

            for (int i = 0; i < CurrentCourse.Problems.Count; i++)
            {
                var problem = CurrentCourse.Problems[i];
                if (problem.ValidateMathProblem() == false)
                {
                    publish = false;
                    faultyProblemOrder = i + 1;
                    break;
                }
            }

            if (publish)
                CurrentCourse.Publish(_CoursesDirPath());
            else
            {
                MessageBox.Show("Aby kurz mohl být zveřejněn, musí mít každá úloha alespoň 1 správnou odpověď.\n" +
                                $"Doplňte prosím správnou odpověď k {faultyProblemOrder}. úloze.",
                                "Nezmatematika: Chyba při zveřejňování kurzu");
            }
        }

        public void SaveCurrentCourse()
        {
            CurrentCourse.Save();
        }

        public void UpdateAbilityToContinueCourse()
        {
            var oldValue = Settings.HasCourseToContinue;
            Settings.HasCourseToContinue = IsInStudentMode == true ? true : TeacherCoursesToContinueList.Count > 0;
            if (oldValue != Settings.HasCourseToContinue)
                SaveUserSettings();
        }
        
        public void GetListOfTeacherCoursesToContinue()
        {
            TeacherCoursesToContinueList.Clear();
            List<Course> resultList = new List<Course>();
            if (Directory.Exists(_UserCoursesDirPath()))
            {
                var files = Directory.EnumerateFiles(_UserCoursesDirPath());

                foreach (var file in files)
                {
                    resultList.Add(Course.Read(file));
                }
            }
            TeacherCoursesToContinueList = new ObservableCollection<Course>(resultList.OrderByDescending(c => c.LastEdited));
        }

        public void GetListOfNewCoursesToStart()
        {
            NewCoursesToStartList.Clear();
            List<Course> resultList = new List<Course>();
            if (Directory.Exists(_CoursesDirPath()))
            {
                var files = Directory.EnumerateFiles(_CoursesDirPath());

                foreach (var file in files)
                {
                    resultList.Add(Course.Read(file));
                }
            }

            foreach (UserCourseData userCourseData in CurrentUser.CoursesData)
            {
                var courseToRemove = resultList.Find(c => c.Id == userCourseData.CourseId);
                if (courseToRemove != null)
                {
                    resultList.Remove(courseToRemove);
                }
            }
            NewCoursesToStartList = new ObservableCollection<Course>(resultList.OrderByDescending(c => c.CourseTitle));
        }

        public void GetListOfStudentCoursesToContinue()
        {
            StudentCoursesInProgress.Clear();
            StudentCoursesCompleted.Clear();
            var dirCourseList = new List<Course>();
            var studentCoursesCompleted = new List<(Course, UserCourseData)>();
            var studentCoursesInProgress = new List<(Course, UserCourseData)>();
            
            if (Directory.Exists(_CoursesDirPath()))
            {
                var files = Directory.EnumerateFiles(_CoursesDirPath());

                foreach (var file in files)
                {
                    dirCourseList.Add(Course.Read(file));
                }
            }

            foreach (UserCourseData userCourseData in CurrentUser.CoursesData)
            {
                var continuableCourse = dirCourseList.Find(c => c.Id == userCourseData.CourseId);
                if (continuableCourse != null)
                {
                    if (userCourseData.Completed == true)
                        StudentCoursesCompleted.Add((continuableCourse, userCourseData));
                    else
                        StudentCoursesInProgress.Add((continuableCourse, userCourseData));
                }
            }

            StudentCoursesCompleted = new ObservableCollection<(Course, UserCourseData)>(studentCoursesCompleted.OrderByDescending(c => c.Item2.CourseFinished));
            StudentCoursesInProgress = new ObservableCollection<(Course, UserCourseData)>(studentCoursesInProgress.OrderByDescending(c => c.Item2.LastSessionStarted));
        }

        public void SaveCurrentMathProblem(TextRange textRange)
        {
            SaveMathProblem(CurrentMathProblem, textRange);
            TempAnswers = new ObservableCollection<string>(CurrentMathProblem.CorrectAnswers);
        }

        public void SaveMathProblem(MathProblem mathProblem, TextRange textRange)
        {
            mathProblem.CorrectAnswers = new ObservableCollection<string>(TempAnswers);
            mathProblem.TrimAndPruneCorrectAnswers();
            try
            {
                FileStream fileStream = new FileStream(mathProblem.FilePath, FileMode.Create);
                textRange.Save(fileStream, DataFormats.Rtf);
                fileStream.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message); 
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
