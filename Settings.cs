﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GitMenu
{
    public class Settings : Singleton<Settings>
    {

        public string GitPath { get; private set; }
        public string ShPath { get; private set; }

        public Settings()
        {
            this.GitPath = @"C:\Program Files\Git\bin\git.exe";
            this.ShPath = @"C:\Program Files\Git\bin\sh.exe";
        }
    }
}
