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
            mcs.AddCommand(new Commands.AddCommand(this.ServiceProvider));
            mcs.AddCommand(new Commands.CommitToolCommand(this.ServiceProvider));
            mcs.AddCommand(new Commands.InitCommand(this.ServiceProvider));
            mcs.AddCommand(new Commands.BlameCommand(this.ServiceProvider));
            mcs.AddCommand(new Commands.GitBashCommand(this.ServiceProvider));
            mcs.AddCommand(new Commands.GitGuiCommand(this.ServiceProvider));
            mcs.AddCommand(new Commands.HistoryCommand(this.ServiceProvider));
            mcs.AddCommand(new Commands.OpenGitProject(this.ServiceProvider));
        }
    }
}
