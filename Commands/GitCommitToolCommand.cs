using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace GitMenu.Commands
{
    [Guid("94c5d3c5-4b82-455f-a7bf-ee7b7efa5398")]
    public class GitCommitToolCommand : GitCommand
    {
        public GitCommitToolCommand()
            : base(PkgCmdIDList.cmdGitCommitTool, CommandFlags.Repository, "Git &Commit Tool")
        {

        }
    }
}
