using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Shell;
using System.ComponentModel.Design;

namespace GitMenu.Commands
{
    public abstract class BaseCommand : OleMenuCommand
    {
        protected BaseCommand(CommandID id, string text)
            : base(OnExecute, id)
        {
            this.BeforeQueryStatus += OnBeforeQueryStatus;
            this.Text = text;
        }

        protected BaseCommand(CommandID id)
            : this(id, string.Empty)
        {
        }

        protected virtual void OnBeforeQueryStatus(object sender, EventArgs e)
        {
            var command = sender as OleMenuCommand;
            if (command == null)
                return;

            command.Visible = this.CanExecute();
        }

        protected virtual bool CanExecute()
        {
            return true;
        }

        protected virtual void OnExecute()
        {
        }

        private static void OnExecute(object sender, EventArgs e)
        {
            var menu = sender as BaseCommand;
            if (menu == null) return;
            menu.OnExecute();
        }
    }
}
