// Guids.cs
// MUST match guids.h
using System;

namespace GitMenu
{
    static class GuidList
    {
        public const string guidGitMenuPkgString = "9ac2f8ae-1b97-4093-b322-a8e5e6fb54fb";
        public const string guidGitMenuCmdSetString = "6f99f062-a97e-4e28-88ef-5413ad15229a";
        
        public static readonly Guid guidGitMenuCmdSet = new Guid(guidGitMenuCmdSetString);
    };
}