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
    public partial class AutoHideStripWindow : AutoHideStripBase
    {
        public AutoHideStripWindow()
        {
            InitializeComponent();

            DockPage page1 = new DockPage();
            page1.DockState = DockState.DockLeftAutoHide;
            this.AddDockPage(page1);

            DockPage page2 = new DockPage();
            page2.DockState = DockState.DockTopAutoHide;
            this.AddDockPage(page2);

            DockPage page3 = new DockPage();
            page3.DockState = DockState.DockRightAutoHide;
            this.AddDockPage(page3);

            DockPage page4 = new DockPage();
            page4.DockState = DockState.DockBottomAutoHide;
            this.AddDockPage(page4);

        }

        #region Paint & Layout
        
        private static DockState[] _dockStates;
        private static DockState[] DockStates
        {
            get
            {
                if (_dockStates == null)
                {
                    _dockStates = new DockState[4];
                    _dockStates[0] = DockState.DockLeftAutoHide;
                    _dockStates[1] = DockState.DockRightAutoHide;
                    _dockStates[2] = DockState.DockTopAutoHide;
                    _dockStates[3] = DockState.DockBottomAutoHide;
                }
                return _dockStates;
            }
        }

        private static StringFormat _stringFormatTabHorizontal;
        private StringFormat StringFormatTabHorizontal
        {
            get
            {
                if (_stringFormatTabHorizontal == null)
                {
                    _stringFormatTabHorizontal = new StringFormat();
                    _stringFormatTabHorizontal.Alignment = StringAlignment.Near;
                    _stringFormatTabHorizontal.LineAlignment = StringAlignment.Center;
                    _stringFormatTabHorizontal.FormatFlags = StringFormatFlags.NoWrap;
                    _stringFormatTabHorizontal.Trimming = StringTrimming.None;
                }

                if (RightToLeft == RightToLeft.Yes)
                    _stringFormatTabHorizontal.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
                else
                    _stringFormatTabHorizontal.FormatFlags &= ~StringFormatFlags.DirectionRightToLeft;

                return _stringFormatTabHorizontal;
            }
        }

        private static StringFormat _stringFormatTabVertical;
        private StringFormat StringFormatTabVertical
        {
            get
            {
                if (_stringFormatTabVertical == null)
                {
                    _stringFormatTabVertical = new StringFormat();
                    _stringFormatTabVertical.Alignment = StringAlignment.Near;
                    _stringFormatTabVertical.LineAlignment = StringAlignment.Center;
                    _stringFormatTabVertical.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.DirectionVertical;
                    _stringFormatTabVertical.Trimming = StringTrimming.None;
                }
                if (RightToLeft == RightToLeft.Yes)
                    _stringFormatTabVertical.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
                else
                    _stringFormatTabVertical.FormatFlags &= ~StringFormatFlags.DirectionRightToLeft;

                return _stringFormatTabVertical;
            }
        }

        private static Matrix _matrixIdentity = new Matrix();
        private static Matrix MatrixIdentity
        {
            get { return _matrixIdentity; }
        }

        private static GraphicsPath _graphicsPath;
        private static GraphicsPath GraphicsPath
        {
            get
            {
                if (_graphicsPath == null)
                    _graphicsPath = new GraphicsPath();

                return _graphicsPath;
            }
        }


        private void DrawTabStrip(Graphics g)
        {
            DrawTabStrip(g, DockState.DockTopAutoHide);
            DrawTabStrip(g, DockState.DockBottomAutoHide);
            DrawTabStrip(g, DockState.DockLeftAutoHide);
            DrawTabStrip(g, DockState.DockRightAutoHide);
        }

        private void DrawTabStrip(Graphics g, DockState dockState)
        {
            Rectangle rectTabStrip = GetLogicalTabStripRectangle(dockState);

            if (rectTabStrip.IsEmpty)
                return;

            Matrix matrixIdentity = g.Transform;
            if (dockState == DockState.DockLeftAutoHide || dockState == DockState.DockRightAutoHide)
            {
                Matrix matrixRotated = new Matrix();
                matrixRotated.RotateAt(90, new PointF(
                                                    (float)rectTabStrip.X + (float)rectTabStrip.Height / 2,
                                                    (float)rectTabStrip.Y + (float)rectTabStrip.Height / 2
                                                    ));
                g.Transform = matrixRotated;
            }

            foreach (DockPage pane in GetDockPages(dockState))
            {
                foreach (var content in pane.DisplayingContents)
                {
                    DrawTab(g, content);
                }
            }
            g.Transform = matrixIdentity;
        }

        private void CalculateTabs()
        {
            CalculateTabs(DockState.DockTopAutoHide);
            CalculateTabs(DockState.DockBottomAutoHide);
            CalculateTabs(DockState.DockLeftAutoHide);
            CalculateTabs(DockState.DockRightAutoHide);
        }

        private void CalculateTabs(DockState dockState)
        {
            Rectangle rectTabStrip = GetLogicalTabStripRectangle(dockState);

            int imageHeight = rectTabStrip.Height - Skin.ImageGapTop - Skin.ImageGapBottom;
            int imageWidth = Skin.ImageWidth;
            if (imageHeight > Skin.ImageHeight)
                imageWidth = Skin.ImageWidth * (imageHeight / Skin.ImageHeight);

            int x = Skin.TabGapLeft + rectTabStrip.X;
            foreach (DockPage pane in GetDockPages(dockState))
            {
                foreach (var content in pane.DisplayingContents)
                {
                    var tab = content.Tab;
                    int width = imageWidth + Skin.ImageGapLeft + Skin.ImageGapRight +
                        TextRenderer.MeasureText(content.Text, Skin.TextFont).Width +
                        Skin.TextGapLeft + Skin.TextGapRight;
                    tab.TabX = x;
                    tab.TabWidth = width;
                    x += width;
                }

                x += Skin.TabGapBetween;
            }
        }

        private Rectangle RtlTransform(Rectangle rect, DockState dockState)
        {
            Rectangle rectTransformed;
            if (dockState == DockState.DockLeftAutoHide || dockState == DockState.DockRightAutoHide)
                rectTransformed = rect;
            else
                rectTransformed = DrawHelper.RtlTransform(this, rect);

            return rectTransformed;
        }

        private GraphicsPath GetTabOutline(DockContent content, bool transformed, bool rtlTransform)
        {
            var tab = content.Tab;
            DockState dockState = content.DockState;
            Rectangle rectTab = GetTabRectangle(content, transformed);
            if (rtlTransform)
                rectTab = RtlTransform(rectTab, dockState);
            bool upTab = (dockState == DockState.DockLeftAutoHide || dockState == DockState.DockBottomAutoHide);
            DrawHelper.GetRoundedCornerTab(GraphicsPath, rectTab, upTab);

            return GraphicsPath;
        }

        private void DrawTab(Graphics g, DockContent content)
        {
            var tab = content.Tab;
            Rectangle rectTabOrigin = GetTabRectangle(content);
            if (rectTabOrigin.IsEmpty)
                return;

            DockState dockState = content.DockState;

            GraphicsPath path = GetTabOutline(content, false, true);

            Color startColor = Skin.TabGradient.StartColor;
            Color endColor = Skin.TabGradient.EndColor;
            LinearGradientMode gradientMode = Skin.TabGradient.LinearGradientMode;
            g.FillPath(new LinearGradientBrush(rectTabOrigin, startColor, endColor, gradientMode), path);
            g.DrawPath(Skin.PenTabBorder, path);

            // Set no rotate for drawing icon and text
            Matrix matrixRotate = g.Transform;
            g.Transform = MatrixIdentity;

            // Draw the icon
            Rectangle rectImage = rectTabOrigin;
            rectImage.X += Skin.ImageGapLeft;
            rectImage.Y += Skin.ImageGapTop;
            int imageHeight = rectTabOrigin.Height - Skin.ImageGapTop - Skin.ImageGapBottom;
            int imageWidth = Skin.ImageWidth;
            if (imageHeight > Skin.ImageHeight)
                imageWidth = Skin.ImageWidth * (imageHeight / Skin.ImageHeight);
            rectImage.Height = imageHeight;
            rectImage.Width = imageWidth;
            rectImage = GetTransformedRectangle(dockState, rectImage);
            if (content.Icon != null)
            {
                g.DrawIcon(content.Icon, RtlTransform(rectImage, dockState));
            }
            // Draw the text
            Rectangle rectText = rectTabOrigin;
            rectText.X += Skin.ImageGapLeft + imageWidth + Skin.ImageGapRight + Skin.TextGapLeft;
            rectText.Width -= Skin.ImageGapLeft + imageWidth + Skin.ImageGapRight + Skin.TextGapLeft;
            rectText = RtlTransform(GetTransformedRectangle(dockState, rectText), dockState);

            Color textColor = Skin.TabGradient.TextColor;

            if (dockState == DockState.DockLeftAutoHide || dockState == DockState.DockRightAutoHide)
                g.DrawString(content.Text, Skin.TextFont, new SolidBrush(textColor), rectText, StringFormatTabVertical);
            else
                g.DrawString(content.Text, Skin.TextFont, new SolidBrush(textColor), rectText, StringFormatTabHorizontal);

            // Set rotate back
            g.Transform = matrixRotate;
        }

        private Rectangle GetLogicalTabStripRectangle(DockState dockState)
        {
            return GetLogicalTabStripRectangle(dockState, false);
        }

        private Rectangle GetLogicalTabStripRectangle(DockState dockState, bool transformed)
        {
            if (!DockHelper.IsDockStateAutoHide(dockState))
                return Rectangle.Empty;

            int leftPanes = GetDockPages(DockState.DockLeftAutoHide).Count;
            int rightPanes = GetDockPages(DockState.DockRightAutoHide).Count;
            int topPanes = GetDockPages(DockState.DockTopAutoHide).Count;
            int bottomPanes = GetDockPages(DockState.DockBottomAutoHide).Count;

            int x, y, width, height;

            height = MeasureHeight();
            if (dockState == DockState.DockLeftAutoHide && leftPanes > 0)
            {
                x = 0;
                y = (topPanes == 0) ? 0 : height;
                width = Height - (topPanes == 0 ? 0 : height) - (bottomPanes == 0 ? 0 : height);
            }
            else if (dockState == DockState.DockRightAutoHide && rightPanes > 0)
            {
                x = Width - height;
                if (leftPanes != 0 && x < height)
                    x = height;
                y = (topPanes == 0) ? 0 : height;
                width = Height - (topPanes == 0 ? 0 : height) - (bottomPanes == 0 ? 0 : height);
            }
            else if (dockState == DockState.DockTopAutoHide && topPanes > 0)
            {
                x = leftPanes == 0 ? 0 : height;
                y = 0;
                width = Width - (leftPanes == 0 ? 0 : height) - (rightPanes == 0 ? 0 : height);
            }
            else if (dockState == DockState.DockBottomAutoHide && bottomPanes > 0)
            {
                x = leftPanes == 0 ? 0 : height;
                y = Height - height;
                if (topPanes != 0 && y < height)
                    y = height;
                width = Width - (leftPanes == 0 ? 0 : height) - (rightPanes == 0 ? 0 : height);
            }
            else
                return Rectangle.Empty;

            if (!transformed)
                return new Rectangle(x, y, width, height);
            else
                return GetTransformedRectangle(dockState, new Rectangle(x, y, width, height));
        }

        private Rectangle GetTabRectangle(DockContent content)
        {
            return GetTabRectangle(content, false);
        }

        private Rectangle GetTabRectangle(DockContent content, bool transformed)
        {
            Tab tab = content.Tab;
            DockState dockState = content.DockState;
            Rectangle rectTabStrip = GetLogicalTabStripRectangle(dockState);

            if (rectTabStrip.IsEmpty)
                return Rectangle.Empty;

            int x = tab.TabX;
            int y = rectTabStrip.Y +
                (dockState == DockState.DockTopAutoHide || dockState == DockState.DockRightAutoHide ?
                0 : Skin.TabGapTop);
            int width = tab.TabWidth;
            int height = rectTabStrip.Height - Skin.TabGapTop;

            if (!transformed)
                return new Rectangle(x, y, width, height);
            else
                return GetTransformedRectangle(dockState, new Rectangle(x, y, width, height));
        }

        private Rectangle GetTransformedRectangle(DockState dockState, Rectangle rect)
        {
            if (dockState != DockState.DockLeftAutoHide && dockState != DockState.DockRightAutoHide)
                return rect;

            PointF[] pts = new PointF[1];
            // the center of the rectangle
            pts[0].X = (float)rect.X + (float)rect.Width / 2;
            pts[0].Y = (float)rect.Y + (float)rect.Height / 2;
            Rectangle rectTabStrip = GetLogicalTabStripRectangle(dockState);
            Matrix matrix = new Matrix();
            matrix.RotateAt(90, new PointF((float)rectTabStrip.X + (float)rectTabStrip.Height / 2,
                (float)rectTabStrip.Y + (float)rectTabStrip.Height / 2));
            matrix.TransformPoints(pts);

            return new Rectangle((int)(pts[0].X - (float)rect.Height / 2 + .5F),
                (int)(pts[0].Y - (float)rect.Width / 2 + .5F),
                rect.Height, rect.Width);
        }

        #endregion

        protected override void OnRefreshChanges()
        {
            CalculateTabs();
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            
            Color startColor = Skin.DockStripGradient.StartColor;
            Color endColor = Skin.DockStripGradient.EndColor;
            LinearGradientMode gradientMode = Skin.DockStripGradient.LinearGradientMode;
            using (LinearGradientBrush brush = new LinearGradientBrush(ClientRectangle, startColor, endColor, gradientMode))
            {
                g.FillRectangle(brush, ClientRectangle);
            }
            CalculateTabs();
            DrawTabStrip(g);
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            CalculateTabs();
            base.OnLayout(levent);
        }

        public override int MeasureHeight()
        {
            return Math.Max(Skin.ImageGapBottom +
                Skin.ImageGapTop + Skin.ImageHeight,
                Skin.TextFont.Height) + Skin.TabGapTop;
        }

        protected override DockContent HitTest(Point ptMouse)
        {
            foreach (DockState state in DockStates)
            {
                Rectangle rectTabStrip = GetLogicalTabStripRectangle(state, true);
                if (!rectTabStrip.Contains(ptMouse))
                    continue;

                foreach (DockPage pane in GetDockPages(state))
                {
                    DockState dockState = pane.DockState;
                    foreach (var content in pane.DisplayingContents)
                    {
                        GraphicsPath path = GetTabOutline(content, true, true);
                        if (path.IsVisible(ptMouse))
                            return content;
                    }
                }
            }

            return null;
        }

     }
}
