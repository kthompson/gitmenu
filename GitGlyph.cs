using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GitMenu
{
    public enum GitGlyph
    {
        None = 0,
        StateiconCheckedin = 1,
        StateiconCheckedout = 2,
        StateiconOrphaned = 3,
        StateiconEditable = 4,
        StateiconBlank = 5,
        StateiconReadonly = 6,
        StateiconDisabled = 7,
        StateiconCheckedoutexclusive = 8,
        StateiconCheckedoutsharedother = 9,
        StateiconCheckedoutexclusiveother = 10,
        StateiconExcludedfromscc = 11,
        Untracked = 12,
        Changed = 13,
        Updated = 14,
        Commited = 15,
    }
}
