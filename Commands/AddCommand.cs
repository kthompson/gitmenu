using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;

namespace GitMenu.Commands
{
    public class AddCommand : GitCommand
    {
        public AddCommand(GitMenuPackage provider)
            : base(provider, PkgCmdIDList.cmdGitAdd, CommandFlags.Repository, "&Add file now")
        {
            
        }

        protected override void OnExecute()
        {
            var items = GitCommand.SelectedProjectItems.ToArray();
            foreach (var item in items)
                AddItem(item);
        }

        private static void AddItem(EnvDTE.ProjectItem item)
        {
            var file = item.GetFullPath();
            string wd = Helper.WorkingDirectoryFromPath(file);
            string name = file.Substring(wd.Length + 1);
            Helper.Exec(wd, true, Settings.Instance.GitPath, "add", name);
            for (var i = 1; i <= item.ProjectItems.Count; i++)
                AddItem(item.ProjectItems.Item(i));
        }
    }
}
