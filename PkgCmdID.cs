// PkgCmdID.cs
// MUST match PkgCmdID.h
using System;

namespace GitMenu
{
    static class PkgCmdIdList
    {
        public const uint GitMenu = 0x1020;
        
        public const int CmdGitCommitTool = 0x1010;
        public const int CmdGitAdd = 0x1030;
        public const int CmdGitInit = 0x1040;
        public const int CmdGitHistory = 0x1050;
        public const int CmdGitBlame = 0x1060;
        public const int CmdGitBash = 0x1070;
        public const int CmdGitGui = 0x1080;
        public const int CmdGitOpenProject = 0x1020;
    };
}