using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GitMenu.Commands
{
    public class GitBlameCommand : GitCommand
    {
        public GitBlameCommand(IServiceProvider provider)
            : base(provider, PkgCmdIDList.cmdGitBlame, CommandFlags.Track | CommandFlags.File, "Git &Blame")
        {

        }
    }
}
