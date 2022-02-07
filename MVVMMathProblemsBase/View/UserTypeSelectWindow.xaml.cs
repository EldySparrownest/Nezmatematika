using MVVMMathProblemsBase.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MVVMMathProblemsBase.View
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
