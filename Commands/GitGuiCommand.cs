using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GitMenu.Commands
{
    public class GitGuiCommand : GitCommand
    {
        public GitGuiCommand(IServiceProvider provider)
            : base(provider, PkgCmdIDList.cmdGitGui, CommandFlags.Always, "Git &Gui")
        {
        }

        protected override void OnExecute()
        {
            string file = GitCommand.GetSelectedPath();
            string wd = WDFromPath(file);
            Exec(wd, true, Settings.Instance.GitPath, "gui");
        }
    }
}
