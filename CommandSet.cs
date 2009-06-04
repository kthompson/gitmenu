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
        public GitMenuPackage Package { get; private set; }

        public CommandSet(GitMenuPackage provider)
        {
            this.Package = provider;
        }

        public void Initialize()
        {
            var mcs = this.Package.GetService<IMenuCommandService, OleMenuCommandService>();
            mcs.AddCommand(new Commands.AddCommand(this.Package));
            mcs.AddCommand(new Commands.CommitToolCommand(this.Package));
            mcs.AddCommand(new Commands.InitCommand(this.Package));
            mcs.AddCommand(new Commands.BlameCommand(this.Package));
            mcs.AddCommand(new Commands.GitBashCommand(this.Package));
            mcs.AddCommand(new Commands.GitGuiCommand(this.Package));
            mcs.AddCommand(new Commands.HistoryCommand(this.Package));
            mcs.AddCommand(new Commands.OpenGitProject(this.Package));
        }
    }
}
