using Nezmatematika.Model;
using Nezmatematika.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Xml.Serialization;

namespace Nezmatematika.ViewModel
{
    public class MainMenuVM : INotifyPropertyChanged
    {
        public const string CourseFilename = "Course.xml";
        public const string UserSettingsFilename = "Settings.xml";
        public string _MySettingsDirectoryPath = System.IO.Path.Combine(App.MyBaseDirectory, "Settings");
        //_UserListPath bude obsahovat seznam uživatelů
        //na indexu [0] bude vždy poslední použitý student (pokud existuje)
        //na indexu [Count-1] bude vždy poslední použitý učitel (pokud existuje)
        public string _UserListPath = System.IO.Path.Combine(App.MyBaseDirectory, "Settings", "UserList.xml");
        public string _DefaultSettingsPath = System.IO.Path.Combine(App.MyBaseDirectory, "Settings", $"Default{UserSettingsFilename}");
        public string _UserSettingsPath = System.IO.Path.Combine(App.MyBaseDirectory, "Settings", $"Default{UserSettingsFilename}");
        public string _CoursesDirPath() => System.IO.Path.Combine(App.MyBaseDirectory, "Courses");
        public string _CoursesArchivedDirPath() => System.IO.Path.Combine(App.MyBaseDirectory, "Courses", "Archived");
        public string _CoursesPublishedDirPath() => System.IO.Path.Combine(App.MyBaseDirectory, "Courses", "Published");
        public string _TeacherCoursesDirPath() => System.IO.Path.Combine(App.MyBaseDirectory, "Courses", CurrentUser.Id);

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

        private List<Course> allArchivedCoursesList;
        public List<Course> AllArchivedCoursesList
        {
            get { return allArchivedCoursesList; }
            set { allArchivedCoursesList = value; }
        }

        private List<Course> allPublishedCoursesList;
        public List<Course> AllPublishedCoursesList
        {
            get { return allPublishedCoursesList; }
            set { allPublishedCoursesList = value; }
        }

        private ObservableCollection<UserCourseData> studentCoursesCompleted;
        public ObservableCollection<UserCourseData> StudentCompletedCoursesData
        {
            get { return studentCoursesCompleted; }
            private set
            {
                studentCoursesCompleted = value;
                OnPropertyChanged("StudentCompletedCoursesData");
            }
        }

        private ObservableCollection<UserCourseData> studentCoursesInProgress;
        public ObservableCollection<UserCourseData> StudentInProgressCoursesData
        {
            get { return studentCoursesInProgress; }
            private set
            {
                studentCoursesInProgress = value;
                OnPropertyChanged("StudentInProgressCoursesData");
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
                    if (IsInStudentMode == true && CurrentUser.UserType == AppMode.Student)
                    {
                        LastStudentUser = CurrentUser;
                    }
                    else if (IsInStudentMode == false && CurrentUser.UserType == AppMode.Teacher)
                    {
                        LastTeacherUser = CurrentUser;
                    }
                    ArchiveLastUsedUsers();

                    _UserSettingsPath = System.IO.Path.Combine(App.MyBaseDirectory, "Settings", $"{CurrentUser.Id}{UserSettingsFilename}");

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
                if (currentMathProblem != null && !currentMathProblem.Equals(value))
                {
                    TeacherNotNullCurrentMathProblemAboutToChange?.Invoke(this, new EventArgs());
                }
                
                currentMathProblem = value;
                
                if (IsInStudentMode == false)
                {
                    PopulateTempAnswersFromCurrentMathProblem();
                    PopulateTempSolutionStepsFromCurrentMathProblem();
                }
                
                if (App.WhereInApp == WhereInApp.CourseForStudent && CurrentUserCourseData != null)
                {
                    TempAnswer = string.Empty;
                    TempSolutionStepText = string.Empty;
                    CurrentProblemSolved = CurrentUserCourseData.GetIsProblemSolved(CurrentMathProblemIndex);

                    if (CurrentProblemSolved)
                        LoadSolvedProblemData();
                    else if (CurrentUserCourseData.VisibleStepsCounts.Count == CurrentMathProblemIndex)
                        CurrentUserCourseData.AddNewVisibleStepsCounter();

                    ReloadShownSolutionSteps();
                }

                OnPropertyChanged("CurrentMathProblem");
                StudentCurrentMathProblemChanged?.Invoke(this, new EventArgs());
                TeacherCurrentMathProblemChanged?.Invoke(this, new EventArgs());
            }
        }

        private int currentMathProblemIndex;

        public int CurrentMathProblemIndex
        {
            get { return currentMathProblemIndex; }
            set
            {
                currentMathProblemIndex = value;
                OnPropertyChanged("CurrentMathProblemIndex");
            }
        }

        private ObservableCollection<string> solutionStepsShownToStudent;
        public ObservableCollection<string> SolutionStepsShownToStudent
        {
            get { return solutionStepsShownToStudent; }
            set
            {
                solutionStepsShownToStudent = value;
                OnPropertyChanged("SolutionStepsShownToStudent");
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

        private string currentSolutionStepText;
        public string CurrentSolutionStepText
        {
            get { return currentSolutionStepText; }
            set
            {
                currentSolutionStepText = value;
                TempSolutionStepText = (CurrentSolutionStepText == null) ? string.Empty : CurrentSolutionStepText;
                OnPropertyChanged("CurrentSolutionStep");
            }
        }

        private UserCourseData currentUserCourseData;
        public UserCourseData CurrentUserCourseData
        {
            get { return currentUserCourseData; }
            set
            {
                currentUserCourseData = value;
                OnPropertyChanged("CurrentUserCourseData");
            }
        }

        private bool currentProblemSolved;
        public bool CurrentProblemSolved
        {
            get { return currentProblemSolved; }
            set
            {
                currentProblemSolved = value;
                OnPropertyChanged("CurrentProblemSolved");
            }
        }


        private string tempTitleBefore;
        public string TempTitleBefore
        {
            get { return tempTitleBefore; }
            set
            {
                tempTitleBefore = value;
                OnPropertyChanged("TempTitleBefore");
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
        private string tempTitleAfter;
        public string TempTitleAfter
        {
            get { return tempTitleAfter; }
            set
            {
                tempTitleAfter = value;
                OnPropertyChanged("TempTitleAfter");
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

        private string tempAnswer;
        public string TempAnswer
        {
            get { return tempAnswer; }
            set
            {
                tempAnswer = value;
                OnPropertyChanged("TempAnswer");
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

        private string tempSolutionStepText;
        public string TempSolutionStepText
        {
            get { return tempSolutionStepText; }
            set
            {
                tempSolutionStepText = value;
                OnPropertyChanged("TempSolutionStepText");
            }
        }

        private ObservableCollection<string> tempSolutionStepsTexts;
        public ObservableCollection<string> TempSolutionStepsTexts
        {
            get { return tempSolutionStepsTexts; }
            set
            {
                tempSolutionStepsTexts = value;
                OnPropertyChanged("TempSolutionStepsTexts");
            }
        }

        private bool isInStudentMode;
        public bool IsInStudentMode
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

        private Visibility studentCoursesToContinueVis;
        public Visibility StudentCoursesToContinueVis
        {
            get { return studentCoursesToContinueVis; }
            set
            {
                studentCoursesToContinueVis = value;
                OnPropertyChanged("StudentCoursesToContinueVis");
            }
        }

        private Visibility teacherCoursesToContinueVis;
        public Visibility TeacherCoursesToContinueVis
        {
            get { return teacherCoursesToContinueVis; }
            set
            {
                teacherCoursesToContinueVis = value;
                OnPropertyChanged("TeacherCoursesToContinueVis");
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

        private Visibility answerFeedbackCorrectVis;
        public Visibility AnswerFeedbackCorrectVis
        {
            get { return answerFeedbackCorrectVis; }
            set
            {
                answerFeedbackCorrectVis = value;
                OnPropertyChanged("AnswerFeedbackCorrectVis");
            }
        }
        private Visibility answerFeedbackIncorrectVis;
        public Visibility AnswerFeedbackIncorrectVis
        {
            get { return answerFeedbackIncorrectVis; }
            set
            {
                answerFeedbackIncorrectVis = value;
                OnPropertyChanged("AnswerFeedbackIncorrectVis");
            }
        }
        private string correctAnswersDisplayList;
        public string CorrectAnswersDisplayList
        {
            get { return correctAnswersDisplayList; }
            set
            {
                correctAnswersDisplayList = value;
                OnPropertyChanged("CorrectAnswersDisplayList");
            }
        }


        private Visibility btnNextProblemVis;
        public Visibility BtnNextProblemVis
        {
            get { return btnNextProblemVis; }
            set
            {
                btnNextProblemVis = value;
                OnPropertyChanged("BtnNextProblemVis");
            }
        }
        private Visibility btnFinishCourseVis;
        public Visibility BtnFinishCourseVis
        {
            get { return btnFinishCourseVis; }
            set
            {
                btnFinishCourseVis = value;
                OnPropertyChanged("BtnFinishCourseVis");
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
        private Visibility aboutAppVis;
        public Visibility AboutAppVis
        {
            get { return aboutAppVis; }
            set
            {
                aboutAppVis = value;
                OnPropertyChanged("AboutAppVis");
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
        public AddNewSolutionStepCommand AddNewSolutionStepCommand { get; set; }
        public ApplyNewSettingsCommand ApplyNewSettingsCommand { get; set; }
        public BackToMainMenuFromAnywhereCommand BackToMainMenuFromAnywhereCommand { get; set; }
        public CheckIfAnswerIsCorrectCommand CheckIfAnswerIsCorrectCommand { get; set; }
        public CreateNewCourseCommand CreateNewCourseCommand { get; set; }
        public CreateNewUserCommand CreateNewUserCommand { get; set; }
        public DeleteCourseCommand DeleteCourseCommand { get; set; }
        public DeleteUserCommand DeleteUserCommand { get; set; }
        public DisplayAboutAppCommand DisplayAboutAppCommand { get; set; }
        public DisplayCourseToEditSelectionCommand DisplayCourseToEditSelectionCommand { get; set; }
        public DisplayNewCourseCreationCommand DisplayNewCourseCreationCommand { get; set; }
        public DisplayNewCourseSelectionCommand DisplayNewCourseSelectionCommand { get; set; }
        public DisplayProfileSelectionCommand DisplayProfileSelectionCommand { get; set; }
        public DisplaySettingsCommand DisplaySettingsCommand { get; set; }
        public DisplayStudentCoursesToContinue DisplayStudentCoursesToContinue { get; set; }
        public EditCorrectAnswerCommand EditCorrectAnswerCommand { get; set; }
        public EditCourseCommand EditCourseCommand { get; set; }
        public EditSolutionStepCommand EditSolutionStepCommand { get; set; }
        public EditUserCommand EditUserCommand { get; set; }
        public FinishTakingCourseCommand FinishTakingCourseCommand { get; set; }
        public MakeStepVisibleCommand MakeStepVisibleCommand { get; set; }
        public OpenCourseForStudentCommand OpenCourseForStudentCommand { get; set; }
        public PrepUserForEditingCommand PrepUserForEditingCommand { get; set; }
        public PublishCourseCommand PublishCourseCommand { get; set; }
        public RemoveCorrectAnswerCommand RemoveCorrectAnswerCommand { get; set; }
        public RemoveSolutionStepCommand RemoveSolutionStepCommand { get; set; }
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
            IsInStudentMode = App.AppMode == AppMode.Student;
            LoadDefaultSettings();
            AllUsersList = new ObservableCollection<User>();
            GetListOfAllUsers();
            SetLastUsedUserFromAllUsers();
            SetCurrentUserToLastUsedUser();
            LoadSettingsForUser(CurrentUser);
            TempAnswers = new ObservableCollection<string>();
            TempSolutionStepsTexts = new ObservableCollection<string>();
            TempCourseTags = new ObservableCollection<string>();
            TempSolutionStepText = string.Empty;
            SolutionStepsShownToStudent = new ObservableCollection<string>();
            EditUserVis = Visibility.Collapsed;
            UserSelVis = Visibility.Collapsed;
            NewCourseVis = Visibility.Collapsed;
            EditCourseVis = Visibility.Collapsed;
            CourseEditorMathProblemUserModeVis = Visibility.Collapsed;
            CourseEditorMathProblemCodeModeVis = Visibility.Collapsed;
            StudentCoursesToContinueVis = Visibility.Collapsed;
            TeacherCoursesToContinueVis = Visibility.Collapsed;
            StudentNewCourseSelVis = Visibility.Collapsed;
            CourseForStudentVis = Visibility.Collapsed;
            ResetAnswerFeedbackVisibility();
            BtnNextProblemVis = Visibility.Collapsed;
            BtnFinishCourseVis = Visibility.Collapsed;
            SettingsVis = Visibility.Collapsed;
            AboutAppVis = Visibility.Collapsed;
            if (IsInStudentMode)
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
            AllArchivedCoursesList = new List<Course>();
            AllPublishedCoursesList = new List<Course>();
            StudentCompletedCoursesData = new ObservableCollection<UserCourseData>();
            StudentInProgressCoursesData = new ObservableCollection<UserCourseData>();
            TeacherCoursesToContinueList = new ObservableCollection<Course>();

            SelectedTextColour = Colors.Black;

            AddNewCorrectAnswerCommand = new AddNewCorrectAnswerCommand(this);
            AddNewProblemCommand = new AddNewProblemCommand(this);
            AddNewSolutionStepCommand = new AddNewSolutionStepCommand(this);
            ApplyNewSettingsCommand = new ApplyNewSettingsCommand(this);
            BackToMainMenuFromAnywhereCommand = new BackToMainMenuFromAnywhereCommand(this);
            CheckIfAnswerIsCorrectCommand = new CheckIfAnswerIsCorrectCommand(this);
            CreateNewCourseCommand = new CreateNewCourseCommand(this);
            CreateNewUserCommand = new CreateNewUserCommand(this);
            DeleteCourseCommand = new DeleteCourseCommand(this);
            DeleteUserCommand = new DeleteUserCommand(this);
            DisplayAboutAppCommand = new DisplayAboutAppCommand(this);
            DisplayCourseToEditSelectionCommand = new DisplayCourseToEditSelectionCommand(this);
            DisplayNewCourseCreationCommand = new DisplayNewCourseCreationCommand(this);
            DisplayNewCourseSelectionCommand = new DisplayNewCourseSelectionCommand(this);
            DisplayProfileSelectionCommand = new DisplayProfileSelectionCommand(this);
            DisplaySettingsCommand = new DisplaySettingsCommand(this);
            DisplayStudentCoursesToContinue = new DisplayStudentCoursesToContinue(this);
            EditCorrectAnswerCommand = new EditCorrectAnswerCommand(this);
            EditCourseCommand = new EditCourseCommand(this);
            EditSolutionStepCommand = new EditSolutionStepCommand(this);
            EditUserCommand = new EditUserCommand(this);
            FinishTakingCourseCommand = new FinishTakingCourseCommand(this);
            MakeStepVisibleCommand = new MakeStepVisibleCommand(this);
            OpenCourseForStudentCommand = new OpenCourseForStudentCommand(this);
            PrepUserForEditingCommand = new PrepUserForEditingCommand(this);
            PublishCourseCommand = new PublishCourseCommand(this);
            RemoveCorrectAnswerCommand = new RemoveCorrectAnswerCommand(this);
            RemoveSolutionStepCommand = new RemoveSolutionStepCommand(this);
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
                if (IsInStudentMode)
                {
                    if (AllUsersList[0].UserType == AppMode.Student)
                        LastStudentUser = AllUsersList[0];
                    else
                        LastStudentUser = null;
                }
                else
                {
                    if (AllUsersList[AllUsersList.Count - 1].UserType == AppMode.Teacher)
                        LastTeacherUser = AllUsersList[AllUsersList.Count - 1];
                    else
                        LastTeacherUser = null;
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
                CurrentUser = LastStudentUser;
            else
                CurrentUser = LastTeacherUser;
        }
        public void LoadSettingsForUser(User user)
        {
            var settingsPath = user != null ? Path.Combine(App.MyBaseDirectory, "Settings", $"{user.Id}{UserSettingsFilename}") : _DefaultSettingsPath;
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
            TempTitleBefore = string.Empty;
            TempFirstName = string.Empty;
            TempLastName = string.Empty;
            TempTitleAfter = string.Empty;
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
            CurrentAnswer = null;
            CurrentMathProblem = null;
            CurrentCourse = null;
            CurrentUserCourseData = null;
        }

        public void BackToMainMenu()
        {
            if (App.WhereInApp == WhereInApp.Settings)
                RestoreUserSettings();

            App.WhereInApp = WhereInApp.MainMenu;

            ClearUserTempValues();
            ClearCourseTempValues();

            //výběry údržby uživatelů
            NewUserVis = Visibility.Collapsed;
            EditUserVis = Visibility.Collapsed;
            UserSelVis = Visibility.Collapsed;

            //učitelské výběry a obrazovky kurzů
            NewCourseVis = Visibility.Collapsed;
            TeacherCoursesToContinueVis = Visibility.Collapsed;
            EditCourseVis = Visibility.Collapsed;
            CourseEditorMathProblemUserModeVis = Visibility.Collapsed;
            CourseEditorMathProblemCodeModeVis = Visibility.Collapsed;

            //studentské výběry a obrazovky kurzů
            StudentNewCourseSelVis = Visibility.Collapsed;
            StudentCoursesToContinueVis = Visibility.Collapsed;
            CourseForStudentVis = Visibility.Collapsed;
            ResetAnswerFeedbackVisibility();

            //výběry nastavení
            SettingsVis = Visibility.Collapsed;
            StudentSettingsVis = Visibility.Collapsed;
            TeacherSettingsVis = Visibility.Collapsed;

            //sekce o programu
            AboutAppVis = Visibility.Collapsed;

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

        public void CreateNewUser(string titBef, string fName, string lName, string titAft, string sName, string cName)
        {
            CurrentUser = new User();

            CurrentUser.TitleBefore = titBef;
            CurrentUser.FirstName = fName;
            CurrentUser.LastName = lName;
            CurrentUser.TitleAfter = titAft;
            CurrentUser.SchoolName = sName;
            CurrentUser.ClassName = cName;
            CurrentUser.UserType = App.AppMode;
            CurrentUser.UpdateDisplayName();
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
            foreach (User user in AllUsersList)
            {
                if (user.UserType == App.AppMode)
                    resultList.Add(user);
            }
            UsersOfTypeList = new ObservableCollection<User>(resultList.OrderBy(u => u.SchoolName)
                                                                        .ThenBy(u => u.ClassName)
                                                                        .ThenBy(u => u.LastName)
                                                                        .ThenBy(u => u.FirstName));
            CurrentUser = curUser;
        }

        public void DeleteUser(User userToDelete)
        {
            string filePath = System.IO.Path.Combine(App.MyBaseDirectory, "Settings", $"{userToDelete.Id}{UserSettingsFilename}");
            File.Delete(filePath);
            UsersOfTypeList.Remove(userToDelete);
            AllUsersList.Remove(userToDelete);
            SaveUserList();
            CurrentUser = UsersOfTypeList[0];
        }

        public void PopulateUserTempValues(User userToEdit)
        {
            TempTitleBefore = userToEdit.TitleBefore;
            TempFirstName = userToEdit.FirstName;
            TempLastName = userToEdit.LastName;
            TempTitleAfter = userToEdit.TitleAfter;
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

        public void PopulateTempSolutionStepsFromCurrentMathProblem()
        {
            if (CurrentMathProblem != null && CurrentMathProblem.SolutionSteps != null)
                TempSolutionStepsTexts = new ObservableCollection<string>(CurrentMathProblem.SolutionSteps);
            else
                TempSolutionStepsTexts = new ObservableCollection<string>();
        }

        public void EditUser(string titBef, string fName, string lName, string titAft, string sName, string cName)
        {
            CurrentUser.TitleBefore = titBef;
            CurrentUser.FirstName = fName;
            CurrentUser.LastName = lName;
            CurrentUser.TitleAfter = titAft;
            CurrentUser.UpdateDisplayName();
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

            if (!CheckUserCourseDataExists())
                CreateUserCourseData(startTime);

            CurrentUserCourseData.UpdateAtSessionStart();
            SaveUCD();

            UpdateAbilityToContinueCourse();

            if (CurrentUserCourseData.Completed)
                BtnFinishCourseVis = Visibility.Visible;
            else
                BtnFinishCourseVis = Visibility.Collapsed;

            SetCurrentMathProblemFromUCD();
            CourseForStudentVis = Visibility.Visible;
            if (CurrentMathProblemIndex < CurrentUserCourseData.CourseProblemCount + CurrentUserCourseData.RequeuedProblems.Count)
                BtnNextProblemVis = Visibility.Visible;
        }
        private void UpdateStats()
        {

        }
        public void SetCurrentMathProblemFromUCD()
        {
            CurrentMathProblemIndex = CurrentUserCourseData.GetIndexToResumeOn();
            SetCurrentMathProblemFromCurrentIndex();
        }

        public void ReloadShownSolutionSteps()
        {
            SolutionStepsShownToStudent.Clear();
            for (int i = 0; i < CurrentUserCourseData.VisibleStepsCounts[CurrentMathProblemIndex]; i++)
            {
                SolutionStepsShownToStudent.Add(CurrentMathProblem.SolutionSteps[i]);
            }
        }

        public void SetCurrentMathProblemFromCurrentIndex()
        {
            int index = CurrentMathProblemIndex;
            if (index >= CurrentUserCourseData.CourseProblemCount)
                index = CurrentUserCourseData.RequeuedProblems[index - CurrentUserCourseData.CourseProblemCount];
            CurrentMathProblem = CurrentCourse.Problems[index];
        }
        public bool IsThisProblemTheLastOne() => CurrentMathProblemIndex == CurrentCourse.Problems.Count + CurrentUserCourseData.RequeuedProblems.Count - 1;
        private bool CheckUserCourseDataExists() => CurrentUser.CoursesData.Find(c => c.CourseId == CurrentCourse.Id && c.Version == CurrentCourse.Version) != null;
        private void CreateUserCourseData(DateTime startTime)
        {
            CurrentUserCourseData = new UserCourseData(CurrentCourse, CurrentUser.Id, startTime);
            CurrentUser.CoursesData.Add(CurrentUserCourseData);
        }
        public bool CheckIfAnswerIsCorrect(string answerToCheck) => CurrentMathProblem.CheckAnswerIsCorrect(answerToCheck);
        public void UpdateUCDAfterAnswer(bool isCorrect)
        {
            if (isCorrect)
                CurrentUserCourseData.UpdateAfterCorrectAnswer();
            else
                CurrentUserCourseData.UpdateAfterIncorrectAnswer(CurrentMathProblem.Index, Settings.RequeueOnMistake);
            SaveUCD();
        }
        public void DisplayAnswerFeedback(bool isCorrect)
        {
            CorrectAnswersDisplayList = CurrentMathProblem.GetCorrectAnswersInOneString();

            if (isCorrect)
                AnswerFeedbackCorrectVis = Visibility.Visible;
            else
                AnswerFeedbackIncorrectVis = Visibility.Visible;
        }
        public void ResetAnswerFeedbackVisibility()
        {
            AnswerFeedbackCorrectVis = Visibility.Collapsed;
            AnswerFeedbackIncorrectVis = Visibility.Collapsed;
        }
        public void BtnNextToBtnFinish()
        {
            BtnNextProblemVis = Visibility.Collapsed;
            BtnFinishCourseVis = Visibility.Visible;
        }
        public void LoadSolvedProblemData()
        {
            TempAnswer = CurrentUserCourseData.StudentAnswers[CurrentMathProblemIndex];
            DisplayAnswerFeedback(CurrentMathProblem.CheckAnswerIsCorrect(TempAnswer));
        }
        public void SaveUCD()
        {
            SaveUserList();
        }
        public void CreateNewCourse()
        {
            CurrentCourse = new Course(CurrentUser, TempCourseTitle, TempCourseDesc, TempCourseTags);
            CurrentCourse.AddNewMathProblem();
            CurrentMathProblem = CurrentCourse.Problems[0];

            Directory.CreateDirectory(CurrentCourse.DirPath);
            TeacherCoursesToContinueList.Add(CurrentCourse);
            UpdateAbilityToContinueCourse();
        }

        public void StartEditingCurrentCourse()
        {
            CurrentCourse.LastOpened = DateTime.Now;
            CurrentMathProblem = CurrentCourse.Problems[0];
            PopulateTempAnswersFromCurrentMathProblem();
            PopulateTempSolutionStepsFromCurrentMathProblem();
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

        public void ReloadSolutionSteps()
        {
            CurrentMathProblem.SolutionSteps.Clear();
            CurrentMathProblem.SolutionSteps = new ObservableCollection<string>(TempSolutionStepsTexts);
        }

        public void ReloadTempSolutionSteps()
        {
            TempSolutionStepsTexts.Clear();
            TempSolutionStepsTexts = new ObservableCollection<string>(CurrentMathProblem.SolutionSteps);
        }

        public void ReplaceInTempSolutionSteps(string oldStepText, string newStepText)
        {
            var replacingAt = TempSolutionStepsTexts.IndexOf(oldStepText);
            if (replacingAt > -1)
                TempSolutionStepsTexts[replacingAt] = newStepText;
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
                CurrentCourse.Publish(_CoursesPublishedDirPath(), _CoursesArchivedDirPath());
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
            Settings.HasCourseToContinue = IsInStudentMode ? true : TeacherCoursesToContinueList.Count > 0;
            if (oldValue != Settings.HasCourseToContinue)
                SaveUserSettings();
        }

        public void GetListOfTeacherCoursesToContinue()
        {
            TeacherCoursesToContinueList.Clear();
            List<Course> resultList = new List<Course>();
            if (Directory.Exists(_TeacherCoursesDirPath()))
            {
                var files = Directory.EnumerateFiles(_TeacherCoursesDirPath());

                foreach (var file in files)
                {
                    resultList.Add(Course.Read(file));
                }
            }
            TeacherCoursesToContinueList = new ObservableCollection<Course>(resultList.OrderByDescending(c => c.LastEdited));
        }

        private void GetAllArchivedCoursesList()
        {
            AllArchivedCoursesList.Clear();
            if (Directory.Exists(_CoursesArchivedDirPath()))
            {
                var files = Directory.EnumerateFiles(_CoursesArchivedDirPath());

                foreach (var file in files)
                {
                    AllArchivedCoursesList.Add(Course.Read(file));
                }
            }
        }
        private void GetAllPublishedCoursesList()
        {
            AllPublishedCoursesList.Clear();
            if (Directory.Exists(_CoursesPublishedDirPath()))
            {
                var files = Directory.EnumerateFiles(_CoursesPublishedDirPath());

                foreach (var file in files)
                {
                    AllPublishedCoursesList.Add(Course.Read(file));
                }
            }
        }

        public void GetListOfNewCoursesToStart()
        {
            NewCoursesToStartList.Clear();
            GetAllPublishedCoursesList();
            List<Course> resultList = new List<Course>(AllPublishedCoursesList);

            foreach (UserCourseData userCourseData in CurrentUser.CoursesData)
            {
                var courseToRemove = resultList.Find(c => c.Id == userCourseData.CourseId && c.Version == userCourseData.Version);
                if (courseToRemove != null)
                    resultList.Remove(courseToRemove);
            }
            NewCoursesToStartList = new ObservableCollection<Course>(resultList.OrderByDescending(c => c.CourseTitle));
        }

        public void GetListOfStudentCoursesToContinue()
        {
            StudentInProgressCoursesData.Clear();
            StudentCompletedCoursesData.Clear();
            GetAllArchivedCoursesList();
            GetAllPublishedCoursesList();

            var listCoursesCompleted = new List<UserCourseData>();
            var listCoursesInProgress = new List<UserCourseData>();

            foreach (UserCourseData userCourseData in CurrentUser.CoursesData)
            {
                var continuableCourse = AllPublishedCoursesList.Find(c => c.Id == userCourseData.CourseId && c.Version == userCourseData.Version);
                if (continuableCourse == null)
                    continuableCourse = AllArchivedCoursesList.Find(c => c.Id == userCourseData.CourseId && c.Version == userCourseData.Version);
                if (continuableCourse != null)
                {
                    if (userCourseData.Completed == true)
                        listCoursesCompleted.Add(userCourseData);
                    else
                        listCoursesInProgress.Add(userCourseData);
                }
            }

            StudentCompletedCoursesData = new ObservableCollection<UserCourseData>(listCoursesCompleted.OrderByDescending(c => c.CourseFinished));
            StudentInProgressCoursesData = new ObservableCollection<UserCourseData>(listCoursesInProgress.OrderByDescending(c => c.LastSessionStarted));
        }

        public void SaveCurrentMathProblem(TextRange textRange)
        {
            SaveMathProblem(CurrentMathProblem, textRange);
            TempAnswers = new ObservableCollection<string>(CurrentMathProblem.CorrectAnswers);
            PopulateTempSolutionStepsFromCurrentMathProblem();
        }

        public void SaveMathProblem(MathProblem mathProblem, TextRange textRange)
        {
            mathProblem.CorrectAnswers = new ObservableCollection<string>(TempAnswers);
            mathProblem.SolutionSteps = new ObservableCollection<string>(TempSolutionStepsTexts);
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
