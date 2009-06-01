using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Design;

namespace GitMenu.Commands
{
    public class OpenGitProject : GitCommand
    {
        public OpenGitProject(IServiceProvider provider)
            : base(provider, new CommandID(GuidList.guidGitMenuFileOpen, PkgCmdIDList.cmdGitOpenProject), CommandFlags.NoRepository, "&Git Project...")
        {
            
        }

        protected override bool CanExecute()
        {
            return true;
        }

        protected override void OnExecute()
        {
            //TODO: implement

        }
    }
}
