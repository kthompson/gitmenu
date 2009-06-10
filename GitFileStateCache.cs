using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace GitMenu
{
    public class GitFileStateCache
    {
        private Dictionary<string, GitFileState> _files;

        private GitFileStateCache()
        {
            _files = new Dictionary<string, GitFileState>();
            allowUpdates = true;
        }

        private bool allowUpdates;

        #region Instance Class

        private class InstanceClass
        {
            static InstanceClass()
            {
                HasInstance = true;
            }

            public static readonly GitFileStateCache Instance = new GitFileStateCache();
        }

        public static bool HasInstance { get; private set; }

        public static GitFileStateCache Instance
        {
            get { return InstanceClass.Instance; }
        }

        #endregion


        public void Update(string file)
        {
            if (string.IsNullOrEmpty(file))
                return;

            EnsureFile(file);

            if (!allowUpdates)
                return;

            bool isDir;
            string wd = Helper.WorkingDirectoryFromPath(file, out isDir);
            string output;

            string name = string.Empty;
            

            if (!isDir)
                name = file.Substring(wd.Length + 1);

            //shows changed files
            //git diff-files --name-status
            Helper.Exec(wd, true, out output, Settings.Instance.GitPath, "diff-files", "--name-status", name);
            UpdateFiles(output, wd, f => f.IsChanged = true);

            //added to cache/index
            //git diff-index --name-status --cached HEAD
            Helper.Exec(wd, true, out output, Settings.Instance.GitPath, "diff-index", "--name-status", "--cached", "HEAD", name);
            UpdateFiles(output, wd, f => f.IsUpdated = true);

            //untracked files
            //git ls-files -t -o -X .gitignore
            var exec = Helper.Exec(wd, true, out output, Settings.Instance.GitPath, "ls-files", "-t", "-o", "-X", ".gitignore", name);
            if (exec > 0)
                exec = Helper.Exec(wd, true, out output, Settings.Instance.GitPath, "ls-files", "-t", "-o", name);

            UpdateFiles(output, wd, f => f.IsUntracked = true);
        }

        private void UpdateFiles(string output, string wd, Action<GitFileState> action)
        {
            using (var sr = new StringReader(output))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    var lsfile = line.Split(new char[] { ' ', '\t' }, 2);
                    var filename = lsfile[1];
                    var state = lsfile[0];
                    var fullPath = Path.Combine(wd, filename);
                    EnsureFile(fullPath);

                    action(_files[fullPath]);
                }
            }
        }

        private void EnsureFile(string filename)
        {
            if (!_files.ContainsKey(filename))
                _files.Add(filename, new GitFileState(filename));
        }

        public GitFileState GetStatus(string path)
        {
            Update(path);

            if (_files.ContainsKey(path))
                return _files[path];
            return null;
        }

        public Lock LockUpdates()
        {
            return new Lock(this);
        }

        public class Lock : IDisposable
        {
            private GitFileStateCache cache;

            internal Lock(GitFileStateCache cache)
            {
                Monitor.Enter(cache);
                this.cache = cache;
                this.cache.allowUpdates = false;
            }

            #region IDisposable Members

            public void Dispose()
            {
                this.cache.allowUpdates = true;
                Monitor.Exit(this.cache);
            }

            #endregion
        }
    }
}
