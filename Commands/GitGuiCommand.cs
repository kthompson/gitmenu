using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GitMenu.Commands
{
    public class GitGuiCommand : GitCommand
    {
        public GitGuiCommand(GitMenuPackage provider)
            : base(provider, PkgCmdIdList.CmdGitGui, CommandFlags.Always, "Git &Gui")
        {
        }

        protected override void OnExecute()
        {
            var file = GetSelectedPath();
            var wd = Helper.WorkingDirectoryFromPath(file);
            Helper.Exec(wd, true, Settings.Instance.GitPath, "gui");
        }
    }
}
