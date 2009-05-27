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
            var mcs = this.ServiceProvider.GetService<IMenuCommandService, OleMenuCommandService>();
            mcs.AddCommand(new Commands.AddAllCommand(this.ServiceProvider));
            mcs.AddCommand(new Commands.GitCommitToolCommand(this.ServiceProvider));
            mcs.AddCommand(new Commands.GitInitCommand(this.ServiceProvider));
            mcs.AddCommand(new Commands.GitBlameCommand(this.ServiceProvider));
            mcs.AddCommand(new Commands.GitBashCommand(this.ServiceProvider));
            mcs.AddCommand(new Commands.GitGuiCommand(this.ServiceProvider));
            mcs.AddCommand(new Commands.GitHistoryCommand(this.ServiceProvider));
        }
    }
}
