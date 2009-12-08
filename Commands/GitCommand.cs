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
using Process = System.Diagnostics.Process;

namespace GitMenu.Commands
{
    public abstract class GitCommand : BaseCommand
    {
        public CommandFlags Selection { get; private set; }

        protected GitCommand(GitMenuPackage provider, int id, CommandFlags selection, string text)
            : base(new CommandID(GuidList.GuidGitMenuCmdSet, id), text)
        {
            this.Selection = selection;
            Package = provider;
        }

        protected GitCommand(GitMenuPackage provider, CommandID id, CommandFlags selection, string text)
            : base(id, text)
        {
            this.Selection = selection;
            Package = provider;
        }

        protected override bool CanExecute()
        {
            if (base.CanExecute())
            {
                if (this.Selection == CommandFlags.Always)
                    return true;

                if ((this.Selection & GetMenuMask()) == this.Selection)
                    return true;
            }

            return false;
        }

        private static CommandFlags _lastMenuMask;
        private static string _lastCheckedPath;
        public static CommandFlags GetMenuMask()
        {
            var pathToCheck = GetSelectedPath();

            if (pathToCheck == _lastCheckedPath)
                return _lastMenuMask;

            _lastCheckedPath = pathToCheck;
            _lastMenuMask = GetMenuMask(_lastCheckedPath);
            return _lastMenuMask;
        }

        protected static string GetSelectedPath()
        {
            var item = SelectedProjectItem;
            if (item != null)
                return item.GetFullPath();

            var item2 = SelectedProject;
            if (item2 != null)
            {
                return Directory.GetParent(item2.FileName).FullName;
            }

            var solution = GitMenuPackage.GitSccProvider.GetSolutionFileName();
            return solution;
        }

        public static CommandFlags GetMenuMask(string path)
        {
            bool isDirectory;
            var wd = Helper.WorkingDirectoryFromPath(path, out isDirectory);

            var selection = isDirectory ? CommandFlags.Directory : CommandFlags.File;
            string output;

            var status = Helper.Exec(wd, true, out output, Settings.Instance.GitPath, "rev-parse", "--show-prefix");

            var eol = output.IndexOf('\n');
            var line = output;

            if(eol >= 0)
                line = output.Substring(0, eol);

            if (status < 0)
                selection = CommandFlags.Last;
            else if (status > 0)
                selection |= CommandFlags.NoRepository;
            else
            {
                var headPath = "HEAD";
                if (!isDirectory)
                    headPath = string.Format("HEAD:{0}{1}", line, path.Substring(wd.Length + 1));

                status = Helper.Exec(wd, true, Settings.Instance.GitPath, "rev-parse", "--verify", headPath);
                if (status < 0)
                    selection = CommandFlags.Last;
                else
                    selection |= CommandFlags.Repository | (status > 0 ? CommandFlags.NoTrack : CommandFlags.Track);
            }

            return selection;
        }

        

        public static bool HasGitRepository(string path)
        {
            throw new NotImplementedException();
        }

        #region static properties

        protected static IEnumerable<SelectedItem> SelectedItems
        {
            get
            {
                return Dte.SelectedItems.Cast<SelectedItem>();
            }
        }

        protected static IEnumerable<ProjectItem> SelectedProjectItems
        {
            get
            {
                return SelectedItems.Select(item => item.ProjectItem);
            }
        }

        protected static IEnumerable<Project> SelectedProjects
        {
            get
            {
                return SelectedItems.Select(item => item.Project);
            }
        }

        protected static ProjectItem SelectedProjectItem
        {
            get
            {
                return SelectedProjectItems.FirstOrDefault();
            }
        }

        protected static Project SelectedProject
        {
            get
            {
                return SelectedProjects.FirstOrDefault();
            }
        }

        public static GitMenuPackage Package { get; private set; }

        private static DTE _dte;
        protected static DTE Dte
        {
            get
            {
                if (_dte == null)
                    _dte = Package.GetService<DTE>();

                return _dte;
            }
        }

        private static GitMenuPackage _gitMenuPackage;
        protected static GitMenuPackage GitMenuPackage
        {
            get
            {
                if (_gitMenuPackage == null)
                    _gitMenuPackage = Package.GetService<GitMenuPackage>();

                return _gitMenuPackage;
            }
        }
        #endregion
    }

    [Flags]
    public enum CommandFlags
    {
        Last            = -1,
        Always          = 0,
        File            = (1 << 0),
        Directory       = (1 << 1),
        NoRepository    = (1 << 2),
        Repository      = (1 << 3),
        Track           = (1 << 4),
        NoTrack         = (1 << 5),
    }
}
