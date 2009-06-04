using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GitMenu.Commands
{
    public class GitBashCommand : GitCommand
    {
        public GitBashCommand(GitMenuPackage provider)
            : base(provider, PkgCmdIDList.cmdGitBash, CommandFlags.Always, "Git Ba&sh")
        {

        }

        protected override void OnExecute()
        {
            string file = GitCommand.GetSelectedPath();
            string wd = Helper.WorkingDirectoryFromPath(file);
            Helper.Exec(wd, false, Settings.Instance.ShPath, "--login", "-i");
        }
    }
}
