using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;

namespace GitMenu
{
    public class CommandSet
    {
        public IServiceProvider ServiceProvider { get; private set; }

        public CommandSet(IServiceProvider provider)
        {
            this.ServiceProvider = provider;
        }

        public void Initialize()
        {
            var menuCommandService = this.ServiceProvider.GetService<IMenuCommandService, OleMenuCommandService>();
            menuCommandService.AddCommand(new Commands.AddAllCommand());
            menuCommandService.AddCommand(new Commands.GitCommitToolCommand());
            menuCommandService.AddCommand(new Commands.GitInitCommand());
        }
    }
}
