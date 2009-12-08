using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GitMenu.Commands
{
    public class HistoryCommand : GitCommand
    {
        public HistoryCommand(GitMenuPackage provider)
            : base(provider, PkgCmdIdList.CmdGitHistory, CommandFlags.Track, "Git &History")
        {

        }

        protected override void OnExecute()
        {
            var file = GetSelectedPath();
            bool isDir;
            var wd = Helper.WorkingDirectoryFromPath(file, out isDir);
            var name = "";
            string output;
            if(!isDir)
                name = file.Substring(wd.Length + 1);

            Helper.Exec(wd, true, out output, Settings.Instance.ShPath, "--login", "-i", "/bin/gitk", "HEAD", "--", name);
        }
    }
}
