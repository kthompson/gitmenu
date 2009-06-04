using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GitMenu.Commands
{
    public class BlameCommand : GitCommand
    {
        public BlameCommand(GitMenuPackage provider)
            : base(provider, PkgCmdIDList.cmdGitBlame, CommandFlags.Track | CommandFlags.File, "Git &Blame")
        {

        }

        protected override void OnExecute()
        {
            string file = GitCommand.GetSelectedPath();
            bool isDir;
            string wd = Helper.WorkingDirectoryFromPath(file, out isDir);
            string name = "";
            string output;
            if (!isDir)
            {
                name = file.Substring(wd.Length + 1);

                Helper.Exec(wd, true, out output, Settings.Instance.GitPath, "gui", "blame", name);
            }
        }
    }
}
