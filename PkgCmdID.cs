// PkgCmdID.cs
// MUST match PkgCmdID.h
using System;

namespace GitMenu
{
    static class PkgCmdIDList
    {
        public const uint GitMenu = 0x1020;
        
        public const int cmdGitCommitTool = 0x1010;
        public const int cmdGitAdd = 0x1030;
        public const int cmdGitInit = 0x1040;
        public const int cmdGitHistory = 0x1050;
        public const int cmdGitBlame = 0x1060;
        public const int cmdGitBash = 0x1070;
        public const int cmdGitGui = 0x1080;
        public const int cmdGitOpenProject = 0x1020;
    };
}