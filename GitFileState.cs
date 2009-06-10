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

        public bool IsChanged { get; set; }
        public bool IsUpdated { get; set; }
        public bool IsUntracked { get; set; }
        
    }
}
