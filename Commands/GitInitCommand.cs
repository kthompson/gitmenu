﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace GitMenu.Commands
{
    public class GitInitCommand : GitCommand
    {
        public GitInitCommand(IServiceProvider provider)
            : base(provider, PkgCmdIDList.cmdGitInit, CommandFlags.NoRepository, "Git I&nit Here")
        {
        }

        protected override void OnExecute()
        {
            string file = GitCommand.SelectedItem.GetFullPath();
            string wd = WDFromPath(file);
            Exec(wd, true, Settings.Instance.GitPath, "init");
        }
    }
}
