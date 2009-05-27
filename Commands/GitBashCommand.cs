using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GitMenu.Commands
{
    public class GitBashCommand : GitCommand
    {
        public GitBashCommand(IServiceProvider provider)
            : base(provider, PkgCmdIDList.cmdGitBash, CommandFlags.Always, "Git Ba&sh")
        {

        }
    }
}
