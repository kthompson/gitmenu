using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace GitMenu.Commands
{
    [Guid("55d8098f-2b1b-4a0f-a0d2-ee6f17d6ee8f")]
    public class GitInitCommand : GitCommand
    {
        public GitInitCommand()
            : base(PkgCmdIDList.cmdGitInit, CommandFlags.NoRepository, "Git I&nit Here")
        {
        }
    }
}
