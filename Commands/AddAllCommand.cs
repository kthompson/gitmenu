using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;

namespace GitMenu.Commands
{
    public class AddAllCommand : GitCommand
    {
        public AddAllCommand(IServiceProvider provider)
            : base(provider, PkgCmdIDList.cmdGitAddAll, CommandFlags.Repository, "Git &Add all files now")
        {
            
        }

        protected override void OnExecute()
        {
            string file = GitCommand.GetSelectedPath();
            string wd = Helper.WorkingDirectoryFromPath(file);
            Helper.Exec(wd, true, Settings.Instance.GitPath, "add", "--all");
        }
    }
}
