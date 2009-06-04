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

        public bool IsChanged { get; private set; }
        public bool IsUpdated { get; private set; }
        public bool IsUntracked { get; private set; }

        public static List<GitFileState> GetStatus(string file)
        {
            if (file == null)
                return new List<GitFileState>();

            bool isDir;
            string wd = Helper.WorkingDirectoryFromPath(file, out isDir);
            string output;

            var files = new Dictionary<string, GitFileState>();
            
            string name = file;
            if (!isDir)
                name = file.Substring(wd.Length + 1);

            //shows changed files
            //git diff-files --name-status 
            Helper.Exec(wd, true, out output, Settings.Instance.GitPath, "diff-files", "--name-status", name);
            UpdateFiles(output, files, f => f.IsChanged = true);

            
            //added to cache/index
            //git diff-index --name-status --cached HEAD
            Helper.Exec(wd, true, out output, Settings.Instance.GitPath, "diff-index", "--name-status", "--cached", "HEAD", name);
            UpdateFiles(output, files, f => f.IsUpdated = true);


            //untracked files
            //git ls-files -t -o -X .gitignore
            var exec = Helper.Exec(wd, true, out output, Settings.Instance.GitPath, "ls-files", "-t", "-o", "-X", ".gitignore", name);
            if (exec > 0)
                exec = Helper.Exec(wd, true, out output, Settings.Instance.GitPath, "ls-files", "-t", "-o", name);

            UpdateFiles(output, files, f => f.IsUntracked = true);

            return files.Values.ToList();
        }

        private static void UpdateFiles(string output, Dictionary<string, GitFileState> files, Action<GitFileState> action)
        {
            using (var sr = new StringReader(output))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    var lsfile = line.Split(new char[] { ' ', '\t' }, 2);
                    var filename = lsfile[1];
                    var state = lsfile[0];
                    if (!files.ContainsKey(filename))
                        files.Add(filename, new GitFileState(filename));
                    
                    action(files[filename]);                    
                }
            }
        }
    }
}
