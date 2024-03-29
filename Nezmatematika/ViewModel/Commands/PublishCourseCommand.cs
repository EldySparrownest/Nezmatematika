﻿using Nezmatematika.Model;
using System;
using System.Windows.Input;

namespace Nezmatematika.ViewModel.Commands
{
    public class PublishCourseCommand : ICommand
    {
        public MainMenuVM MMVM { get; set; }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public PublishCourseCommand(MainMenuVM vm)
        {
            MMVM = vm;
        }

        public bool CanExecute(object parameter)
        {
            return App.WhereInApp == WhereInApp.CourseEditor && MMVM.CurrentCourse != null;
        }

        public void Execute(object parameter)
        {
            MMVM.PublishCurrentCourse();
        }
    }
}
