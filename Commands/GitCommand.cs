using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Design;

namespace GitMenu.Commands
{
    public abstract class GitCommand : BaseCommand
    {
        public CommandFlags Selection { get; private set; }

        public GitCommand(int id, CommandFlags selection, string text)
            : base(id, text)
        {
            this.Selection = selection;
        }
    }

    [Flags]
    public enum CommandFlags
    {
        Always = 0,
        File = (1 << 0),
        Directory = (1 << 1),
        NoRepository = (1 << 2),
        Repository = (1 << 3),
        Track = (1 << 4),
        NoTrack = (1 << 5),
    }
}
