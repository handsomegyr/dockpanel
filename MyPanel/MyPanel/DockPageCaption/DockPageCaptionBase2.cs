using System;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.ComponentModel;

namespace MyPanel
{
    public abstract class DockPageCaptionBase : Panel,IDockPageCaptionControl
	{
        protected DockPageCaptionBase()
		{
            this.SuspendLayout();

            SetStyle(
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint, true);

            SetStyle(ControlStyles.Selectable, false);

            m_components = new Container();
            m_toolTip = new ToolTip(Components);

            this.Height = MeasureHeight();

            this.ResumeLayout();
		}

        protected bool ShouldShowAutoHideButton
        {
            get
            {
                return (this.ActiveContent != null) ? ! this.ActiveContent.IsFloat : false;
            }
        }

        protected bool IsAutoHide
        {
            get
            {
                //return DockPaneCaption.DockPane.IsAutoHide;
                return ((this.ActiveContent as DockContent) != null) ? ((DockContent)this.ActiveContent).IsAutoHide : false;
            }

        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            //if (e.Button == MouseButtons.Left &&
            //    this.AllowEndUserDocking &&
            //    this.AllowDockDragAndDrop &&
            //    this.ActiveContent != null &&
            //    !DockHelper.IsDockStateAutoHide(this.ActiveContent.DockState))
            //{
            //    //DockPane.DockPanel.BeginDrag(DockPane);
            //    DockPageManager.BeginDrag(DockPane);
            //}   
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]         
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == (int)Win32.Msgs.WM_LBUTTONDBLCLK)
            {
                //if (DockHelper.IsDockStateAutoHide(DockPane.DockState))
                //{
                //    //DockPane.DockPanel.ActiveAutoHideContent = null;
                //    return;
                //}

                //if (DockPane.IsFloat)
                //    DockPane.RestoreToPanel();
                //else
                //    DockPane.Float();
            }
            base.WndProc(ref m);
        }

        #region IDockPageCaptionControl


        public bool IsActivated
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string CaptionText
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public DockPaneCaptionSkin Skin
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Button DockButton
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Button CloseButton
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public event EventHandler DockButtonClicked;

        public event EventHandler CloseButtonClicked;


        #region Events

        public event EventHandler AutoHideButtonClick;
        protected virtual void OnAutoHideButtonClick(object sender, EventArgs e)
        {
            if (AutoHideButtonClick != null)
                AutoHideButtonClick(sender, e);
        }

        public event EventHandler CloseButtonClick;
        protected virtual void OnCloseButtonClick(object sender, EventArgs e)
        {
            if (CloseButtonClick != null)
                CloseButtonClick(sender, e);
        }

        #endregion

        #endregion

    }
}
