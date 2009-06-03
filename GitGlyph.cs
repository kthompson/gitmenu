using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GitMenu
{
    public enum GitGlyph
    {
        None = 0,
        STATEICON_CHECKEDIN = 1,
        STATEICON_CHECKEDOUT = 2,
        STATEICON_ORPHANED = 3,
        STATEICON_EDITABLE = 4,
        STATEICON_BLANK = 5,
        STATEICON_READONLY = 6,
        STATEICON_DISABLED = 7,
        STATEICON_CHECKEDOUTEXCLUSIVE = 8,
        STATEICON_CHECKEDOUTSHAREDOTHER = 9,
        STATEICON_CHECKEDOUTEXCLUSIVEOTHER = 10,
        STATEICON_EXCLUDEDFROMSCC = 11,
        Untracked = 12,
        Modified = 13,
        Indexed = 14,
        Normal = 15,
    }
}
