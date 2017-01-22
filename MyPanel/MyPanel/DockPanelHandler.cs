using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Guoyongrong.WinFormsUI.Docking
{
    internal class DockPanelHandler
    {
        private DockPanel m_DockPanel = null;
        public DockPanel DockPanel
        {
            get
            {
                return m_DockPanel;
            }
        }

        public DockPanelHandler(DockPanel dockPanel)
        {
            m_DockPanel = dockPanel;
        }

        #region DockContent Manager

        internal  event DockContentEventHandler DockContentAdded;
        private  void onDockContentAdded(DockContentEventArgs e)
        {
            if (DockContentAdded != null)
            {
                DockContentAdded(null, e);
            }
        }
        internal  event DockContentEventHandler DockContentRemoved;
        private  void onDockContentRemoved(DockContentEventArgs e)
        {
            if (DockContentRemoved != null)
            {
                DockContentRemoved(null, e);
            }
        }

        internal  Dictionary<Guid, DockContent> DockContents = new Dictionary<Guid, DockContent>();
        internal  DockContent GetOrCreateDockContent(IContent content)
        {
            DockContent dc = null;
            if (IsDockContentExisted(content.GUID))
            {
                dc = DockContents[content.GUID];
            }
            else
            {
                dc = new DockContent(content);                
                AddDockContent(dc);
            }
            return dc;
        }
        internal  void AddDockContent(DockContent content)
        {
            if (IsDockContentExisted(content.GUID))
            {
                return;
            }
            DockContents.Add(content.GUID, content);
            onDockContentAdded(new DockContentEventArgs(content));
        }
        internal  bool IsDockContentExisted(Guid GUID)
        {
            return DockContents.ContainsKey(GUID);
        }
        internal  void RemoveDockContent(DockContent content)
        {
            if (!IsDockContentExisted(content.GUID))
            {
                return;
            }
            DockContents.Remove(content.GUID);
            onDockContentRemoved(new DockContentEventArgs(content));
        }

        internal void CloseContent(DockContent content)
        {
            DockPanel dockPanel = DockPanel;
            dockPanel.SuspendLayout(true);

            if (content == null)
                return;

            if (!content.CloseButtonEnable)
                return;

            if (content.HideOnClose)
            {
                content.IsHidden = true;
            }
            else
            {
                content.Close();
            }
            if (content.DockPage!=null)
            {
                content.DockPage.RefreshChanges();
            }
            dockPanel.ResumeLayout(true, true);
        }

        #endregion

        #region DockPage Manager
        
        internal  event DockPageEventHandler DockPageAdded;
        private  void onDockPageAdded(DockPageEventArgs e)
        {
            if (DockPageAdded != null)
            {
                DockPageAdded(null, e);
            }
        }
        internal  event DockPageEventHandler DockPageRemoved;
        private  void onDockPageRemoved(DockPageEventArgs e)
        {
            if (DockPageRemoved != null)
            {
                DockPageRemoved(null, e);
            }
        }

        internal  DockPageCollection DockPages = new DockPageCollection();
        internal  DockPage CreateDockPage(DockState state)
        {
            DockPage page = new DockPage();
            page.Bounds = Rectangle.Empty;
            page.Visible = false;
            page.RemoveAll();
            page.DockState = state;
            DockPages.Add(page);
            onDockPageAdded(new DockPageEventArgs(page));
            return page;
        }
        internal  DockPage GetOrCreateDockPage(DockState state)
        {
            foreach (DockPage page in DockPages)
            {
                if (page.DockContents.Count < 1 && page.IsFloat == DockHelper.IsFloat(state) && page.DockControl==null)
                {
                    return page;
                }
            }
            return CreateDockPage(state);
        }
        
        private  DockPage GetFloatPaneFromContents(DockPage page)
        {
            DockPage floatPane = null;
            for (int i = 0; i < page.DisplayingContents.Count; i++)
            {
                DockContent content = page.DisplayingContents[i];
                if (!DockHelper.IsDockStateValid(DockState.Float, content.DockAreas))
                    continue;

                if (floatPane != null && content.DockContext.PrevFloatPage != floatPane)
                    return null;
                else
                    floatPane = content.DockContext.PrevFloatPage;
            }

            return floatPane;
        }
        private  DockContent GetFirstContent(DockPage page, DockState dockState)
        {
            for (int i = 0; i < page.DisplayingContents.Count; i++)
            {
                DockContent content = page.DisplayingContents[i];
                if (DockHelper.IsDockStateValid(dockState, content.DockAreas))
                    return content;
            }
            return null;
        }

        internal  DockPage FloatAt(DockPage page)
        {
            DockContent content = page.ActiveContent;
            DockPage floatPane = GetFloatPaneFromContents(page);
            if (floatPane == null)
            {
                DockContent firstContent = GetFirstContent(page,DockState.Float);
                if (firstContent == null)
                {
                    return null;
                }
                floatPane = GetOrCreateDockPage(DockState.Float);
            }
            
            DockContent[] contents = new DockContent[page.DisplayingContents.Count];
            page.DisplayingContents.CopyTo(contents,0);

            for (int i = 0; i < contents.Length; i++)
            {
                DockContent c = contents[i];
                floatPane.AddDockContent(c);
            }

            floatPane.ActiveContent = content;

            return floatPane;
        }

        internal  void RestoreToPanel(DockPage page)
        {
            DockContent content = page.ActiveContent;
            DockContent[] contents = new DockContent[page.DisplayingContents.Count];
            page.DisplayingContents.CopyTo(contents,0);

            for (int i = 0; i < contents.Length; i++)
            {
                DockContent c = contents[i];
                DockPage prevPanelPage = c.DockContext.PrevPanelPage;
                if (prevPanelPage != null && prevPanelPage.DockState != DockState.Unknown)
                {
                    prevPanelPage.AddDockContent(c);
                    prevPanelPage.ActiveContent = c;
                    if (c == content)
                    {
                        prevPanelPage.ActiveContent = c;
                    }
                }
            }
        }
        
        #endregion

    }
}
