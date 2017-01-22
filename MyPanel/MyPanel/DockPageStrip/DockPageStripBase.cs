using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel;
using System.Drawing.Text;

namespace Guoyongrong.WinFormsUI.Docking
{
    public abstract class DockPageStripBase : Panel, IDockPageStripControl,IDockContentContainer
	{
        #region Events

        public event DockContentChangedEventHandler ActiveContentChanged;
        protected virtual void OnActiveContentChanged(DockContentChangedEventArgs e)
        {
            if (ActiveContentChanged != null)
                ActiveContentChanged(this, e);
        }

        public event DockContentEventHandler DockContentClicked;
        protected virtual void OnDockContentClicked(DockContentEventArgs e)
        {
            if (DockContentClicked != null)
                DockContentClicked(this, e);
        }

        public event DockContentEventHandler DockContentDoubleClicked;
        protected virtual void OnDockContentDoubleClicked(DockContentEventArgs e)
        {
            if (DockContentDoubleClicked != null)
                DockContentDoubleClicked(this, e);
        }
        #endregion
        
        private DockContent m_activeContent = null;
        public DockContent ActiveContent
        {
            get { return m_activeContent; }
            internal set
            {
                
                if (ActiveContent == value)
                    return;

                if (value != null)
                {
                    if (!DisplayingContents.Contains(value))
                    {
                        return;
                        //throw (new InvalidOperationException(Strings.DockPane_ActiveContent_InvalidValue));
                    }
                }
                else
                {
                    if (DisplayingContents.Count != 0)
                    {
                        return;
                        //throw (new InvalidOperationException(Strings.DockPane_ActiveContent_InvalidValue));
                    }
                }

                DockContent oldValue = m_activeContent;

                m_activeContent = value;

                DockContentChangedEventArgs e = new DockContentChangedEventArgs(oldValue, m_activeContent);

                OnActiveContentChanged(e);

                RefreshChanges();
            }
        }

        protected DockPageStripBase()
		{
            SetStyle(ControlStyles.Selectable, false);

            SetStyle(ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer, true);

            AllowDrop = true;
		}
     
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.CompositingMode = CompositingMode.SourceOver;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            
            base.OnPaint(e);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == (int)Win32.Msgs.WM_LBUTTONDBLCLK)
            {
                int index = HitTest();
                if (index != -1)
                {
                    DockContent content = DisplayingContents[index];
                    OnDockContentDoubleClicked(new DockContentEventArgs(content));
                }
            }
            base.WndProc(ref m);
        }

        protected virtual int HitTest()
		{
			return HitTest(PointToClient(Control.MousePosition));
		}
        
        #region IDockContentContainer

        [Browsable(false)]
        internal CheckIsVisibleFunction IsVisibleFunc
        {
            get
            {
                return DisplayingContents.IsVisibleFunc;
            }
            set 
            {
                DisplayingContents.IsVisibleFunc = value;
            }
        }

        protected VisibleContentCollection m_displayingContents;
        public VisibleContentCollection DisplayingContents
        {
            get { return m_displayingContents; }
        }

        private DockContentCollection m_DockContents = new DockContentCollection();
        public DockContentCollection DockContents
        {
            get { return m_DockContents; }
        }

        public void AddDockContent(DockContent content)
        {
            DockContents.Add(content);
        }

        public void RemoveDockContent(DockContent content)
        {
            DockContents.Remove(content);
        }

        public void RemoveAll()
        {
            this.DockContents.Clear();
        }

        #endregion

        #region IDockPageStripControl

        private bool m_IsActivated = false;
        public bool IsActivated
        {
            get { return m_IsActivated; }
            set
            {
                if (m_IsActivated == value)
                {
                    return;
                }
                m_IsActivated = value;
                RefreshChanges();
            }
        }

        private DockPaneStripSkin m_skin = new DockPaneStripSkin();
        public DockPaneStripSkin Skin
        {
            get { return m_skin; }
            set
            {
                if (m_skin == value)
                {
                    return;
                }

                m_skin = value;
                RefreshChanges();
            }
        }

        private DocumentTabStripLocation m_documentTabStripLocation = DocumentTabStripLocation.Top;
        [DefaultValue(DocumentTabStripLocation.Top)]
        [Category("Category_Docking")]
        [Description("DockPanel_DocumentTabStripLocation")]
        public DocumentTabStripLocation DocumentTabStripLocation
        {
            get { return m_documentTabStripLocation; }
            set
            {
                if (m_documentTabStripLocation == value)
                    return;

                m_documentTabStripLocation = value;
                RefreshChanges();
            }
        }

        private bool m_showDocumentIcon = false;
        [DefaultValue(false)]
        [Category("Category_Docking")]
        [Description("DockPanel_ShowDocumentIcon_Description")]
        public bool ShowDocumentIcon
        {
            get { return m_showDocumentIcon; }
            set
            {
                if (m_showDocumentIcon == value)
                    return;

                m_showDocumentIcon = value;
                RefreshChanges();
            }
        }

        private AppearanceStyle m_appearance = AppearanceStyle.ToolWindow;
        [DefaultValue(AppearanceStyle.ToolWindow)]
        [Category("Category_AppearanceStyle")]
        [Description("AppearanceStyle")]
        public AppearanceStyle Appearance
        {
            get { return m_appearance; }
            set
            {
                if (m_appearance == value)
                    return;

                m_appearance = value;
                RefreshChanges();
            }
        }

        public abstract Control CloseButton { get; }

        public abstract int HitTest(Point point);
        public abstract GraphicsPath GetOutline(int index);
        public abstract void RefreshChanges();

        #endregion

    }
}
