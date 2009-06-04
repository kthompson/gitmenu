using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using System.Windows.Forms;
using EnvDTE;
using System.Collections;
using Microsoft.VisualStudio.OLE.Interop;

namespace GitMenu.Services
{

    /// <summary>
    /// Identical to Microsoft.VisualStudio.Shell.Interop.__SccStatus 
    /// in Microsoft.VisualStudio.Shell.Interop.9.0
    /// </summary>
    enum SccStatus
    {
        SCC_STATUS_INVALID = -1,
        SCC_STATUS_NOTCONTROLLED = 0x0000,
        SCC_STATUS_CONTROLLED = 0x0001,
        SCC_STATUS_CHECKEDOUT = 0x0002,
        SCC_STATUS_OUTOTHER = 0x0004,
        SCC_STATUS_OUTEXCLUSIVE = 0x0008,
        SCC_STATUS_OUTMULTIPLE = 0x0010,
        SCC_STATUS_OUTOFDATE = 0x0020,
        SCC_STATUS_DELETED = 0x0040,
        SCC_STATUS_LOCKED = 0x0080,
        SCC_STATUS_MERGED = 0x0100,
        SCC_STATUS_SHARED = 0x0200,
        SCC_STATUS_PINNED = 0x0400,
        SCC_STATUS_MODIFIED = 0x0800,
        SCC_STATUS_OUTBYUSER = 0x1000,
        SCC_STATUS_NOMERGE = 0x2000,
        SCC_STATUS_RESERVED_1 = 0x4000,
        SCC_STATUS_RESERVED_2 = 0x8000
    }

    [Guid(GuidList.guidGitSccProviderString)]
    public class GitSccProvider : 
        IVsSccProvider,
        IVsSccManager2,
        IVsQueryEditQuerySave3,
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
            rgdwSccStatus[0] = (uint)SccStatus.SCC_STATUS_CONTROLLED;
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
            var file = GitFileState.GetStatus(path).FirstOrDefault();
            if (file == null)
                return GitGlyph.Commited;

            if (file.IsUntracked)
                return GitGlyph.Untracked;

            if (file.IsChanged)
                return GitGlyph.Changed;

            if (file.IsUpdated)
                return GitGlyph.Updated;

            return GitGlyph.None;

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

        /// <summary>
        /// Enumerate all hierarchy project items recursively traversing nested hierarchies.
        /// </summary>
        private List<VSITEMSELECTION> GetAllItems(Func<VSITEMSELECTION, bool> where)
        {
            //Get the solution service so we can traverse each project hierarchy contained within.
            IVsSolution solution = (IVsSolution)this.Package.GetService<SVsSolution>();
            var items = new List<VSITEMSELECTION>();
            if (null != solution)
            {
                IVsHierarchy solutionHierarchy = solution as IVsHierarchy;
                if (null != solutionHierarchy)
                {
                    
                    Action<IVsHierarchy, uint, int> action = delegate(IVsHierarchy hier, uint itemid, int recursion)
                    {
                        var item = new VSITEMSELECTION();
                        item.pHier = hier;
                        item.itemid = itemid;
                        if(where == null || where(item))
                            items.Add(item);
                    };
                    this.EnumHierarchyItems(solutionHierarchy, VSConstants.VSITEMID_ROOT, 0, true, false, action);
                }
            }
            return items;
        }

        /// <summary>
        /// Enumerates over the hierarchy items for the given hierarchy traversing into nested hierarchies.
        /// </summary>
        /// <param name="hierarchy">hierarchy to enmerate over.</param>
        /// <param name="itemid">item id of the hierarchy</param>
        /// <param name="recursionLevel">Depth of recursion. e.g. if recursion started with the Solution
        /// node, then : Level 0 -- Solution node, Level 1 -- children of Solution, etc.</param>
        /// <param name="hierIsSolution">true if hierarchy is Solution Node. This is needed to special
        /// case the children of the solution to work around a bug with VSHPROPID_FirstChild and 
        /// VSHPROPID_NextSibling implementation of the Solution.</param>
        /// <param name="visibleNodesOnly">true if only nodes visible in the Solution Explorer should
        /// be traversed. false if all project items should be traversed.</param>
        /// <param name="processNodeFunc">pointer to function that should be processed on each
        /// node as it is visited in the depth first enumeration.</param>
        //public delegate void ProcessHierarchyNode(IVsHierarchy hierarchy, uint itemid, int recursionLevel);
        private void EnumHierarchyItems(IVsHierarchy hierarchy, uint itemid, int recursionLevel, bool hierIsSolution, bool visibleNodesOnly, Action<IVsHierarchy, uint, int> processNodeFunc)
        {
            int hr;
            IntPtr nestedHierarchyObj;
            uint nestedItemId;
            Guid hierGuid = typeof(IVsHierarchy).GUID;

            // Check first if this node has a nested hierarchy. If so, then there really are two 
            // identities for this node: 1. hierarchy/itemid 2. nestedHierarchy/nestedItemId.
            // We will recurse and call EnumHierarchyItems which will display this node using
            // the inner nestedHierarchy/nestedItemId identity.
            hr = hierarchy.GetNestedHierarchy(itemid, ref hierGuid, out nestedHierarchyObj, out nestedItemId);
            if (VSConstants.S_OK == hr && IntPtr.Zero != nestedHierarchyObj)
            {
                IVsHierarchy nestedHierarchy = Marshal.GetObjectForIUnknown(nestedHierarchyObj) as IVsHierarchy;
                Marshal.Release(nestedHierarchyObj);    // we are responsible to release the refcount on the out IntPtr parameter
                if (nestedHierarchy != null)
                {
                    // Display name and type of the node in the Output Window
                    EnumHierarchyItems(nestedHierarchy, nestedItemId, recursionLevel, false, visibleNodesOnly, processNodeFunc);
                }
            }
            else
            {
                object pVar;

                // Display name and type of the node in the Output Window
                processNodeFunc(hierarchy, itemid, recursionLevel);

                recursionLevel++;

                //Get the first child node of the current hierarchy being walked
                // NOTE: to work around a bug with the Solution implementation of VSHPROPID_FirstChild,
                // we keep track of the recursion level. If we are asking for the first child under
                // the Solution, we use VSHPROPID_FirstVisibleChild instead of _FirstChild. 
                // In VS 2005 and earlier, the Solution improperly enumerates all nested projects
                // in the Solution (at any depth) as if they are immediate children of the Solution.
                // Its implementation _FirstVisibleChild is correct however, and given that there is
                // not a feature to hide a SolutionFolder or a Project, thus _FirstVisibleChild is 
                // expected to return the identical results as _FirstChild.
                hr = hierarchy.GetProperty(itemid,
                    ((visibleNodesOnly || (hierIsSolution && recursionLevel == 1) ?
                        (int)__VSHPROPID.VSHPROPID_FirstVisibleChild : (int)__VSHPROPID.VSHPROPID_FirstChild)),
                    out pVar);
                Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(hr);
                if (VSConstants.S_OK == hr)
                {
                    //We are using Depth first search so at each level we recurse to check if the node has any children
                    // and then look for siblings.
                    uint childId = GetItemId(pVar);
                    while (childId != VSConstants.VSITEMID_NIL)
                    {
                        EnumHierarchyItems(hierarchy, childId, recursionLevel, false, visibleNodesOnly, processNodeFunc);
                        // NOTE: to work around a bug with the Solution implementation of VSHPROPID_NextSibling,
                        // we keep track of the recursion level. If we are asking for the next sibling under
                        // the Solution, we use VSHPROPID_NextVisibleSibling instead of _NextSibling. 
                        // In VS 2005 and earlier, the Solution improperly enumerates all nested projects
                        // in the Solution (at any depth) as if they are immediate children of the Solution.
                        // Its implementation   _NextVisibleSibling is correct however, and given that there is
                        // not a feature to hide a SolutionFolder or a Project, thus _NextVisibleSibling is 
                        // expected to return the identical results as _NextSibling.
                        hr = hierarchy.GetProperty(childId,
                            ((visibleNodesOnly || (hierIsSolution && recursionLevel == 1)) ?
                                (int)__VSHPROPID.VSHPROPID_NextVisibleSibling : (int)__VSHPROPID.VSHPROPID_NextSibling),
                            out pVar);
                        if (VSConstants.S_OK == hr)
                        {
                            childId = GetItemId(pVar);
                        }
                        else
                        {
                            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(hr);
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the item id.
        /// </summary>
        /// <param name="pvar">VARIANT holding an itemid.</param>
        /// <returns>Item Id of the concerned node</returns>
        private uint GetItemId(object pvar)
        {
            if (pvar == null) return VSConstants.VSITEMID_NIL;
            if (pvar is int) return (uint)(int)pvar;
            if (pvar is uint) return (uint)pvar;
            if (pvar is short) return (uint)(short)pvar;
            if (pvar is ushort) return (uint)(ushort)pvar;
            if (pvar is long) return (uint)(long)pvar;
            return VSConstants.VSITEMID_NIL;
        }

        /// <summary>
        /// Returns the filename of the solution
        /// </summary>
        public string GetSolutionFileName()
        {
            IVsSolution sol = (IVsSolution)this.Package.GetService<SVsSolution>();
            string solutionDirectory, solutionFile, solutionUserOptions;
            if (sol.GetSolutionInfo(out solutionDirectory, out solutionFile, out solutionUserOptions) == VSConstants.S_OK)
            {
                return solutionFile;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Refreshes the glyphs of the specified hierarchy nodes
        /// </summary>
        public void RefreshGlyphs(IList<VSITEMSELECTION> selectedNodes)
        {
            foreach (VSITEMSELECTION vsItemSel in selectedNodes)
            {
                IVsSccProject2 sccProject2 = vsItemSel.pHier as IVsSccProject2;
                if (vsItemSel.itemid == VSConstants.VSITEMID_ROOT)
                {
                    if (sccProject2 == null)
                    {
                        // Note: The solution's hierarchy does not implement IVsSccProject2, IVsSccProject interfaces
                        // It may be a pain to treat the solution as special case everywhere; a possible workaround is 
                        // to implement a solution-wrapper class, that will implement IVsSccProject2, IVsSccProject and
                        // IVsHierarhcy interfaces, and that could be used in provider's code wherever a solution is needed.
                        // This approach could unify the treatment of solution and projects in the provider's code.

                        // Until then, solution is treated as special case
                        var rgpszFullPaths = new string[] { GetSolutionFileName() };
                        var rgsiGlyphs = new VsStateIcon[1];
                        var rgdwSccStatus = new uint[1];
                        this.GetSccGlyph(1, rgpszFullPaths, rgsiGlyphs, rgdwSccStatus);

                        // Set the solution's glyph directly in the hierarchy
                        IVsHierarchy solHier = (IVsHierarchy)this.Package.GetService<SVsSolution>();
                        solHier.SetProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_StateIconIndex, rgsiGlyphs[0]);
                    }
                    else
                    {
                        // Refresh all the glyphs in the project; the project will call back GetSccGlyphs() 
                        // with the files for each node that will need new glyph
                        sccProject2.SccGlyphChanged(0, null, null, null);
                    }
                }
                else
                {
                    // It may be easier/faster to simply refresh all the nodes in the project, 
                    // and let the project call back on GetSccGlyphs, but just for the sake of the demo, 
                    // let's refresh ourselves only one node at a time
                    //IList<string> sccFiles = GetNodeFiles(sccProject2, vsItemSel.itemid);

                    // We'll use for the node glyph just the Master file's status (ignoring special files of the node)
                    //if (sccFiles.Count > 0)
                        UpdateNode(vsItemSel);
                }
            }
        }

        public void RefreshAllGlyphs()
        {
            var items = this.GetAllItems(null);
            this.RefreshGlyphs(items);
        }

        public void RefreshGlyphs(List<string> names)
        {
            Func<VSITEMSELECTION, bool> where = delegate(VSITEMSELECTION item)
            {
                string name;
                item.pHier.GetCanonicalName(item.itemid, out name);
                return names.Contains(name);
            };
            var items = this.GetAllItems(where);
            this.RefreshGlyphs(items);
        }

        private void UpdateNode(VSITEMSELECTION vsItemSel)
        {
            try
            {
                var sccProject2 = vsItemSel.pHier as IVsSccProject2;
                var rgpszFullPaths = new string[1];
                vsItemSel.pHier.GetCanonicalName(vsItemSel.itemid, out rgpszFullPaths[0]);
                var rgsiGlyphs = new VsStateIcon[1];
                var rgdwSccStatus = new uint[1];

                this.GetSccGlyph(1, rgpszFullPaths, rgsiGlyphs, rgdwSccStatus);

                var rguiAffectedNodes = new uint[] { vsItemSel.itemid };
                sccProject2.SccGlyphChanged(1, rguiAffectedNodes, rgsiGlyphs, rgdwSccStatus);
            }
            catch (Exception e)
            {
                e.ToString();
            }
        }

        /// <summary>
        /// Returns a list of source controllable files associated with the specified node
        /// </summary>
        private IList<string> GetNodeFiles(IVsSccProject2 pscp2, uint itemid)
        {
            // NOTE: the function returns only a list of files, containing both regular files and special files
            // If you want to hide the special files (similar with solution explorer), you may need to return 
            // the special files in a hastable (key=master_file, values=special_file_list)

            // Initialize output parameters
            IList<string> sccFiles = new List<string>();
            if (pscp2 != null)
            {
                CALPOLESTR[] pathStr = new CALPOLESTR[1];
                CADWORD[] flags = new CADWORD[1];

                if (pscp2.GetSccFiles(itemid, pathStr, flags) == 0)
                {
                    for (int elemIndex = 0; elemIndex < pathStr[0].cElems; elemIndex++)
                    {
                        IntPtr pathIntPtr = Marshal.ReadIntPtr(pathStr[0].pElems, elemIndex);
                        String path = Marshal.PtrToStringAuto(pathIntPtr);

                        sccFiles.Add(path);

                        // See if there are special files
                        if (flags.Length > 0 && flags[0].cElems > 0)
                        {
                            int flag = Marshal.ReadInt32(flags[0].pElems, elemIndex);

                            if (flag != 0)
                            {
                                // We have special files
                                CALPOLESTR[] specialFiles = new CALPOLESTR[1];
                                CADWORD[] specialFlags = new CADWORD[1];

                                pscp2.GetSccSpecialFiles(itemid, path, specialFiles, specialFlags);
                                for (int i = 0; i < specialFiles[0].cElems; i++)
                                {
                                    IntPtr specialPathIntPtr = Marshal.ReadIntPtr(specialFiles[0].pElems, i * IntPtr.Size);
                                    String specialPath = Marshal.PtrToStringAuto(specialPathIntPtr);

                                    sccFiles.Add(specialPath);
                                    Marshal.FreeCoTaskMem(specialPathIntPtr);
                                }

                                if (specialFiles[0].cElems > 0)
                                {
                                    Marshal.FreeCoTaskMem(specialFiles[0].pElems);
                                }
                            }
                        }

                        Marshal.FreeCoTaskMem(pathIntPtr);
                    }
                    if (pathStr[0].cElems > 0)
                    {
                        Marshal.FreeCoTaskMem(pathStr[0].pElems);
                    }
                }
            }

            return sccFiles;
        }

        #region IVsQueryEditQuerySave3 Members

        public int QuerySaveFile2(string pszMkDocument, uint[] rgf, VSQEQS_FILE_ATTRIBUTE_DATA[] pFileInfo, out uint pdwQSResult, out uint prgfMoreInfo)
        {
            prgfMoreInfo = (uint)tagVSQuerySaveResultFlags.QSR_DefaultFlag;
            pdwQSResult = (uint)tagVSQuerySaveResult.QSR_SaveOK;
            
            //HACK: this really shouldnt be done this way as we are refreshing all icons everytime one file is edited

            Func<VSITEMSELECTION, bool> where = delegate(VSITEMSELECTION item)
            {
                string name;
                item.pHier.GetCanonicalName(item.itemid, out name);
                return pszMkDocument.ToLower() == name;
            };
            var nodes = this.GetAllItems(where);
            RefreshGlyphs(nodes);
            
            return VSConstants.S_OK;
        }

        public int QuerySaveFiles2(uint[] rgfQuerySave, int cFiles, string[] rgpszMkDocuments, uint[] rgrgf, VSQEQS_FILE_ATTRIBUTE_DATA[] rgFileInfo, out uint pdwQSResult, out uint prgfMoreInfo)
        {
            prgfMoreInfo = (uint)tagVSQuerySaveResultFlags.QSR_DefaultFlag;
            pdwQSResult = (uint)tagVSQuerySaveResult.QSR_SaveOK;
            return VSConstants.S_OK;
        }

        #endregion


    }
}
