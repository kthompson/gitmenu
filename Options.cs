using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Microsoft.VisualStudio.Shell;
using System.IO;
using System.Windows.Forms.Design;
using System.Drawing.Design;
using System.Runtime.InteropServices;

namespace GitMenu
{
    [Guid("148422C7-79D9-4d4d-BE46-66F9BA0091EA")]
    public class Options : DialogPage
    {

        public override object AutomationObject
        {
            get
            {
                return Settings.Instance;
            }
        }

    }
}
