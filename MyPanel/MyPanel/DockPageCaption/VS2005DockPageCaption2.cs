using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;
using System.Windows.Forms.VisualStyles;
using System.Drawing.Text;

namespace MyPanel
{
	public class VS2005DockPageCaption : DockPageCaptionBase
	{
        private sealed class InertButton : InertButtonBase
        {
            private Bitmap m_image, m_imageAutoHide;

            public InertButton(VS2005DockPageCaption dockPaneCaption, Bitmap image, Bitmap imageAutoHide)
                : base()
            {
                m_dockPaneCaption = dockPaneCaption;
                m_image = image;
                m_imageAutoHide = imageAutoHide;
                RefreshChanges();
            }

            private VS2005DockPageCaption m_dockPaneCaption;
            private VS2005DockPageCaption DockPaneCaption
            {
                get { return m_dockPaneCaption; }
            }

            public override Bitmap Image
            {
                get { return DockPaneCaption.IsAutoHide ? m_imageAutoHide : m_image; }
            }

            protected override void OnRefreshChanges()
            {
                //if (DockPaneCaption.DockPane.DockPanel != null)
                {
                    if (DockPaneCaption.TextColor != ForeColor)
                    {
                        ForeColor = DockPaneCaption.TextColor;
                        Invalidate();
                    }
                }
            }
        }
		
        private InertButton m_buttonClose;
        private InertButton ButtonClose
        {
            get
            {
                if (m_buttonClose == null)
                {
                    m_buttonClose = new InertButton(this, Skin.ImageButtonClose, Skin.ImageButtonClose);
                    ToolTip.SetToolTip(m_buttonClose, Skin.ToolTipClose);
                    m_buttonClose.Click += new EventHandler(Close_Click);
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
                    m_buttonAutoHide = new InertButton(this, Skin.ImageButtonDock, Skin.ImageButtonAutoHide);
                    ToolTip.SetToolTip(m_buttonAutoHide, Skin.ToolTipAutoHide);
                    m_buttonAutoHide.Click += new EventHandler(AutoHide_Click);
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
                    m_buttonOptions = new InertButton(this, Skin.ImageButtonOptions, Skin.ImageButtonOptions);
                    ToolTip.SetToolTip(m_buttonOptions, Skin.ToolTipOptions);
                    m_buttonOptions.Click += new EventHandler(Options_Click);
                    Controls.Add(m_buttonOptions);
                }
                return m_buttonOptions;
            }
        }


        protected DockContent dummyContent = new DockContent();

        public VS2005DockPageCaption() : base()
        {
            dummyContent.Text = "Caption1";
            this.ActiveContent = dummyContent;
            this.ActiveContentChanged += new ContentChangedEventHandler(CaptionBase_ActiveContentChanged);

        }

        void CaptionBase_ActiveContentChanged(object sender, ContentChangedEventArgs e)
        {
            this.Height = MeasureHeight();
        }

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

		public override int MeasureHeight()
		{
            int height = Skin.TextFont.Height + Skin.TextGapTop + Skin.TextGapBottom;

            if (height < ButtonClose.Image.Height + Skin.ButtonGapTop + Skin.ButtonGapBottom)
                height = ButtonClose.Image.Height + Skin.ButtonGapTop + Skin.ButtonGapBottom;

			return height;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint (e);
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.CompositingMode = CompositingMode.SourceOver;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

			DrawCaption(e.Graphics);
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
            Color colorText= this.TextColor;
            TextRenderer.DrawText(g, this.CaptionText, Skin.TextFont, DrawHelper.RtlTransform(this, rectCaptionText), colorText, TextFormat);
		}

		protected override void OnLayout(LayoutEventArgs levent)
		{
			SetButtonsPosition();
			base.OnLayout (levent);
		}

		protected override void OnRefreshChanges()
		{
			SetButtons();
			Invalidate();
		}

		private void SetButtons()
		{
			ButtonClose.Enabled = CloseButtonEnabled;
            ButtonClose.Visible = CloseButtonVisible;

			ButtonAutoHide.Visible = ShouldShowAutoHideButton;

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

		private void Close_Click(object sender, EventArgs e)
		{
            OnCloseButtonClick(sender, e);
		}

		private void AutoHide_Click(object sender, EventArgs e)
		{
            OnAutoHideButtonClick(sender,e);
		}

        private void Options_Click(object sender, EventArgs e)
        {
            ShowTabPageContextMenu(this,PointToClient(Control.MousePosition));
        }

	}
}
