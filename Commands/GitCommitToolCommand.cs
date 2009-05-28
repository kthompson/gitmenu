using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace GitMenu.Commands
{
    public class GitCommitToolCommand : GitCommand
    {
        public GitCommitToolCommand(IServiceProvider provider)
            : base(provider, PkgCmdIDList.cmdGitCommitTool, CommandFlags.Repository, "Git &Commit Tool")
        {

        }

        protected override void OnExecute()
        {
            string file = GitCommand.SelectedItem.GetFullPath();
            string wd = WDFromPath(file);
            Exec(wd, true, Settings.Instance.GitPath, "citool");
        }
    }
}
