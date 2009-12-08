using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace GitMenu.Commands
{
    public class CommitToolCommand : GitCommand
    {
        public CommitToolCommand(GitMenuPackage provider)
            : base(provider, PkgCmdIdList.CmdGitCommitTool, CommandFlags.Repository, "Git &Commit Tool")
        {

        }

        protected override void OnExecute()
        {
            var file = GetSelectedPath();
            var wd = Helper.WorkingDirectoryFromPath(file);
            Helper.Exec(wd, true, Settings.Instance.GitPath, "citool");
            Package.GitSccProvider.RefreshAllGlyphs();
        }
    }
}
