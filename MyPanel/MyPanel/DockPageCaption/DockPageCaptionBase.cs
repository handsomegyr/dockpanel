using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Security.Permissions;

namespace Guoyongrong.WinFormsUI.Docking
{
    public abstract class DockPageCaptionBase : Panel
    {
        protected DockPageCaptionBase(){
            
            SetStyle(
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint, true);

            SetStyle(ControlStyles.Selectable, false);
        }

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

        private DockContent m_activeContent = null;
        public DockContent ActiveContent
        {
            get { return m_activeContent; }
            set
            {
                if (ActiveContent == value)
                    return;

                //if (value == null)
                //{
                //    throw (new InvalidOperationException(Strings.DockPane_ActiveContent_InvalidValue));
                //}

                DockContent oldValue = m_activeContent;

                m_activeContent = value;

                if (m_activeContent != null)
                    m_activeContent.Visible = true;

                //if (oldValue != null)
                //    oldValue.Visible = false;

                DockContentChangedEventArgs e = new DockContentChangedEventArgs(oldValue, m_activeContent);

                OnActiveContentChanged(e);

                RefreshChanges();
            }
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
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Left)
            {
                OnDockContentClicked(new DockContentEventArgs(this.ActiveContent));
            }
        }
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == (int)Win32.Msgs.WM_LBUTTONDBLCLK)
            {
                OnDockContentDoubleClicked(new DockContentEventArgs(this.ActiveContent));
            }
            base.WndProc(ref m);
        }

        private bool m_isActivated = false;
        public bool IsActivated
        {
            get
            {
                return m_isActivated;
            }
            set
            {
                if (m_isActivated == value)
                    return;

                m_isActivated = value;
                RefreshChanges();
            }
        }

        private DockPaneCaptionSkin m_skin = new DockPaneCaptionSkin();
        public DockPaneCaptionSkin Skin
        {
            get
            {
                return m_skin;
            }
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

        public string CaptionText
        {
            get
            {
                return ActiveContent == null ? string.Empty : ActiveContent.Text;
            }
        }

        public abstract Control DockButton { get; }

        public abstract Control CloseButton { get; }
        
        public abstract void RefreshChanges();

    }
}
