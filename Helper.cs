using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace GitMenu
{
    public static class Helper
    {
        public static int Exec(string wd, bool hidden, params string[] args)
        {
            string output;
            return Exec(wd, hidden, out output, args);
        }

        public static int Exec(string wd, bool hidden, out string output, params string[] args)
        {
            output = string.Empty;
            var start = new ProcessStartInfo
            {
                FileName = args.First(),
                Arguments = string.Join(" ", args.Skip(1).ToArray()),
                WindowStyle = hidden ? ProcessWindowStyle.Hidden : ProcessWindowStyle.Normal,
                CreateNoWindow = hidden,
                RedirectStandardOutput = hidden,
                WorkingDirectory = wd,
                UseShellExecute = false,
            };
            try
            {
                using (var proc = Process.Start(start))
                {
                    if (hidden)
                    {
                        output = proc.StandardOutput.ReadToEnd();
                        proc.WaitForExit();
                        return proc.ExitCode;
                    }
                    return 0;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                throw;
            }
        }

        public static string WorkingDirectoryFromPath(string path)
        {
            bool isDirectory;
            return WorkingDirectoryFromPath(path, out isDirectory);
        }

        /// <summary>
        /// Gets Working Directory from a path.
        /// </summary>
        /// <param name="path">The path to a file or folder.</param>
        /// <param name="isDirectory">if set to <c>true</c> path is a directory.</param>
        /// <returns></returns>
        public static string WorkingDirectoryFromPath(string path, out bool isDirectory)
        {
            isDirectory = Directory.Exists(path);

            if (!isDirectory)
                path = path.Substring(0, path.LastIndexOf(Path.DirectorySeparatorChar));

            return path;
        }
    }
}
