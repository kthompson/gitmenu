using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using EnvDTE;
using EnvDTE80;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace GitMenu.Commands
{
    public abstract class GitCommand : BaseCommand
    {
        public CommandFlags Selection { get; private set; }

        public GitCommand(IServiceProvider provider, int id, CommandFlags selection, string text)
            : base(new CommandID(GuidList.guidGitMenuCmdSet, id), text)
        {
            this.Selection = selection;
            GitCommand.ServiceProvider = provider;
        }

        protected override bool CanExecute()
        {
            if (base.CanExecute())
            {
                ProjectItem item = GitCommand.SelectedItem;
                if (item != null)
                {
                    var props = (Properties)item.Properties;
                    var name = (from prop in props.OfType<Property>()
                               where prop.Name == "FullPath"
                               select new FileSystemInfo(prop.Value.ToString())).FirstOrDefault();
                    var isDirectory = ((name.Attributes & FileAttributes.Directory) == FileAttributes.Directory);
                    Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Testing for execution of {0} on item: {1}", this.GetType().ToString(), item.Name));
                    return true;
                }
            }

            return false;
        }



        #region static properties

        protected static IEnumerable<ProjectItem> SelectedItems
        {
            get
            {
                return GitCommand.Dte.SelectedItems.OfType<ProjectItem>();
            }
        }

        protected static ProjectItem SelectedItem
        {
            get
            {
                return GitCommand.SelectedItems.FirstOrDefault();
            }
        }

        public static IServiceProvider ServiceProvider { get; private set; }

        private static DTE dte;
        protected static DTE Dte
        {
            get
            {
                if (dte == null)
                    dte = ServiceProvider.GetService<DTE>();

                return dte;
            }
        }

        private static GitMenuPackage gitMenuPackage;
        protected static GitMenuPackage GitMenuPackage
        {
            get
            {
                if (gitMenuPackage == null)
                    gitMenuPackage = ServiceProvider.GetService<GitMenuPackage>();

                return gitMenuPackage;
            }
        }
        #endregion
    }

    [Flags]
    public enum CommandFlags
    {
        Always = 0,
        File = (1 << 0),
        Directory = (1 << 1),
        NoRepository = (1 << 2),
        Repository = (1 << 3),
        Track = (1 << 4),
        NoTrack = (1 << 5),
    }
}
