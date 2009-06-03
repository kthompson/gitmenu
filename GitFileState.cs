using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GitMenu
{
    public class GitFileState
    {
        public GitFileState(string filename)
        {
            this.FileName = filename;
        }

        public string FileName { get; private set; }

        public bool IsCached { get; private set; }
        public bool IsUnmerged { get; private set; }
        public bool IsRemoved { get; private set; }
        public bool IsModified { get; private set; }
        public bool IsToBeKilled { get; private set; }
        public bool IsOther { get; private set; }
        public bool IsTracked { get; private set; }
        public bool IsUntracked { get { return !this.IsTracked; } }

        private void AddStateValue(string state)
        {
            switch (state)
            {
                case "H": //cached
                    this.IsCached = true;
                    return;
                case "M": //unmerged
                    this.IsUnmerged = true;
                    return;
                case "C": //modified
                    this.IsModified = true;
                    return;
                case "R": //removed
                    this.IsRemoved = true;
                    return;
                case "K": //to be killed
                    this.IsToBeKilled = true;
                    return;
                case "?": //other
                    this.IsOther = true;
                    return;
                default:
                    break;
            }
        }

        public static List<GitFileState> GetLsFiles(string file)
        {
            bool isDir;
            string wd = Helper.WorkingDirectoryFromPath(file, out isDir);
            string output;
            //git ls-files -t --full-name -domc -X .gitignore
            
            string name = file;
            if (!isDir)
                name = file.Substring(wd.Length + 1);

            var exec = Helper.Exec(wd, true, out output, Settings.Instance.GitPath, "ls-files", "-t", "-domc", "-X", ".gitignore", name);
            if(exec > 0)
                exec = Helper.Exec(wd, true, out output, Settings.Instance.GitPath, "ls-files", "-t", "-domc", name);

            var files = new Dictionary<string, GitFileState>();

            using (var sr = new StringReader(output))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    var lsfile = line.Split(new char[] {' '}, 2);
                    var filename = lsfile[1];
                    var state = lsfile[0];
                    if (!files.ContainsKey(filename))
                        files.Add(filename, new GitFileState(filename));

                    files[filename].AddStateValue(state);
                }
            }

            //git ls-tree --name-only HEAD
            Helper.Exec(wd, true, out output, Settings.Instance.GitPath, "ls-tree", "--name-only", "HEAD", name);

            using (var sr = new StringReader(output))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (files.ContainsKey(line))
                        files[line].IsTracked = true;
                }
            }

            return files.Values.ToList();
        }
    }
}
