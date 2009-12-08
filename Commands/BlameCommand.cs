using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GitMenu.Commands
{
    public class BlameCommand : GitCommand
    {
        public BlameCommand(GitMenuPackage provider)
            : base(provider, PkgCmdIdList.CmdGitBlame, CommandFlags.Track | CommandFlags.File, "Git &Blame")
        {

        }

        protected override void OnExecute()
        {
            var file = GetSelectedPath();
            bool isDir;
            var wd = Helper.WorkingDirectoryFromPath(file, out isDir);
            string output;
            
            if (isDir) return;
            var name = file.Substring(wd.Length + 1);
            Helper.Exec(wd, true, out output, Settings.Instance.GitPath, "gui", "blame", name);
        }
    }
}
