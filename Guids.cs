// Guids.cs
// MUST match guids.h
using System;

namespace GitMenu
{
    static class GuidList
    {
        public const string guidGitMenuPkgString = "9ac2f8ae-1b97-4093-b322-a8e5e6fb54fb";
        public const string guidGitMenuCmdSetString = "6f99f062-a97e-4e28-88ef-5413ad15229a";
        public const string guidGitMenuFileOpenString = "D9580075-D9E3-4bf2-A7A0-3EDD0943519A";
        
        public static readonly Guid guidGitMenuCmdSet = new Guid(guidGitMenuCmdSetString);
        public static readonly Guid guidGitMenuFileOpen = new Guid(guidGitMenuFileOpenString);
    };
}