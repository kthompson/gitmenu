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
                if (this.Selection == CommandFlags.Always)
                    return true;

                if ((this.Selection & GetMenuMask()) == this.Selection)
                    return true;
            }

            return false;
        }

        private static CommandFlags lastMenuMask;
        private static string lastCheckedPath;
        public static CommandFlags GetMenuMask()
        {
            var pathToCheck = GetSelectedPath();

            if (pathToCheck == lastCheckedPath)
                return lastMenuMask;

            lastCheckedPath = pathToCheck;
            lastMenuMask = GetMenuMask(lastCheckedPath);
            return lastMenuMask;
        }

        public static string GetSelectedPath()
        {
            var item = GitCommand.SelectedProjectItem;
            if (item != null)
                return item.GetFullPath();

            var item2 = GitCommand.SelectedProject;
            if (item2 != null)
                return Directory.GetParent(item2.FileName).FullName;

            return string.Empty;
        }

        public static string WDFromPath(string path)
        {
           bool isDirectory;
           return WDFromPath(path, out isDirectory);
        }

        /// <summary>
        /// Gets Working Directory from a path.
        /// </summary>
        /// <param name="path">The path to a file or folder.</param>
        /// <param name="isDirectory">if set to <c>true</c> path is a directory.</param>
        /// <returns></returns>
        public static string WDFromPath(string path, out bool isDirectory)
        {
            isDirectory = Directory.Exists(path);

            if (!isDirectory)
                path = path.Substring(0, path.LastIndexOf(Path.DirectorySeparatorChar));

            return path;
        }

        public static CommandFlags GetMenuMask(string path)
        {
            bool isDirectory;
            var wd = WDFromPath(path, out isDirectory);

            CommandFlags selection = isDirectory ? CommandFlags.Directory : CommandFlags.File;
            string output;

            int status = Exec(wd, true, out output, Settings.Instance.GitPath, "rev-parse","--show-prefix");

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

                status = Exec(wd, true, Settings.Instance.GitPath, "rev-parse", "--verify", headPath);
                if (status < 0)
                    selection = CommandFlags.Last;
                else
                    selection |= CommandFlags.Repository | (status > 0 ? CommandFlags.NoTrack : CommandFlags.Track);
            }

            return selection;
        }

        protected static int Exec(string wd, bool hidden, params string[] args)
        {
            string output;
            return Exec(wd, hidden, out output, args);
        }

        protected static int Exec(string wd, bool hidden, out string output, params string[] args)
        {
            var start = new ProcessStartInfo
            {
                FileName = args.First(),
                Arguments = string.Join(" ", args.Skip(1).ToArray()),
                WindowStyle = hidden ? ProcessWindowStyle.Hidden : ProcessWindowStyle.Normal,
                CreateNoWindow = hidden,
                RedirectStandardOutput = true,
                WorkingDirectory = wd,
                UseShellExecute = false,
            };
            try
            {
                using (var proc = Process.Start(start))
                {
                    output = proc.StandardOutput.ReadToEnd();

                    return proc.ExitCode;



                }
            }
            catch(Exception ex)
            {
                Trace.WriteLine(ex.Message);
                throw;
            }
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
                return GitCommand.Dte.SelectedItems.Cast<SelectedItem>();
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
                return GitCommand.SelectedProjectItems.FirstOrDefault();
            }
        }

        protected static Project SelectedProject
        {
            get
            {
                return GitCommand.SelectedProjects.FirstOrDefault();
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
