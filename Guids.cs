// Guids.cs
// MUST match guids.h
using System;

namespace GitMenu
{
    static class GuidList
    {
        public const string GuidGitMenuPkgString = "9ac2f8ae-1b97-4093-b322-a8e5e6fb54fb";
        public const string GuidGitSccProviderString = "C59BE8E4-F369-419e-9343-C23698E077C6";

        public const string GuidGitMenuCmdSetString = "6f99f062-a97e-4e28-88ef-5413ad15229a";
        public const string GuidGitMenuFileOpenString = "D9580075-D9E3-4bf2-A7A0-3EDD0943519A";

        public static readonly Guid GuidGitMenuPkg = new Guid(GuidGitMenuPkgString);
        public static readonly Guid GuidGitSccProvider = new Guid(GuidGitSccProviderString);

        public static readonly Guid GuidGitMenuCmdSet = new Guid(GuidGitMenuCmdSetString);
        public static readonly Guid GuidGitMenuFileOpen = new Guid(GuidGitMenuFileOpenString);
    };
}