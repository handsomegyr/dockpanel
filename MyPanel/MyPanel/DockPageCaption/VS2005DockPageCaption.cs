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
    public partial class VS2005DockPageCaption : DockPageCaptionBase
    {
        #region Skin
        
        private Color TextColor
        {
            get
            {
                if (this.IsActivated)
                    return Skin.ToolWindowGradient.ActiveCaptionGradient.TextColor;
                else
                    return Skin.ToolWindowGradient.InactiveCaptionGradient.TextColor;
            }
        }

        private Color StartColor
        {
            get
            {
                if (this.IsActivated)
                    return Skin.ToolWindowGradient.ActiveCaptionGradient.StartColor;
                else
                    return Skin.ToolWindowGradient.InactiveCaptionGradient.StartColor;
            }
        }

        private Color EndColor
        {
            get
            {
                if (this.IsActivated)
                    return Skin.ToolWindowGradient.ActiveCaptionGradient.EndColor;
                else
                    return Skin.ToolWindowGradient.InactiveCaptionGradient.EndColor;
            }
        }

        private LinearGradientMode GradientMode
        {
            get
            {
                if (this.IsActivated)
                    return Skin.ToolWindowGradient.ActiveCaptionGradient.LinearGradientMode;
                else
                    return Skin.ToolWindowGradient.InactiveCaptionGradient.LinearGradientMode;
            }
        }

        private TextFormatFlags TextFormat
        {
            get
            {
                TextFormatFlags _textFormat =
                TextFormatFlags.SingleLine |
                TextFormatFlags.EndEllipsis |
                TextFormatFlags.VerticalCenter;

                if (RightToLeft == RightToLeft.No)
                    return _textFormat;
                else
                    return _textFormat | TextFormatFlags.RightToLeft | TextFormatFlags.Right;
            }
        }
        
        #endregion

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            this.Height = this.MeasureHeight();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.CompositingMode = CompositingMode.SourceOver;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            DrawCaption(e.Graphics);
        }
        
        #region #region Paint & Layout
        
        protected int MeasureHeight()
        {
            int height = Skin.TextFont.Height + Skin.TextGapTop + Skin.TextGapBottom;

            if (height < ButtonClose.Image.Height + Skin.ButtonGapTop + Skin.ButtonGapBottom)
                height = ButtonClose.Image.Height + Skin.ButtonGapTop + Skin.ButtonGapBottom;

            return height;
        }

        private void DrawCaption(Graphics g)
        {
            //如果控件工作区矩形的高和宽中有一个为0
            if (ClientRectangle.Width == 0 || ClientRectangle.Height == 0)
                return;

            //控件工作区矩形颜色填充
            Color startColor = this.StartColor;
            Color endColor = this.EndColor;
            LinearGradientMode gradientMode = this.GradientMode;
            using (LinearGradientBrush brush = new LinearGradientBrush(ClientRectangle, startColor, endColor, gradientMode))
            {
                if (this.IsActivated)
                {
                    brush.Blend = Skin.ActiveBackColorGradientBlend;
                }
                g.FillRectangle(brush, ClientRectangle);
            }

            //文字所用空间矩形大小计算
            Rectangle rectCaptionText = ClientRectangle;
            rectCaptionText.X += Skin.TextGapLeft;
            rectCaptionText.Width -= Skin.TextGapLeft + Skin.TextGapRight;
            rectCaptionText.Width -= Skin.ButtonGapLeft + ButtonClose.Width + Skin.ButtonGapRight;
            if (ShouldShowAutoHideButton)
                rectCaptionText.Width -= ButtonAutoHide.Width + Skin.ButtonGapBetween;
            if (HasTabPageContextMenu)
                rectCaptionText.Width -= ButtonOptions.Width + Skin.ButtonGapBetween;
            rectCaptionText.Y += Skin.TextGapTop;
            rectCaptionText.Height -= Skin.TextGapTop + Skin.TextGapBottom;

            //文字显示
            Color colorText = this.TextColor;
            TextRenderer.DrawText(g, this.CaptionText, Skin.TextFont, DrawHelper.RtlTransform(this, rectCaptionText), colorText, TextFormat);
        }

        #endregion
        
        protected override void OnLayout(LayoutEventArgs levent)
        {
            SetButtonsPosition();
            base.OnLayout(levent);
        }
  
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Button == MouseButtons.Right)
                ShowTabPageContextMenu(this, new Point(e.X, e.Y));
        }

        public VS2005DockPageCaption()
        {
            InitializeComponent();

            m_toolTip = new ToolTip(this.components);

            dummyContent.Text = "Caption1";
            this.ActiveContent = dummyContent;
            this.ActiveContentChanged += new DockContentChangedEventHandler(CaptionBase_ActiveContentChanged);

            this.Height = this.MeasureHeight();

        }

        protected DockContent dummyContent = new DockContent();

        private void CaptionBase_ActiveContentChanged(object sender, DockContentChangedEventArgs e)
        {
            this.Height = MeasureHeight();
        }

        #region ToolTip

        private ToolTip m_toolTip;
        protected ToolTip ToolTip
        {
            get { return m_toolTip; }
        }
        #endregion

        #region Button

        private InertButton m_buttonClose;
        private InertButton ButtonClose
        {
            get
            {
                if (m_buttonClose == null)
                {
                    m_buttonClose = new InertButton(Skin.ImageButtonClose, Skin.ImageButtonClose);
                    ToolTip.SetToolTip(m_buttonClose, Skin.ToolTipClose);
                    //m_buttonClose.Click += new EventHandler(Close_Click);
                    Controls.Add(m_buttonClose);
                }

                return m_buttonClose;
            }
        }

        private InertButton m_buttonAutoHide;
        private InertButton ButtonAutoHide
        {
            get
            {
                if (m_buttonAutoHide == null)
                {
                    m_buttonAutoHide = new InertButton(Skin.ImageButtonDock, Skin.ImageButtonAutoHide);
                    ToolTip.SetToolTip(m_buttonAutoHide, Skin.ToolTipAutoHide);
                    //m_buttonAutoHide.Click += new EventHandler(AutoHide_Click);
                    Controls.Add(m_buttonAutoHide);
                }

                return m_buttonAutoHide;
            }
        }

        private InertButton m_buttonOptions;
        private InertButton ButtonOptions
        {
            get
            {
                if (m_buttonOptions == null)
                {
                    m_buttonOptions = new InertButton(Skin.ImageButtonOptions, Skin.ImageButtonOptions);
                    ToolTip.SetToolTip(m_buttonOptions, Skin.ToolTipOptions);
                    m_buttonOptions.Click += new EventHandler(Options_Click);
                    Controls.Add(m_buttonOptions);
                }
                return m_buttonOptions;
            }
        }

        private void SetButtons()
        {
            ButtonClose.ForeColor = this.TextColor;
            ButtonClose.ImageCategory = this.IsAutoHide;
            ButtonClose.Enabled = CloseButtonEnabled;
            ButtonClose.Visible = CloseButtonVisible;

            ButtonAutoHide.ForeColor = this.TextColor;
            ButtonAutoHide.ImageCategory = this.IsAutoHide;
            ButtonAutoHide.Visible = ShouldShowAutoHideButton;

            ButtonOptions.ForeColor = this.TextColor;
            ButtonOptions.ImageCategory = this.IsAutoHide;
            ButtonOptions.Visible = HasTabPageContextMenu;

            ButtonClose.RefreshChanges();
            ButtonAutoHide.RefreshChanges();
            ButtonOptions.RefreshChanges();

            SetButtonsPosition();
        }

        private void SetButtonsPosition()
        {
            // set the size and location for close and auto-hide buttons
            Rectangle rectCaption = ClientRectangle;
            int buttonWidth = ButtonClose.Image.Width;
            int buttonHeight = ButtonClose.Image.Height;
            int height = rectCaption.Height - Skin.ButtonGapTop - Skin.ButtonGapBottom;
            if (buttonHeight < height)
            {
                buttonWidth = buttonWidth * (height / buttonHeight);
                buttonHeight = height;
            }
            Size buttonSize = new Size(buttonWidth, buttonHeight);
            int x = rectCaption.X + rectCaption.Width - 1 - Skin.ButtonGapRight - m_buttonClose.Width;
            int y = rectCaption.Y + Skin.ButtonGapTop;
            Point point = new Point(x, y);
            ButtonClose.Bounds = DrawHelper.RtlTransform(this, new Rectangle(point, buttonSize));

            // If the close button is not visible draw the auto hide button overtop.
            // Otherwise it is drawn to the left of the close button.
            if (CloseButtonVisible)
                point.Offset(-(buttonWidth + Skin.ButtonGapBetween), 0);

            ButtonAutoHide.Bounds = DrawHelper.RtlTransform(this, new Rectangle(point, buttonSize));
            if (ShouldShowAutoHideButton)
                point.Offset(-(buttonWidth + Skin.ButtonGapBetween), 0);
            ButtonOptions.Bounds = DrawHelper.RtlTransform(this, new Rectangle(point, buttonSize));
        }

        //private void Close_Click(object sender, EventArgs e)
        //{
        //    OnCloseButtonClick(sender, e);
        //}

        //private void AutoHide_Click(object sender, EventArgs e)
        //{
        //    OnDockButtonClick(sender, e);
        //}

        private void Options_Click(object sender, EventArgs e)
        {
            ShowTabPageContextMenu(this, PointToClient(Control.MousePosition));
        }

        #region TabPageContextMenu

        private object TabPageContextMenu
        {
            get
            {
                DockContent content = ActiveContent;

                if (content == null)
                    return null;

                if (content.TabPageContextMenuStrip != null)
                    return content.TabPageContextMenuStrip;
                else if (content.TabPageContextMenu != null)
                    return content.TabPageContextMenu;
                else
                    return null;
            }
        }

        protected bool HasTabPageContextMenu
        {
            get { return TabPageContextMenu != null; }
        }

        protected void ShowTabPageContextMenu(Control control, Point position)
        {
            object menu = TabPageContextMenu;

            if (menu == null)
                return;

            ContextMenuStrip contextMenuStrip = menu as ContextMenuStrip;
            if (contextMenuStrip != null)
            {
                contextMenuStrip.Show(control, position);
                return;
            }

            ContextMenu contextMenu = menu as ContextMenu;
            if (contextMenu != null)
                contextMenu.Show(this, position);
        }

        #endregion

        protected bool ShouldShowAutoHideButton
        {
            get
            {
                return (this.ActiveContent != null) ? ! this.ActiveContent.IsFloat : false;
            }
        }

        protected int IsAutoHide
        {
            get
            {
                bool isAutoHide= (this.ActiveContent != null) ? this.ActiveContent.IsAutoHide : false;
                return isAutoHide ? 1 : 0;
            }

        }

        protected bool CloseButtonEnabled
        {
            get
            {
                return (this.ActiveContent != null) ? this.ActiveContent.CloseButtonEnable : false;
            }
        }

        protected bool CloseButtonVisible
        {
            get
            {
                return (this.ActiveContent != null) ? this.ActiveContent.CloseButtonVisible : false;
            }
        }

        #endregion

        public override Control DockButton
        {
            get
            {
                return ButtonAutoHide;
            }
        }

        public override Control CloseButton
        {
            get
            {
                return this.ButtonClose;
            }
        }
        
        public override void RefreshChanges()
        {
            this.OnRefreshChanges();
        }

        protected void OnRefreshChanges()
        {
            SetButtons();
            Invalidate();
        }

    }
}
