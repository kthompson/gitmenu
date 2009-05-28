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

        public BaseCommand(CommandID id, string text)
            : base(OnExecute, id)
        {
            this.BeforeQueryStatus += new EventHandler(OnBeforeQueryStatus);
            this.Text = text;
        }

        public BaseCommand(CommandID id)
            : this(id, string.Empty)
        {
        }

        protected virtual void OnBeforeQueryStatus(object sender, EventArgs e)
        {
            OleMenuCommand command = sender as OleMenuCommand;
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
