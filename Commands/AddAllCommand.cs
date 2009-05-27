using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;

namespace GitMenu.Commands
{
    [Guid("9f688e41-bb98-497f-9ef6-947f68e81693")]
    public class AddAllCommand : GitCommand
    {
        public AddAllCommand()
            : base(PkgCmdIDList.cmdGitAddAll, CommandFlags.Repository, "Git &Add all files now")
        {
            
        }
    }
}
