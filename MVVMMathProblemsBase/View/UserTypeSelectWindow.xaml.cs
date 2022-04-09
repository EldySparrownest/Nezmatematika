using Nezmatematika.ViewModel;
using System.Windows;

namespace Nezmatematika.View
{
    /// <summary>
    /// Interakční logika pro UserTypeSelectWindow.xaml
    /// </summary>
    public partial class UserTypeSelectWindow : Window
    {
        UserTypeSelectVM vM;

        public UserTypeSelectWindow()
        {
            InitializeComponent();

            vM = Resources["vm"] as UserTypeSelectVM;
        }

        private void SelectModeAndSwitchToMainMenu(bool isInStudentMode)
        {
            App.AppMode = isInStudentMode ? AppMode.Student : AppMode.Teacher;
            MainMenuWindow mainMenuWindow = new MainMenuWindow();
            App.WhereInApp = WhereInApp.MainMenu;
            mainMenuWindow.Show();
            this.Close();
        }

        private void btnStudent_Click(object sender, RoutedEventArgs e)
        {
            SelectModeAndSwitchToMainMenu(true);
        }

        private void btnTeacher_Click(object sender, RoutedEventArgs e)
        {
            SelectModeAndSwitchToMainMenu(false);
        }
    }
}
