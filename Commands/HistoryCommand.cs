using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GitMenu.Commands
{
    public class HistoryCommand : GitCommand
    {
        public HistoryCommand(GitMenuPackage provider)
            : base(provider, PkgCmdIDList.cmdGitHistory, CommandFlags.Track, "Git &History")
        {

        }

        protected override void OnExecute()
        {
            string file = GitCommand.GetSelectedPath();
            bool isDir;
            string wd = Helper.WorkingDirectoryFromPath(file, out isDir);
            string name = "";
            string output;
            if(!isDir)
                name = file.Substring(wd.Length + 1);

            Helper.Exec(wd, true, out output, Settings.Instance.ShPath, "--login", "-i", "/bin/gitk", "HEAD", "--", name);
        }
    }
}
