using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using System.Windows.Forms;

namespace GitMenu.Services
{
    [Guid(GuidList.guidGitSccProviderString)]
    public class GitSccProvider : 
        IVsSccProvider,
        IVsSccManager2,
        IVsSccGlyphs
    {
        public GitMenuPackage Package { get; private set; }

        public GitSccProvider(GitMenuPackage package)
        {
            this.Package = package;
        }

        public bool Active { get; private set; }

        #region IVsSccProvider Members

        public int AnyItemsUnderSourceControl(out int pfResult)
        {
            // Set pfResult to false when the solution can change to an other scc provider
            pfResult = 1;
            return VSConstants.S_OK;
        }

        public int SetActive()
        {
            if (!this.Active)
            {
                this.Active = true;
            }

            return VSConstants.S_OK;
        }

        public int SetInactive()
        {
            if (this.Active)
            {
                this.Active = false;
            }

            return VSConstants.S_OK;
        }

        #endregion

        #region IVsSccManager2 Members

        public int BrowseForProject(out string pbstrDirectory, out int pfOK)
        {
            pbstrDirectory = null;
            pfOK = 0;

            return VSConstants.E_NOTIMPL;
        }

        public int CancelAfterBrowseForProject()
        {
            return VSConstants.E_NOTIMPL;
        }

        public int GetSccGlyph(int cFiles, string[] rgpszFullPaths, VsStateIcon[] rgsiGlyphs, uint[] rgdwSccStatus)
        {
            rgsiGlyphs[0] = (VsStateIcon)GetGlyphFromPath(rgpszFullPaths[0]);
            
            return VSConstants.S_OK;
        }

        public int GetSccGlyphFromStatus(uint dwSccStatus, VsStateIcon[] psiGlyph)
        {
            return VSConstants.S_OK;
        }

        public int IsInstalled(out int pbInstalled)
        {
            pbInstalled = 1;
            return VSConstants.S_OK;
        }

        public int RegisterSccProject(IVsSccProject2 pscp2Project, string pszSccProjectName, string pszSccAuxPath, string pszSccLocalPath, string pszProvider)
        {
            return VSConstants.S_OK;
        }

        public int UnregisterSccProject(IVsSccProject2 pscp2Project)
        {
            return VSConstants.S_OK;
        }

        

        #endregion

        public GitGlyph GetGlyphFromPath(string path)
        {
            var file = GitFileState.GetLsFiles(path).FirstOrDefault();
            if (file == null)
                return GitGlyph.None;

            if (file.IsUntracked)
                return GitGlyph.Untracked;

            if (file.IsModified)
                return GitGlyph.Modified;

            if (file.IsCached)
                return GitGlyph.Indexed;

            return GitGlyph.Normal;

        }

        #region IVsSccGlyphs Members

        uint _baseIndex;
        ImageList _glyphList;
        public int GetCustomGlyphList(uint BaseIndex, out uint pdwImageListHandle)
        {
            try
            {
                if (_glyphList != null && BaseIndex != _baseIndex)
                {
                    _glyphList.Dispose();
                    _glyphList = null;
                }

                // We give VS all our custom glyphs from baseindex upwards
                if (_glyphList == null)
                {
                    _baseIndex = BaseIndex;
                    _glyphList = StatusIcons.CreateStatusImageList();
                    for (int i = (int)BaseIndex - 1; i >= 0; i--)
                    {
                        _glyphList.Images.RemoveAt(i);
                    }
                }
                pdwImageListHandle = unchecked((uint)_glyphList.Handle);

                return VSConstants.S_OK;
            }
            catch (Exception e)
            {
                pdwImageListHandle = 0;
                return VSConstants.S_FALSE;
            }
        }

        #endregion
    }
}
