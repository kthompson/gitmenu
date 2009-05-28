﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GitMenu.Commands
{
    public class GitHistoryCommand : GitCommand
    {
        public GitHistoryCommand(IServiceProvider provider)
            : base(provider, PkgCmdIDList.cmdGitHistory, CommandFlags.Track, "Git &History")
        {

        }

        protected override void OnExecute()
        {
            string file = GitCommand.SelectedProjectItem.GetFullPath();
            bool isDir;
            string wd = WDFromPath(file, out isDir);
            string name = "";
            string output;
            if(!isDir)
                name = file.Substring(wd.Length + 1);

            Exec(wd, true, out output, Settings.Instance.ShPath,  "--login", "-i", "/bin/gitk", "HEAD", "--", name);
        }
    }
}
