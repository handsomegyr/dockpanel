using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace Guoyongrong.WinFormsUI.Docking
{
    public partial class DockPage : Panel, IDockContentContainer, IDragSource
    {
        #region Events

        public event DockContentChangedEventHandler ActiveContentChanged;
        protected void OnActiveContentChanged(DockContentChangedEventArgs e)
        {
            if (ActiveContentChanged != null)
                ActiveContentChanged(this, e);
        }

        public event DockStateChangedEventHandler DockStateChanged;
        protected virtual void OnDockStateChanged(DockStateChangedEventArgs e)
        {
            if (DockStateChanged != null)
                DockStateChanged(this, e);
        }
        #endregion

        #region Property

        private DockState m_dockState = DockState.Unknown;
        public DockState DockState
        {
            get { return m_dockState; }
            set
            {
                if (value == DockState)
                {
                    return;
                }
                
                DockState oldState = m_dockState;
                DockState newState = value;
                foreach (DockContent dc in this.DisplayingContents)
                {
                    dc.DockState = newState;
                }

                m_dockState = value;

                DockStateChangedEventArgs e = new DockStateChangedEventArgs(oldState, newState);
                OnDockStateChanged(e);

                RefreshChanges();

            }
        }

        public string CaptionText
        {
            get { return this.DockPageCaptionControl.CaptionText; }
        }

        private DockContent m_activeContent;
        public virtual DockContent ActiveContent
        {
            get
            {
                return m_activeContent;
            }
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

        #endregion

        #region Skin

        private bool m_allowEndUserDocking = true;
        [Category("Category_Docking")]
        [Description("DockPanel_AllowEndUserDocking_Description")]
        [DefaultValue(true)]
        public virtual bool AllowEndUserDocking
        {
            get { return m_allowEndUserDocking; }
            set { m_allowEndUserDocking = value; }
        }

        private bool m_allowDockDragAndDrop = true;
        [Category("Category_Docking")]
        [Description("DockPanel_AllowEndUserDocking_Description")]
        [DefaultValue(true)]
        public virtual bool AllowDockDragAndDrop
        {
            get { return m_allowDockDragAndDrop; }
            set { m_allowDockDragAndDrop = value; }
        }
        
        #endregion

        private bool CheckIsVisibled(DockContent c)
        {
            return (c.DockState == this.DockState && !c.IsHidden);
        }
        
        public DockPage()
        {
            SetStyle(ControlStyles.Selectable, false);
            SetStyle(ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer, true);

            this.SuspendLayout();

            InitializeComponent();

            AllowDrop = true;

            m_splitter = new DockPageSplitter(this);

            m_captionControl = new VS2005DockPageCaption();
            m_tabStripControl = new VS2005DockPageStrip();
            m_tabStripControl.IsVisibleFunc = CheckIsVisibled;
            m_tabStripControl.ActiveContentChanged += new DockContentChangedEventHandler(m_tabStripControl_ActiveContentChanged);
            m_tabStripControl.RemoveAll();
            Controls.AddRange(new Control[] { m_captionControl, m_tabStripControl });
            m_displayingContents = new VisibleContentCollection(CheckIsVisibled, this.DockContents);
            this.DockContents.DockContentRemoved += new DockContentEventHandler(DockContents_DockContentRemoved);
            this.DockContents.DockContentAdded += new DockContentEventHandler(DockContents_DockContentAdded);

            DockContent dummyContent1 = new DockContent();
            dummyContent1.Text = "Content1";
            this.AddDockContent(dummyContent1);

            DockContent dummyContent2 = new DockContent();
            dummyContent2.Text = "Content2";
            this.AddDockContent(dummyContent2);

            this.DockState = DockState.Unknown;

            this.ResumeLayout();

        }

        #region DockContents Events
        private void DockContents_DockContentRemoved(object sender, DockContentEventArgs e)
        {
            this.DockPageStripControl.RemoveDockContent(e.Content);
            if (ActiveContent == e.Content)
            {
                if (this.DisplayingContents.Count > 0)
                {
                    ActiveContent = this.DisplayingContents[0];
                }
            }

            if (this.DisplayingContents.Count == 0)
            {
                ActiveContent = null;
            }

            this.RefreshChanges();
        }
        private void DockContents_DockContentAdded(object sender, DockContentEventArgs e)
        {
            this.DockPageStripControl.AddDockContent(e.Content);
            this.ActiveContent = e.Content;
            this.RefreshChanges();
        }
        #endregion

        #region m_tabStripControl.Events
        void m_tabStripControl_ActiveContentChanged(object sender, DockContentChangedEventArgs e)
        {
            DockContent content = e.NewContent;
            this.ActiveContent = content;
        }
        #endregion

        #region Paint & Layout

        private void ValidateActiveContent()
        {
            if (ActiveContent == null)
            {
                if (DisplayingContents.Count != 0)
                    ActiveContent = DisplayingContents[0];
                return;
            }

            if (DisplayingContents.IndexOf(ActiveContent) >= 0)
                return;

            DockContent prevVisible = null;
            for (int i = DockContents.IndexOf(ActiveContent) - 1; i >= 0; i--)
                if (DockContents[i].DockState == DockState)
                {
                    prevVisible = DockContents[i];
                    break;
                }

            DockContent nextVisible = null;
            for (int i = DockContents.IndexOf(ActiveContent) + 1; i < DockContents.Count; i++)
                if (DockContents[i].DockState == DockState)
                {
                    nextVisible = DockContents[i];
                    break;
                }

            if (prevVisible != null)
                ActiveContent = prevVisible;
            else if (nextVisible != null)
                ActiveContent = nextVisible;
            else
                ActiveContent = null;
        }

        private void RefreshChanges(bool performLayout)
        {
            if (IsDisposed)
                return;

            this.Appearance =
                (this.DockState == DockState.Document) ?
                AppearanceStyle.Document : AppearanceStyle.ToolWindow;

            if (this.ActiveContent != null && !this.CheckIsVisibled(this.ActiveContent) && this.ActiveContent.IsFloat)
            {
                if (this.DisplayingContents.Count > 0)
                {
                    this.ActiveContent = this.DisplayingContents[0];
                }
                else
                {
                    this.ActiveContent = null;
                }
            }
            ValidateActiveContent();
            DockPageCaptionControl.Skin = this.Skin.DockPaneCaptionSkin;
            DockPageCaptionControl.ActiveContent = this.ActiveContent;
            DockPageCaptionControl.IsActivated = this.IsActivated;

            DockPageStripControl.Appearance = this.Appearance;
            DockPageStripControl.DocumentTabStripLocation = this.DocumentTabStripLocation;
            DockPageStripControl.Skin = this.Skin.DockPaneStripSkin;
            DockPageStripControl.ShowDocumentIcon = this.ShowDocumentIcon;
            DockPageStripControl.ActiveContent = this.ActiveContent;
            DockPageStripControl.IsActivated = this.IsActivated;

            DockPageCaptionControl.RefreshChanges();
            DockPageStripControl.RefreshChanges();

            if (performLayout)
                PerformLayout();
        }
        
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            this.RefreshChanges();

        }
        protected override void OnLayout(LayoutEventArgs levent)
        {
            //bool isHidden = (this.DockContents.Count == 0);
            bool isHidden = (this.DisplayingContents.Count == 0);
            this.Visible = !isHidden;

            SetIsHidden(isHidden);

            if (!IsHidden)
            {
                DockPageCaptionControl.Bounds = CaptionRectangle;
                DockPageStripControl.Bounds = TabStripRectangle;
                DockPageStripControl.PerformLayout();

                SetContentBounds();

                //foreach (IDockContent content in Contents)
                //{
                //    if (DisplayingContents.Contains(content))
                //        if (content.DockHandler.FlagClipWindow && content.DockHandler.Form.Visible)
                //            content.DockHandler.FlagClipWindow = false;
                //}
            }

            base.OnLayout(levent);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.CompositingMode = CompositingMode.SourceOver;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

        }
        protected virtual Rectangle TabStripRectangle
        {
            get
            {
                if (Appearance == AppearanceStyle.ToolWindow)
                    return TabStripRectangle_ToolWindow;
                else
                    return TabStripRectangle_Document;
            }
        }

        protected virtual Rectangle TabStripRectangle_Document
        {
            get
            {
                if (this.DisplayingContents.Count == 0)
                    return Rectangle.Empty;

                if (this.DisplayingContents.Count == 1 && this.DocumentStyle == DocumentStyle.DockingSdi)
                    return Rectangle.Empty;

                Rectangle rectWindow = DisplayingRectangle;
                int x = rectWindow.X;
                int width = rectWindow.Width;
                int height = DockPageStripControl.Height;

                int y = 0;
                if (this.DocumentTabStripLocation == DocumentTabStripLocation.Bottom)
                    y = rectWindow.Height - height;
                else
                    y = rectWindow.Y;

                return new Rectangle(x, y, width, height);
            }
        }

        protected virtual Rectangle TabStripRectangle_ToolWindow
        {
            get
            {
                if (this.DisplayingContents.Count <= 1 || IsAutoHide)
                    return Rectangle.Empty;

                Rectangle rectWindow = DisplayingRectangle;

                int width = rectWindow.Width;
                int height = DockPageStripControl.Height;
                int x = rectWindow.X;
                int y = rectWindow.Bottom - height;
                Rectangle rectCaption = CaptionRectangle;
                if (rectCaption.Contains(x, y))
                    y = rectCaption.Y + rectCaption.Height;

                return new Rectangle(x, y, width, height);
            }
        }

        protected virtual Rectangle CaptionRectangle
        {
            get
            {
                if (!HasCaption)
                    return Rectangle.Empty;

                Rectangle rectWindow = DisplayingRectangle;
                int x, y, width;
                x = rectWindow.X;
                y = rectWindow.Y;
                width = rectWindow.Width;
                int height = DockPageCaptionControl.Height;
                return new Rectangle(x, y, width, height);
            }
        }

        protected virtual Rectangle DisplayingRectangle
        {
            get { return ClientRectangle; }
        }

        internal virtual Rectangle ContentRectangle
        {
            get
            {
                Rectangle rectWindow = DisplayingRectangle;
                Rectangle rectCaption = CaptionRectangle;
                Rectangle rectTabStrip = TabStripRectangle;

                int x = rectWindow.X;

                int y = rectWindow.Y + (rectCaption.IsEmpty ? 0 : rectCaption.Height);

                if (this.Appearance == AppearanceStyle.Document &&
                    this.DocumentTabStripLocation == DocumentTabStripLocation.Top)
                    y += rectTabStrip.Height;

                int width = rectWindow.Width;
                int height = rectWindow.Height - rectCaption.Height - rectTabStrip.Height;

                return new Rectangle(x, y, width, height);
            }
        }

        internal virtual void SetContentBounds()
        {
            Rectangle rectContent = ContentRectangle;
            Rectangle rectInactive = new Rectangle(-rectContent.Width, rectContent.Y, rectContent.Width, rectContent.Height);
            foreach (DockContent content in DockContents)
            {
                if (content.Parent == this)
                {
                    if (content == ActiveContent)
                        content.Bounds = rectContent;
                    else
                        content.Bounds = rectInactive;
                }
            }
        }

        #endregion

        #region HitTest

        internal HitTestResult GetHitTest(Point ptMouse)
        {
            Point ptMouseClient = PointToClient(ptMouse);

            Rectangle rectCaption = CaptionRectangle;
            if (rectCaption.Contains(ptMouseClient))
                return new HitTestResult(HitTestArea.Caption, -1);

            Rectangle rectContent = ContentRectangle;
            if (rectContent.Contains(ptMouseClient))
                return new HitTestResult(HitTestArea.Content, -1);

            Rectangle rectTabStrip = TabStripRectangle;
            if (rectTabStrip.Contains(ptMouseClient))
                return new HitTestResult(HitTestArea.TabStrip, DockPageStripControl.HitTest(DockPageStripControl.PointToClient(ptMouse)));

            return new HitTestResult(HitTestArea.None, -1);
        }

        #endregion
        
       
        private Guid m_Guid = Guid.NewGuid();
        public Guid GUID
        {
            get { return m_Guid; }
            set { m_Guid = value; }
        }

        private bool m_HasCaption = true;
        public bool HasCaption
        {
            get
            {
                if (DockState == DockState.Document ||
                    DockState == DockState.Hidden ||
                    DockState == DockState.Unknown)
                    return false;
                else
                    return m_HasCaption;
            }
            set
            {
                if (m_HasCaption == value)
                {
                    return;
                }
                m_HasCaption = value;
                RefreshChanges();
            }
        }

        public bool IsFloat
        {
            get { return DockState == DockState.Float; }
        }

        public bool IsAutoHide
        {
            get { return DockHelper.IsDockStateAutoHide(DockState); }
        }

        private DockPageCaptionBase m_captionControl;
        public DockPageCaptionBase DockPageCaptionControl
        {
            get { return m_captionControl; }
        }

        private DockPageStripBase m_tabStripControl;
        public DockPageStripBase DockPageStripControl
        {
            get { return m_tabStripControl; }
        }

        private DocumentStyle m_documentStyle = DocumentStyle.DockingMdi;
        [Category("Category_Docking")]
        [Description("DockPanel_DocumentStyle_Description")]
        [DefaultValue(DocumentStyle.DockingMdi)]
        public DocumentStyle DocumentStyle
        {
            get { return m_documentStyle; }
            set
            {
                if (value == m_documentStyle)
                    return;

                m_documentStyle = value;

                RefreshChanges();
            }
        }

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

        private DockPanelSkin m_skin = new DockPanelSkin();
        public DockPanelSkin Skin
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

        private bool m_showDocumentIcon = true;
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

        private AppearanceStyle m_Appearance = AppearanceStyle.ToolWindow;
        public AppearanceStyle Appearance
        {
            get { return m_Appearance; }
            set
            {
                if (m_Appearance == value)
                {
                    return;
                }

                m_Appearance = value;

                RefreshChanges();
            }
        }

        public void RefreshChanges()
        {
            RefreshChanges(true);
        }

       

        #region IDockContentContainer

        private VisibleContentCollection m_displayingContents;
        public VisibleContentCollection DisplayingContents
        {
            get { return m_displayingContents; }
        }

        private DockContentCollection m_DockContents = new DockContentCollection();
        public DockContentCollection DockContents
        {
            get
            {
                return m_DockContents;
            }
        }

        public void AddDockContent(DockContent content)
        {
            content.DockState = this.DockState;
            content.DockPage = this;
            this.DockContents.Add(content);
            if (this.ActiveContent != content)
            {
                this.ActiveContent = content;
            }
        }

        public void RemoveDockContent(DockContent content)
        {
            if (this.DockContents.Contains(content))
            {
                //content.DockState = DockState.Unknown;
                content.DockPage = null;
                this.DockContents.Remove(content);
            }
        }

        public void RemoveAll()
        {
            for (int i = this.DockContents.Count - 1; i >= 0; i--)
            {
                this.RemoveDockContent(this.DockContents[i]);
            }
        }

        #endregion

        #region DockTo

        private DockPageSplitter m_splitter;
        public DockPageSplitter Splitter
        {
            get { return m_splitter; }
        }

        internal Rectangle SplitterBounds
        {
            set { Splitter.Bounds = value; }
        }

        internal DockAlignment SplitterAlignment
        {
            set { Splitter.Alignment = value; }
        }

        private bool m_isHidden = false;
        internal bool IsHidden
        {
            get { return m_isHidden; }
            set { m_isHidden = value; }
        }
        private void SetIsHidden(bool value)
        {
            this.Visible = !value;

            if (m_isHidden == value)
                return;

            m_isHidden = value;

            if (Parent != null)
            {
                Parent.PerformLayout();
            }
        }

        internal void SetParent(Control value)
        {
            if (Parent == value)
                return;

            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            // Workaround of .Net Framework bug:
            // Change the parent of a control with focus may result in the first
            // MDI child form get activated. 
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //DockContent contentFocused = GetFocusedContent();
            //if (contentFocused != null)
            //    DockPanel.SaveFocus();

            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

            Parent = (Control)value;

            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            // Workaround of .Net Framework bug:
            // Change the parent of a control with focus may result in the first
            // MDI child form get activated. 
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //if (contentFocused != null)
            //    contentFocused.DockHandler.Activate();
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        }

        private NestedDockingStatus m_NestedDockingStatus = new NestedDockingStatus();
        internal NestedDockingStatus NestedDockingStatus
        {
            get
            {
                return m_NestedDockingStatus;
            }
        }

        private IDockPageContainer _dockControl;
        internal IDockPageContainer DockControl
        {
            get { return _dockControl; }
            set 
            {
                if (_dockControl == value)
                {
                    return;
                }

                IDockPageContainer oldPageContainer = _dockControl;
                _dockControl = value;

                if (oldPageContainer != null)
                {
                    oldPageContainer.RemoveDockPage(this);
                }                 
            }
        }

        internal void SetContentIndex(DockContent content, int index)
        {
            int oldIndex = DockContents.IndexOf(content);
            if (oldIndex == -1)
                throw (new ArgumentException(Strings.DockPane_SetContentIndex_InvalidContent));

            if (index < 0 || index > DockContents.Count - 1)
                if (index != -1)
                    throw (new ArgumentOutOfRangeException(Strings.DockPane_SetContentIndex_InvalidIndex));

            if (oldIndex == index)
                return;
            if (oldIndex == DockContents.Count - 1 && index == -1)
                return;

            DockContents.Remove(content);
            if (index == -1)
                DockContents.Add(content);
            else if (oldIndex < index)
                DockContents.AddAt(content, index - 1);
            else
                DockContents.AddAt(content, index);

            RefreshChanges(true);
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
