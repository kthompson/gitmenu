using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace GitMenu.Commands
{
    public class InitCommand : GitCommand
    {
        public InitCommand(GitMenuPackage provider)
            : base(provider, PkgCmdIdList.CmdGitInit, CommandFlags.NoRepository, "Git I&nit Here")
        {
        }

        protected override void OnExecute()
        {
            var file = GetSelectedPath();
            var wd = Helper.WorkingDirectoryFromPath(file);
            Helper.Exec(wd, true, Settings.Instance.GitPath, "init");
        }
    }
}
