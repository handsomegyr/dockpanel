using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics.CodeAnalysis;
using System.Security.Permissions;

namespace Guoyongrong.WinFormsUI.Docking
{
    public partial class FloatWindow : Form, IDragSource,IDockPageContainer
    {
        public event EventHandler CaptionClick;
        protected void OnCaptionClick(EventArgs e)
        {
            if (CaptionClick != null)
                CaptionClick(this, e);
        }

        public event EventHandler CaptionDoubleClick;
        protected void OnCaptionDoubleClick(EventArgs e)
        {
            if (CaptionDoubleClick != null)
                CaptionDoubleClick(this, e);
        }

        public event EventHandler FormCloseClick;
        protected void OnFormCloseClick(EventArgs e)
        {
            if (FormCloseClick != null)
                FormCloseClick(this, e);
        }

        internal const int WM_CHECKDISPOSE = (int)(Win32.Msgs.WM_USER + 1);

        private bool m_allowEndUserDocking = true;
        [Category("Category_Docking")]
        [Description("DockPanel_AllowEndUserDocking_Description")]
        [DefaultValue(true)]
        public virtual bool AllowEndUserDocking
        {
            get { return m_allowEndUserDocking; }
            set { m_allowEndUserDocking = value; }
        }

        private NestedPageCollection m_nestedPanes;
        internal NestedPageCollection NestedPanes
        {
            get { return m_nestedPanes; }
        }
        internal VisibleNestedPageCollection VisibleNestedPanes
        {
            get { return NestedPanes.VisibleNestedPanes; }
        }

        public FloatWindow()
        {
            SuspendLayout();

            InitializeComponent();

            this.Visible = false;
            m_nestedPanes = new NestedPageCollection(this);

            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            ShowInTaskbar = false;

            ResumeLayout();
            
        }

        public void RefreshChanges()
        {
            if (IsDisposed)
                return;

            if (!this.Visible)
            {
                return;
            }

            Visible = (VisibleNestedPanes.Count > 0) && this.Visible;
            SetText();

            if (VisibleNestedPanes.Count == 0)
            {
                ControlBox = true;
                return;
            }

            bool hasCaption = !(VisibleNestedPanes.Count == 1);
            
            for (int i = VisibleNestedPanes.Count - 1; i >= 0; i--)
            {
                VisibleNestedPanes[i].HasCaption = hasCaption;
            }

            for (int i = VisibleNestedPanes.Count - 1; i >= 0; i--)
            {
                DockContentCollection contents = VisibleNestedPanes[i].DockContents;
                for (int j = contents.Count - 1; j >= 0; j--)
                {
                    DockContent content = contents[j];
                    if (content.DockState != DockState.Float)
                        continue;

                    if (content.CloseButtonEnable && content.CloseButtonVisible)
                    {
                        ControlBox = true;
                        return;
                    }
                }
            }
            //Only if there is a ControlBox do we turn it off
            //old code caused a flash of the window.
            if (ControlBox)
                ControlBox = false;

        }

        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.Windows.Forms.Control.set_Text(System.String)")]
        internal void SetText()
        {
            DockPage theOnlyPane = (VisibleNestedPanes.Count == 1) ? VisibleNestedPanes[0] : null;

            if (theOnlyPane == null)
                Text = " ";	// use " " instead of string.Empty because the whole title bar will disappear when ControlBox is set to false.
            else if (theOnlyPane.ActiveContent == null)
                if (theOnlyPane.DockContents.Count == 0)
                {
                    Text = " ";
                }
                else
                {
                    Text = theOnlyPane.DockContents[0].Text;
                }
                else
                    Text = theOnlyPane.ActiveContent.Text;
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            if (!this.Visible)
            {
                return;
            }

            VisibleNestedPanes.Refresh();
            RefreshChanges();
            base.OnLayout(levent);
        }
        
        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            Rectangle rectWorkArea = SystemInformation.VirtualScreen;

            if (y + height > rectWorkArea.Bottom)
                y -= (y + height) - rectWorkArea.Bottom;

            if (y < rectWorkArea.Top)
                y += rectWorkArea.Top - y;

            base.SetBoundsCore(x, y, width, height, specified);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == (int)Win32.Msgs.WM_NCLBUTTONDOWN)
            {
                if (IsDisposed)
                    return;

                uint result = NativeMethods.SendMessage(this.Handle, (int)Win32.Msgs.WM_NCHITTEST, 0, (uint)m.LParam);
                if (result == 2)	// HITTEST_CAPTION
                {
                    Activate();
                    OnCaptionClick(new EventArgs());
                }
                else
                    base.WndProc(ref m);

                return;
            }
            else if (m.Msg == (int)Win32.Msgs.WM_NCRBUTTONDOWN)
            {
                uint result = NativeMethods.SendMessage(this.Handle, (int)Win32.Msgs.WM_NCHITTEST, 0, (uint)m.LParam);
                if (result == 2)	// HITTEST_CAPTION
                {
                    DockPage theOnlyPane = (VisibleNestedPanes.Count == 1) ? VisibleNestedPanes[0] : null;
                    if (theOnlyPane != null && theOnlyPane.ActiveContent != null)
                    {
                        //theOnlyPane.ShowTabPageContextMenu(this, PointToClient(Control.MousePosition));
                        return;
                    }
                }

                base.WndProc(ref m);
                return;
            }
            else if (m.Msg == (int)Win32.Msgs.WM_CLOSE)
            {
                if (NestedPanes.Count == 0)
                {
                    base.WndProc(ref m);
                    return;
                }

                OnFormCloseClick(new EventArgs());
                
                return;
            }
            else if (m.Msg == (int)Win32.Msgs.WM_NCLBUTTONDBLCLK)
            {
                uint result = NativeMethods.SendMessage(this.Handle, (int)Win32.Msgs.WM_NCHITTEST, 0, (uint)m.LParam);
                if (result != 2)	// HITTEST_CAPTION
                {
                    base.WndProc(ref m);
                    return;
                }

                OnCaptionDoubleClick(new EventArgs());
                return;
            }
            else if (m.Msg == WM_CHECKDISPOSE)
            {
                if (NestedPanes.Count == 0)
                    Dispose();

                return;
            }

            base.WndProc(ref m);
        }

        #region IDockPageContainer

        private Guid m_Guid = Guid.NewGuid();
        public Guid GUID
        {
            get { return m_Guid; }
            set { m_Guid = value; }
        }

        public bool IsAutoHide
        {
            get { return false; }
        }
        public bool IsFloat
        {
            get { return true; }
        }

        internal void SetDockPageParent(DockPage page)
        {
            page.SetParent(this);
            page.Splitter.Parent = this;
        }

        public void AddDockPage(DockPage page)
        {
            AddDockPage(page, null, DockAlignment.Unknown, 0.5, -1);
        }

        public void AddDockPage(DockPage page, DockPage previousPage, DockAlignment alignment, double proportion, int contentIndex)
        {
            //如果page为空
            if (page == null)
            {
                return;
            }

            page.DockState = this.DockState;
            page.DockControl = this;
            SetDockPageParent(page);

            this.NestedPanes.Add(page, previousPage, alignment, proportion, contentIndex);
            this.DockPageCollection.Add(page);
            
            if (this.NestedPanes.Contains(page)) this.PerformLayout();
        }

        public void RemoveDockPage(DockPage page)
        {
            //如果page为空
            if (page == null)
            {
                return;
            }

            bool isContained = this.NestedPanes.Contains(page);
            if (isContained)
            {
                page.DockControl = null;
                page.SetParent(null);
                page.Splitter.Parent = null;
            }

            //pan.Visible = false;
            this.NestedPanes.Remove(page);
            this.DockPageCollection.Remove(page);

            if (isContained) this.PerformLayout();
        }

        private DockPageCollection m_DockPages = new DockPageCollection();
        public DockPageCollection DockPageCollection
        {
            get
            {
                return m_DockPages;
            }
        }

        public void RemoveAll()
        {
            for (int i = this.DockPageCollection.Count - 1; i >= 0; i--)
            {
                this.RemoveDockPage(this.DockPageCollection[i]);
            }
        }

        public DockState DockState
        {
            get { return DockState.Float; }
        }
        public Rectangle DisplayingRectangle
        {
            get { return this.ClientRectangle; }
        }

        #endregion

        #region IDockDragSource Members

        #region IDragSource Members

        Control IDragSource.DragControl
        {
            get { return this; }
        }

        #endregion

        #endregion

    }
}
