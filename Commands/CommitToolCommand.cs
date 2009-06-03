using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace GitMenu.Commands
{
    public class CommitToolCommand : GitCommand
    {
        public CommitToolCommand(IServiceProvider provider)
            : base(provider, PkgCmdIDList.cmdGitCommitTool, CommandFlags.Repository, "Git &Commit Tool")
        {

        }

        protected override void OnExecute()
        {
            string file = GitCommand.GetSelectedPath();
            string wd = Helper.WorkingDirectoryFromPath(file);
            Helper.Exec(wd, true, Settings.Instance.GitPath, "citool");
        }
    }
}
