using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Guoyongrong.WinFormsUI.Docking.Win32;
using System.Diagnostics.CodeAnalysis;

namespace Guoyongrong.WinFormsUI.Docking
{
    public delegate string GetPersistStringCallback();

    [ToolboxItem(false)]
    public class DockContent : Form, IDragSource, IDock, IContent
    {
        internal event DockContentEventHandler AutoHidePortionChanged;
        private void onAutoHidePortionChanged(DockContentEventArgs e)
        {
            if (AutoHidePortionChanged != null)
            {
                AutoHidePortionChanged(null, e);
            }
        }

        internal event DockContentEventHandler CloseButtonVisibleChanged;
        private void onCloseButtonVisibleChanged(DockContentEventArgs e)
        {
            if (CloseButtonVisibleChanged != null)
            {
                CloseButtonVisibleChanged(null, e);
            }
        }

        internal event DockContentEventHandler CloseButtonEnableChanged;
        private void onCloseButtonEnableChanged(DockContentEventArgs e)
        {
            if (CloseButtonEnableChanged != null)
            {
                CloseButtonEnableChanged(null, e);
            }
        }

        public DockContent(): this(new FormContent())
        {
            m_DockState = DefaultShowState;
        }
        public DockContent(IContent content): this(content, DockState.Unknown)
        {
            m_DockState = DefaultShowState;
        }
        public DockContent(IContent content,DockState dockState)
        {
            this.Tab = new Tab();
            initFormStyle(this);
            m_DockContext = new DockContext(this);
            this.GetPersistStringCallback = new GetPersistStringCallback(GetPersistString);
            m_Content = content;
            m_DockState = dockState;
            initFormContentStyle();
            Control.Dock = DockStyle.Fill;
            this.Controls.Add(Control);

            this.Text = Content.Text;
            this.Icon = Content.Icon;
        }

        private void initFormContentStyle()
        {
            Form f = this.Content as Form;
            if(f==null){
                return;
            }

            initFormStyle(f);

        }
        private void initFormStyle(Form f)
        {
            f.TopLevel = false;
            f.ShowInTaskbar = false;
            f.WindowState = FormWindowState.Normal;
            f.Visible = true;
            f.FormBorderStyle = FormBorderStyle.None;
            NativeMethods.SetWindowPos(f.Handle,
                         IntPtr.Zero, 0, 0, 0, 0,
                         FlagsSetWindowPos.SWP_NOACTIVATE |
                         FlagsSetWindowPos.SWP_NOMOVE |
                         FlagsSetWindowPos.SWP_NOSIZE |
                         FlagsSetWindowPos.SWP_NOZORDER |
                         FlagsSetWindowPos.SWP_NOOWNERZORDER |
                         FlagsSetWindowPos.SWP_FRAMECHANGED);
        }

        private Tab _tab;
        internal Tab Tab
        {
            get { return _tab; }
            set { _tab = value; }
        }

        #region IDock

        public bool IsAutoHide
        {
            get { return DockHelper.IsDockStateAutoHide(this.DockState); }
        }

        public bool IsFloat
        {
            get { return this.DockState == DockState.Float; }
        }

        private DockState m_DockState;
        public DockState DockState
        {
            get { return m_DockState; }
            set
            {
                m_DockState = value;
                if (this.DockPage != null)
                {
                    this.DockPage.RefreshChanges();
                }
            }
        }

        #endregion

        #region DockTo

        private double m_autoHidePortion = 0.25;
        internal double AutoHidePortion
        {
            get { return m_autoHidePortion; }
            set
            {
                if (value <= 0)
                    throw (new ArgumentOutOfRangeException(Strings.DockContentHandler_AutoHidePortion_OutOfRange));

                if (m_autoHidePortion == value)
                    return;

                m_autoHidePortion = value;

                onAutoHidePortionChanged(new DockContentEventArgs(this));

            }
        }

        private DockContext m_DockContext;
        internal DockContext DockContext
        {
            get { return m_DockContext; }
        }
      
        private DockPage m_DockPage;
        internal DockPage DockPage
        {
            get { return m_DockPage; }
            set 
            { 
                if(m_DockPage == value)
                {
                    this.SetParent(value);
                    return;
                }

                if (value == null)
                {
                    this.SetParent(value);
                    return;
                }

                m_DockPage = value;

                DockContext info = this.DockContext;

                if (m_DockPage != null)
                {
                    if (m_DockPage.IsFloat)
                    {
                        info.PrevFloatPage = m_DockPage;
                    }
                    else
                    {
                        info.PrevPanelPage = m_DockPage;
                    }

                }

                this.SetParent(m_DockPage);
                
            }
        }

        private void SetParent(Control value)
        {
            if (this.Parent == value)
                return;
            
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            // Workaround of .Net Framework bug:
            // Change the parent of a control with focus may result in the first
            // MDI child form get activated. 
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            bool bRestoreFocus = false;
            if (this.ContainsFocus)
            {
                //Suggested as a fix for a memory leak by bugreports
                if (value == null && !IsFloat)
                {
                    //DockPanel.ContentFocusManager.GiveUpFocus(this.Content);
                    ;
                }
                else
                {
                    //DockPanel.SaveFocus();
                    bRestoreFocus = true;
                }
            }
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

            this.Parent = value;

            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            // Workaround of .Net Framework bug:
            // Change the parent of a control with focus may result in the first
            // MDI child form get activated. 
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            if (bRestoreFocus)
                Activate();
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        }

        #endregion

        #region IContent

        [Category("Category_Docking")]
        [Description("DockContent_HideOnClose_Description")]
        [DefaultValue(false)]
        public bool HideOnClose
        {
            get { return Content.HideOnClose; }
            set { Content.HideOnClose = value; }
        }

        #region PersistString
        internal string PersistString
        {
            get { return GetPersistStringCallback == null ? this.Content.GetType().ToString() : GetPersistStringCallback(); }
        }
        private GetPersistStringCallback m_getPersistStringCallback = null;
        public GetPersistStringCallback GetPersistStringCallback
        {
            get { return m_getPersistStringCallback; }
            set { m_getPersistStringCallback = value; }
        }
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public virtual string GetPersistString()
        {
            return this.Content.GetPersistString();
        }
        #endregion

        public string ToolTipText
        {
            get { return Content.ToolTipText; }
            set { Content.ToolTipText = value; }
        }

        public DockState DefaultDockState
        {
            get
            {
                if (ShowHint != DockState.Unknown && ShowHint != DockState.Hidden)
                    return ShowHint;

                if ((DockAreas & DockAreas.Document) != 0)
                    return DockState.Document;
                if ((DockAreas & DockAreas.DockRight) != 0)
                    return DockState.DockRight;
                if ((DockAreas & DockAreas.DockLeft) != 0)
                    return DockState.DockLeft;
                if ((DockAreas & DockAreas.DockBottom) != 0)
                    return DockState.DockBottom;
                if ((DockAreas & DockAreas.DockTop) != 0)
                    return DockState.DockTop;

                return DockState.Unknown;
            }
        }

        public DockState DefaultShowState
        {
            get
            {
                if (ShowHint != DockState.Unknown)
                    return ShowHint;

                if ((DockAreas & DockAreas.Document) != 0)
                    return DockState.Document;
                if ((DockAreas & DockAreas.DockRight) != 0)
                    return DockState.DockRight;
                if ((DockAreas & DockAreas.DockLeft) != 0)
                    return DockState.DockLeft;
                if ((DockAreas & DockAreas.DockBottom) != 0)
                    return DockState.DockBottom;
                if ((DockAreas & DockAreas.DockTop) != 0)
                    return DockState.DockTop;
                if ((DockAreas & DockAreas.Float) != 0)
                    return DockState.Float;

                return DockState.Unknown;
            }
        }

        public DockState ShowHint
        {
            get { return this.Content.ShowHint; }
            set { ;}
        }

        public DockAreas DockAreas
        {
            get { return this.Content.DockAreas; }
            set { ;}
        }

        private IContent m_Content;
        public IContent Content
        {
            get { return m_Content; }
        }
        
        public Control Control
        {
            get { return Content as Control; }
        }

        public bool CloseButtonEnable
        {
            get { return Content.CloseButtonEnable; }
            set
            {
                if (Content.CloseButtonEnable == value)
                    return;

                Content.CloseButtonEnable = value;
                onCloseButtonEnableChanged(new DockContentEventArgs(this));
            }
        }

        public bool CloseButtonVisible
        {
            get { return Content.CloseButtonVisible; }
            set 
            {
                if (Content.CloseButtonVisible == value)
                    return;
                Content.CloseButtonVisible = value;

                onCloseButtonVisibleChanged(new DockContentEventArgs(this));
            }
        }

        public ContextMenuStrip TabPageContextMenuStrip
        {
            get { return Content.TabPageContextMenuStrip; }
            set { Content.TabPageContextMenuStrip = value; }
        }

        public ContextMenu TabPageContextMenu
        {
            get { return Content.TabPageContextMenu; }
            set { Content.TabPageContextMenu = value; }
        }

        public Guid GUID
        {
            get { return Content.GUID; }
        }

        public bool AllowEndUserDocking
        {
            get
            {
                return Content.AllowEndUserDocking;
            }
            set
            {
                Content.AllowEndUserDocking = value;
            }
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            Content.Activate();
        }
        
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            this.IsHidden = true;
            Content.Close();

        }

        private bool m_isHidden = false;
        public bool IsHidden
        {
            get { return m_isHidden; }
            set
            {
                if (m_isHidden == value)
                    return;
                m_isHidden = value;
            }
        }

        #endregion

        #region IDockDragSource Members

        Control IDragSource.DragControl
        {
            get { return this.Control; }
        }

        #endregion

    }
}
