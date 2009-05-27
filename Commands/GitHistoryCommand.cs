using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GitMenu.Commands
{
    public class GitHistoryCommand : GitCommand
    {
        public GitHistoryCommand(IServiceProvider provider)
            : base(provider, PkgCmdIDList.cmdGitHistory, CommandFlags.Track, "Git &History")
        {

        }
    }
}
