using Nezmatematika.Model;
using Nezmatematika.ViewModel.Commands;
using Nezmatematika.ViewModel.Helpers;
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
using static Nezmatematika.ViewModel.Helpers.FilePathHelper;

namespace Nezmatematika.ViewModel
{
    public class MainMenuVM : INotifyPropertyChanged
    {
        #region SettingsValues
        private bool isInStudentMode;
        public bool IsInStudentMode
        {
            get { return isInStudentMode; }
            set
            {
                isInStudentMode = value;
                StudentVis = isInStudentMode ? Visibility.Visible : Visibility.Collapsed;
                TeacherVis = isInStudentMode ? Visibility.Collapsed : Visibility.Visible;
                OnPropertyChanged("IsInStudentMode");
            }
        }

        private UserBase lastStudentUserBase;
        public UserBase LastStudentUserBase
        {
            get { return lastStudentUserBase; }
            set
            {
                lastStudentUserBase = value;
                OnPropertyChanged("LastStudentUserBase");
            }
        }

        private UserBase lastTeacherUserBase;
        public UserBase LastTeacherUserBase
        {
            get { return lastTeacherUserBase; }
            set
            {
                lastTeacherUserBase = value;
                OnPropertyChanged("LastTeacherUserBase");
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
        #endregion SettingsValues

        #region Collections
        private ObservableCollection<UserBase> allUserBasesList;
        public ObservableCollection<UserBase> AllUserBasesList
        {
            get { return allUserBasesList; }
            set
            {
                allUserBasesList = value;
                OnPropertyChanged("AllUserBasesList");
            }
        }

        private ObservableCollection<UserBase> userBasesOfTypeList;
        public ObservableCollection<UserBase> UserBasesOfTypeList
        {
            get { return userBasesOfTypeList; }
            private set
            {
                userBasesOfTypeList = value;
                OnPropertyChanged("UserBasesOfTypeList");
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
        #endregion Collections

        #region CurrentValues
        private User currentUser;
        public User CurrentUser
        {
            get { return currentUser; }
            set
            {
                currentUser = value;

                if (value != null)
                {
                    if (IsInStudentMode == true && CurrentUser.UserBase.UserType == AppMode.Student)
                        LastStudentUserBase = CurrentUser.UserBase;
                    else if (IsInStudentMode == false && CurrentUser.UserBase.UserType == AppMode.Teacher)
                        LastTeacherUserBase = CurrentUser.UserBase;

                    ArchiveLastUsedUsers();

                    if (EditUserVis == Visibility.Visible)
                        PopulateUserTempValues(CurrentUser.UserBase);
                }
                LoadSettingsForCurrentUser();
                ReloadCurrentUserStats();
                OnPropertyChanged("CurrentUser");
            }
        }

        private UserBase currentUserBase;
        public UserBase CurrentUserBase
        {
            get { return currentUserBase; }
            set
            {
                currentUserBase = value;
                SetCurrentUserFromUserBase(currentUserBase);
                OnPropertyChanged("CurrentUserBase");
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
                    BtnHintVis = Visibility.Visible;
                    TempAnswer = string.Empty;
                    TempSolutionStepText = string.Empty;
                    CurrentProblemSolved = CurrentUserCourseData.GetIsProblemSolved(CurrentMathProblemIndex);

                    if (CurrentProblemSolved)
                    {
                        BtnHintVis = Visibility.Collapsed;
                        LoadSolvedProblemData();
                    }
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

        private string correctAnswersAsOneString;
        public string CorrectAnswersAsOneString
        {
            get { return correctAnswersAsOneString; }
            set
            {
                correctAnswersAsOneString = value;
                OnPropertyChanged("CorrectAnswersAsOneString");
            }
        }

        private Dictionary<string, string> dicStats;

        public Dictionary<string, string> DicStats
        {
            get { return dicStats; }
            set
            {
                dicStats = value;
                OnPropertyChanged("DicStats");
            }
        }

        #endregion CurrentValues

        #region Temp
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
            get { return tempFirstName; }
            set
            {
                tempFirstName = value;
                OnPropertyChanged("TempFirstName");
            }
        }
        private string tempLastName;
        public string TempLastName
        {
            get { return tempLastName; }
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
            get { return tempClassName; }
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
        #endregion Temp

        #region Visibilities
        private Visibility mainMenuVis;
        public Visibility MainMenuVis
        {
            get { return mainMenuVis; }
            set
            {
                mainMenuVis = value;
                OnPropertyChanged("MainMenuVis");
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
        private Visibility btnHintVis;
        public Visibility BtnHintVis
        {
            get { return btnHintVis; }
            set
            {
                btnHintVis = value;
                OnPropertyChanged("BtnHintVis");
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
        private Visibility statsVis;
        public Visibility StatsVis
        {
            get { return statsVis; }
            set
            {
                statsVis = value;
                OnPropertyChanged("StatsVis");
            }
        }
        private Visibility studentStatsVis;
        public Visibility StudentStatsVis
        {
            get { return studentStatsVis; }
            set
            {
                studentStatsVis = value;
                OnPropertyChanged("StudentStatsVis");
            }
        }
        private Visibility teacherStatsVis;
        public Visibility TeacherStatsVis
        {
            get { return teacherStatsVis; }
            set
            {
                teacherStatsVis = value;
                OnPropertyChanged("TeacherStatsVis");
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
        #endregion Visibilities

        #region Commands
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
        public DisplayStatisticsCommand DisplayStatisticsCommand { get; set; }
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
        public RemoveProblemCommand RemoveProblemCommand { get; set; }
        public RemoveSolutionStepCommand RemoveSolutionStepCommand { get; set; }
        public RestoreDefaultSettingsCommand RestoreDefaultSettingsCommand { get; set; }
        public SwitchBetweenUserAndCodeModeCommand SwitchBetweenUserAndCodeModeCommand { get; set; }
        public SwitchToNextProblemCommand SwitchToNextProblemCommand { get; set; }
        public SwitchToPreviousProblemCommand SwitchToPreviousProblemCommand { get; set; }
        #endregion Commands

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler StudentCurrentMathProblemChanged;
        public event EventHandler TeacherNotNullCurrentMathProblemAboutToChange;
        public event EventHandler TeacherCurrentMathProblemChanged;
        #endregion Events

        #region Constructors
        public MainMenuVM()
        {
            IsInStudentMode = App.AppMode == AppMode.Student;
            LoadDefaultSettings();
            AllUserBasesList = new ObservableCollection<UserBase>();
            GetListOfAllUsers();
            SetLastUsedUserFromAllUsers();
            SetCurrentUserToLastUsedUser();
            TempAnswers = new ObservableCollection<string>();
            TempSolutionStepsTexts = new ObservableCollection<string>();
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
            BtnHintVis = Visibility.Collapsed;
            BtnNextProblemVis = Visibility.Collapsed;
            BtnFinishCourseVis = Visibility.Collapsed;
            StatsVis = Visibility.Collapsed;
            SettingsVis = Visibility.Collapsed;
            AboutAppVis = Visibility.Collapsed;

            NewUserVis = (CurrentUser == null) ? Visibility.Visible : Visibility.Collapsed;

            UserBasesOfTypeList = new ObservableCollection<UserBase>();
            GetUserBasesOfTypeList();

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
            DisplayStatisticsCommand = new DisplayStatisticsCommand(this);
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
            RemoveProblemCommand = new RemoveProblemCommand(this);
            RemoveSolutionStepCommand = new RemoveSolutionStepCommand(this);
            RestoreDefaultSettingsCommand = new RestoreDefaultSettingsCommand(this);
            SwitchBetweenUserAndCodeModeCommand = new SwitchBetweenUserAndCodeModeCommand(this);
            SwitchToNextProblemCommand = new SwitchToNextProblemCommand(this);
            SwitchToPreviousProblemCommand = new SwitchToPreviousProblemCommand(this);
        }
        #endregion Constructors

        #region SettingAndLoadingMethods
        public void LoadDefaultSettings()
        {
            if (File.Exists(_DefaultSettingsFullPath))
                this.Settings = MySettings.Read(_DefaultSettingsFullPath);
        }
        public void SetLastUsedUserFromAllUsers()
        {
            if (AllUserBasesList.Count >= 1)
            {
                LastStudentUserBase = (AllUserBasesList[0].UserType == AppMode.Student) ? AllUserBasesList[0] : null;
                LastTeacherUserBase = (AllUserBasesList[AllUserBasesList.Count - 1].UserType == AppMode.Teacher) ? AllUserBasesList[AllUserBasesList.Count - 1] : null;
            }
            else
            {
                LastStudentUserBase = null;
                LastTeacherUserBase = null;
            }
            SaveUserList();
        }
        public void SetCurrentUserFromUserBase(UserBase userBase)
        {
            CurrentUser = userBase == null ? null : new User(userBase, GetFullPath(FullPathOptions.FileUserCoursesData, userBase), GetFullPath(FullPathOptions.FileUserStats, userBase));
        }
        public void SetCurrentUserToLastUsedUser()
        {
            var userBase = IsInStudentMode ? LastStudentUserBase : LastTeacherUserBase;
            SetCurrentUserFromUserBase(userBase);
        }
        public void LoadSettingsForCurrentUser()
        {
            var settingsPath = CurrentUser != null ? GetFullPath(FullPathOptions.FileUserSettings, CurrentUser.UserBase) : _DefaultSettingsFullPath;
            if (File.Exists(settingsPath))
                this.Settings = MySettings.Read(settingsPath);
        }
        public void UpdateAbilityToContinueCourse()
        {
            var oldValue = Settings.HasCourseToContinue;
            Settings.HasCourseToContinue = IsInStudentMode ? true : TeacherCoursesToContinueList.Count > 0;
            if (oldValue != Settings.HasCourseToContinue)
                SaveUserSettings();
        }
        public void OpenCurrentCourseForStudent()
        {
            App.WhereInApp = WhereInApp.CourseForStudent;
            var startTime = DateTime.Now;

            if (!CheckUserCourseDataExists())
            {
                CreateUserCourseData(startTime);
                CurrentUser.UserStats.NewCourseStartedUpdate();
            }

            CurrentUserCourseData.UpdateAtSessionStart();
            SaveDataAndStats();

            UpdateAbilityToContinueCourse();

            BtnFinishCourseVis = CurrentUserCourseData.Completed ? Visibility.Visible : Visibility.Collapsed;

            SetCurrentMathProblemFromUCD();
            CourseForStudentVis = Visibility.Visible;
            if (CurrentMathProblemIndex < CurrentUserCourseData.CourseProblemCount + CurrentUserCourseData.RequeuedProblems.Count)
                BtnNextProblemVis = Visibility.Visible;
        }
        public void SetCurrentMathProblemFromUCD()
        {
            CurrentMathProblemIndex = CurrentUserCourseData.GetIndexToResumeOn();
            SetCurrentMathProblemFromCurrentIndex();
        }
        public void PopulateUserTempValues(UserBase userBaseToEdit)
        {
            TempTitleBefore = userBaseToEdit.TitleBefore;
            TempFirstName = userBaseToEdit.FirstName;
            TempLastName = userBaseToEdit.LastName;
            TempTitleAfter = userBaseToEdit.TitleAfter;
            TempSchoolName = userBaseToEdit.SchoolName;
            TempClassName = userBaseToEdit.ClassName;
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
        public void ReloadCurrentUserStats()
        {
            var stats = (CurrentUser != null && CurrentUser.UserStats != null) ? CurrentUser.UserStats : new UserStats();
            DicStats = stats.GetAsDictionary();
        }
        public void ReloadShownSolutionSteps()
        {
            SolutionStepsShownToStudent.Clear();
            for (int i = 0; i < CurrentUserCourseData.VisibleStepsCounts[CurrentMathProblemIndex]; i++)
            {
                SolutionStepsShownToStudent.Add(CurrentMathProblem.SolutionSteps[i]);
            }
        }
        #endregion SettingAndLoadingMethods

        #region SavingMethods
        public void SaveUserSettings()
        {
            Settings.Save(GetFullPath(FullPathOptions.FileUserSettings, CurrentUser.UserBase));
        }
        public void CreateNewUser(string titBef, string fName, string lName, string titAft, string sName, string cName)
        {
            CurrentUser = new User(titBef, fName, lName, titAft, sName, cName);

            this.Settings.ThisUser = CurrentUser.UserBase;
            this.Settings.HasCourseToContinue = false;
            RestoreDefaultSettingsForCurrentUser();
            SaveUserSettings();
            AllUserBasesList.Add(CurrentUser.UserBase);
            SaveUserList();
            UserBasesOfTypeList.Add(CurrentUser.UserBase);
            ClearUserTempValues();
        }
        public void DeleteUser(UserBase userBaseToDelete)
        {
            File.Delete(GetFullPath(FullPathOptions.FileUserStats, userBaseToDelete));
            File.Delete(GetFullPath(FullPathOptions.FileUserCoursesData, userBaseToDelete));
            File.Delete(GetFullPath(FullPathOptions.FileUserSettings, userBaseToDelete));
            UserBasesOfTypeList.Remove(userBaseToDelete);
            AllUserBasesList.Remove(userBaseToDelete);
            SaveUserList();
            SetCurrentUserFromUserBase(UserBasesOfTypeList[0]);
        }
        public void EditUser(string titBef, string fName, string lName, string titAft, string sName, string cName)
        {
            CurrentUser.UserBase.Edit(titBef, fName, lName, titAft, sName, cName);
            SaveUserList();
            ClearUserTempValues();
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
                FileStream fileStream = new FileStream(Path.Combine(App.MyBaseDirectory, mathProblem.RelFilePath), FileMode.Create);
                textRange.Save(fileStream, DataFormats.Rtf);
                fileStream.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        public void ArchiveLastUsedUsers()
        {
            if (CurrentUser != null && AllUserBasesList.Count >= 1)
            {
                int index = (IsInStudentMode == true) ? 0 : AllUserBasesList.Count - 1;
                for (int i = 0; i < AllUserBasesList.Count; i++)
                {
                    if (AllUserBasesList[i].Equals(CurrentUser.UserBase))
                    {
                        AllUserBasesList.RemoveAt(i);
                        AllUserBasesList.Insert(index, CurrentUser.UserBase);
                        SaveUserList();
                        return;
                    }
                }
            }
        }
        public void SaveUserList()
        {
            XmlHelper.Save(_UserListFullPath(), typeof(List<UserBase>), new List<UserBase>(this.AllUserBasesList));
        }
        #endregion SavingMethods

        #region ClearingAndResettingMethods
        public void BackToMainMenu()
        {
            if (App.WhereInApp == WhereInApp.Settings)
                LoadSettingsForCurrentUser();

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

            //výběry statistik
            StatsVis = Visibility.Collapsed;
            StudentStatsVis = Visibility.Collapsed;
            TeacherStatsVis = Visibility.Collapsed;

            //výběry nastavení
            SettingsVis = Visibility.Collapsed;
            StudentSettingsVis = Visibility.Collapsed;
            TeacherSettingsVis = Visibility.Collapsed;

            //sekce o programu
            AboutAppVis = Visibility.Collapsed;

            MainMenuVis = Visibility.Visible;
            StudentVis = IsInStudentMode ? Visibility.Visible : Visibility.Collapsed;
            TeacherVis = IsInStudentMode ? Visibility.Collapsed : Visibility.Visible;
        }
        public void RestoreDefaultSettingsForCurrentUser()
        {
            var usersNewSettings = MySettings.Read(_DefaultSettingsFullPath);
            usersNewSettings.HasCourseToContinue = this.Settings.HasCourseToContinue;
            usersNewSettings.Save(GetFullPath(FullPathOptions.FileUserSettings, CurrentUser.UserBase));
            LoadSettingsForCurrentUser();
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
        }
        public void ClearCurrentValuesExceptUser()
        {
            CurrentAnswer = null;
            CurrentMathProblem = null;
            CurrentCourse = null;
            CurrentUserCourseData = null;
        }
        #endregion ClearingAndResettingMethods

        #region MethodForCoursesInStudentMode
        public void SetCurrentMathProblemFromCurrentIndex()
        {
            int index = CurrentMathProblemIndex;
            if (index >= CurrentUserCourseData.CourseProblemCount)
                index = CurrentUserCourseData.RequeuedProblems[index - CurrentUserCourseData.CourseProblemCount];
            CurrentMathProblem = CurrentCourse.Problems[index];
        }
        public bool IsThisProblemTheLastOne() => CurrentMathProblemIndex == CurrentCourse.Problems.Count + CurrentUserCourseData.RequeuedProblems.Count - 1;
        private bool CheckUserCourseDataExists() => CurrentUser.CoursesData.Find(c => c.CourseId == CurrentCourse?.Id && c.Version == CurrentCourse?.Version) != null;
        private void CreateUserCourseData(DateTime startTime)
        {
            CurrentUserCourseData = new UserCourseData(CurrentCourse, CurrentUser.UserBase.Id, startTime);
            CurrentUser.CoursesData.Add(CurrentUserCourseData);
        }
        public bool CheckIfAnswerIsCorrect(string answerToCheck) => CurrentMathProblem.CheckAnswerIsCorrect(answerToCheck);
        public void UpdateUCDAndStatsAfterAnswer(bool isCorrect)
        {
            if (isCorrect)
            {
                CurrentUserCourseData.UpdateAfterCorrectAnswer(out var completed);
                if (completed)
                {
                    CurrentUserCourseData.UpdateWhenCourseCompleted(out var sessionDuration);
                    CurrentUser.UserStats.SessionEndUpdate(sessionDuration);
                    CurrentUser.UserStats.CourseCompletedUpdate();
                }

            }
            else
                CurrentUserCourseData.UpdateAfterIncorrectAnswer(CurrentMathProblem.Index, Settings.RequeueOnMistake);

            var firstTry = CurrentMathProblemIndex < CurrentCourse.Problems.Count;
            var withoutHints = SolutionStepsShownToStudent.Count == 0;
            CurrentUser.UserStats.AnswerSentUpdate(isCorrect, firstTry, withoutHints);
            SaveDataAndStats();
        }
        public void DisplayAnswerFeedback(bool isCorrect)
        {
            CorrectAnswersAsOneString = CurrentMathProblem.GetCorrectAnswersInOneString();

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
        public void SaveDataAndStats()
        {
            CurrentUser.SaveDataAndStats(GetFullPath(FullPathOptions.FileUserCoursesData, CurrentUser.UserBase), GetFullPath(FullPathOptions.FileUserStats, CurrentUser.UserBase));
        }
        #endregion MethodForCoursesInStudentMode

        #region MethodsForCoursesInEditor
        public void CreateNewCourse()
        {
            CurrentCourse = new Course(CurrentUser.UserBase, TempCourseTitle);
            CurrentCourse.AddNewMathProblem(Settings.CapitalisationMatters);
            CurrentMathProblem = CurrentCourse.Problems[0];
            CurrentUser.UserStats.CourseCreatedUpdate();
            SaveDataAndStats();

            Directory.CreateDirectory(Path.Combine(App.MyBaseDirectory, CurrentCourse.RelDirPath));
            TeacherCoursesToContinueList.Add(CurrentCourse);
            UpdateAbilityToContinueCourse();

            SaveCurrentCourse();
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
            {
                CurrentCourse.PublishCourse(_CoursesPublishedRelDirPath(), _CoursesArchivedRelDirPath(), out int problemCountChange); // cesty musejí být relativní !!!
                CurrentUser.UserStats.CoursePublishedUpdate(CurrentCourse.Version, problemCountChange);
                SaveDataAndStats();
            }
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
        #endregion MethodsForCoursesInEditor

        #region GetMethods
        public void GetListOfAllUsers()
        {
            AllUserBasesList.Clear();
            List<UserBase> resultList = new List<UserBase>();
            if (File.Exists(_UserListFullPath()))
            {
                using (StreamReader sw = new StreamReader(_UserListFullPath()))
                {
                    XmlSerializer xmls = new XmlSerializer(typeof(List<UserBase>));
                    resultList = xmls.Deserialize(sw) as List<UserBase>;
                }
            }
            AllUserBasesList = new ObservableCollection<UserBase>(resultList);
        }

        public void GetUserBasesOfTypeList()
        {
            var curUserBase = CurrentUser.UserBase;
            UserBasesOfTypeList.Clear();
            var resultList = new List<UserBase>();
            foreach (UserBase user in AllUserBasesList)
            {
                if (user.UserType == App.AppMode)
                    resultList.Add(user);
            }
            UserBasesOfTypeList = new ObservableCollection<UserBase>(resultList.OrderBy(u => u.LastName)
                                                                        .ThenBy(u => u.FirstName)
                                                                        .ThenBy(u => u.ClassName)
                                                                        .ThenBy(u => u.SchoolName));
            CurrentUserBase = curUserBase;
        }

        public void GetListOfTeacherCoursesToContinue()
        {
            TeacherCoursesToContinueList.Clear();
            List<Course> resultList = new List<Course>();
            var fullTeacherDirPath = GetFullPath(FullPathOptions.DirCoursesTeacher, CurrentUser.UserBase);
            if (Directory.Exists(fullTeacherDirPath))
            {
                var files = Directory.EnumerateFiles(fullTeacherDirPath);

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
            var archivedFullDirPath = GetFullPath(FullPathOptions.DirCoursesArchived);
            if (Directory.Exists(archivedFullDirPath))
            {
                var files = Directory.EnumerateFiles(archivedFullDirPath);

                foreach (var file in files)
                {
                    AllArchivedCoursesList.Add(Course.Read(file));
                }
            }
        }
        private void GetAllPublishedCoursesList()
        {
            AllPublishedCoursesList.Clear();
            var publishedFullDirPath = GetFullPath(FullPathOptions.DirCoursesPublished);
            if (Directory.Exists(publishedFullDirPath))
            {
                var files = Directory.EnumerateFiles(publishedFullDirPath);

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
        #endregion GetMethods

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
