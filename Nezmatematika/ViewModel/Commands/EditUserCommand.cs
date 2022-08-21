using System;
using System.Windows;
using System.Windows.Input;

namespace Nezmatematika.ViewModel.Commands
{
    public class EditUserCommand : ICommand
    {
        public MainMenuVM MMVM { get; set; }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public EditUserCommand(MainMenuVM vm)
        {
            MMVM = vm;
        }

        public bool CanExecute(object parameter)
        {
            if (MMVM.CurrentUser == null)
                return false;
            if (String.IsNullOrEmpty(MMVM.TempFirstName))
                return false;
            if (String.IsNullOrEmpty(MMVM.TempLastName))
                return false;
            if (String.IsNullOrEmpty(MMVM.TempSchoolName))
                return false;
            if (String.IsNullOrEmpty(MMVM.TempClassName))
                return false;
            return true;
        }

        public void Execute(object parameter)
        {
            MMVM.EditUser(MMVM.TempTitleBefore, MMVM.TempFirstName, MMVM.TempLastName, MMVM.TempTitleAfter, MMVM.TempSchoolName, MMVM.TempClassName);
            MMVM.EditUserVis = Visibility.Collapsed;
            MMVM.NewUserVis = Visibility.Visible;
            MMVM.GetUserBasesOfTypeList();
        }
    }
}
