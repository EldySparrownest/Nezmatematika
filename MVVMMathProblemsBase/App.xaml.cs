﻿using MVVMMathProblemsBase.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MVVMMathProblemsBase
{
    /// <summary>
    /// Interakční logika pro App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static User AppUser = null;
        public static bool? IsInStudentMode = null;
    }
}
