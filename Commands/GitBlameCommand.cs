using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GitMenu.Commands
{
    public class GitBlameCommand : GitCommand
    {
        public GitBlameCommand(IServiceProvider provider)
            : base(provider, PkgCmdIDList.cmdGitBlame, CommandFlags.Track | CommandFlags.File, "Git &Blame")
        {

        }

        protected override void OnExecute()
        {
            string file = GitCommand.SelectedItem.GetFullPath();
            bool isDir;
            string wd = WDFromPath(file, out isDir);
            string name = "";
            string output;
            if (!isDir)
            {
                name = file.Substring(wd.Length + 1);

                Exec(wd, true, out output, Settings.Instance.GitPath, "gui", "blame", name);
            }
        }
    }
}
