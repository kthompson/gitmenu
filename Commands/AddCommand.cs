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
            var names = new List<string>();
            foreach (var item in items)
                if(item != null)
                    AddItem(item, names);

            var projects = GitCommand.SelectedProjects.ToArray();
            foreach (var item in projects)
                if (item != null)
                    AddItem(item, names);
            //TODO: this does not currently update project glyphs
            GitCommand.Package.GitSccProvider.RefreshGlyphs(names);

        }
        private static void AddItem(EnvDTE.Project item, List<string> names)
        {
            var file = item.FileName;
            names.Add(file);
            string wd = Helper.WorkingDirectoryFromPath(file);
            string name = file.Substring(wd.Length + 1);
            Helper.Exec(wd, true, Settings.Instance.GitPath, "add", name);            
        }

        private static void AddItem(EnvDTE.ProjectItem item, List<string> names)
        {
            var file = item.GetFullPath();
            names.Add(file);
            string wd = Helper.WorkingDirectoryFromPath(file);
            string name = file.Substring(wd.Length + 1);
            Helper.Exec(wd, true, Settings.Instance.GitPath, "add", name);
            for (var i = 1; i <= item.ProjectItems.Count; i++)
                AddItem(item.ProjectItems.Item(i), names);
        }
    }
}
