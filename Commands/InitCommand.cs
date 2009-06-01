using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace GitMenu.Commands
{
    public class InitCommand : GitCommand
    {
        public InitCommand(IServiceProvider provider)
            : base(provider, PkgCmdIDList.cmdGitInit, CommandFlags.NoRepository, "Git I&nit Here")
        {
        }

        protected override void OnExecute()
        {
            string file = GitCommand.GetSelectedPath();
            string wd = WDFromPath(file);
            Exec(wd, true, Settings.Instance.GitPath, "init");
        }
    }
}
