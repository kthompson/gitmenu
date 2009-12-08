using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GitMenu.Commands
{
    public class GitBashCommand : GitCommand
    {
        public GitBashCommand(GitMenuPackage provider)
            : base(provider, PkgCmdIdList.CmdGitBash, CommandFlags.Always, "Git Ba&sh")
        {

        }

        protected override void OnExecute()
        {
            var file = GetSelectedPath();
            var wd = Helper.WorkingDirectoryFromPath(file);
            Helper.Exec(wd, false, Settings.Instance.ShPath, "--login", "-i");
        }
    }
}
