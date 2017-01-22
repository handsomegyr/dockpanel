using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Security.Permissions;

namespace Guoyongrong.WinFormsUI.Docking
{
    public partial class VS2005DockPageStrip : DockPageStripBase
    {
        #region Skin
        
        private TextFormatFlags ToolWindowTextFormat
        {
            get
            {
                TextFormatFlags textFormat = TextFormatFlags.EndEllipsis |
                    TextFormatFlags.HorizontalCenter |
                    TextFormatFlags.SingleLine |
                    TextFormatFlags.VerticalCenter;
                if (RightToLeft == RightToLeft.Yes)
                    return textFormat | TextFormatFlags.RightToLeft | TextFormatFlags.Right;
                else
                    return textFormat;
            }
        }
        private TextFormatFlags DocumentTextFormat
        {
            get
            {
                TextFormatFlags textFormat = TextFormatFlags.EndEllipsis |
                    TextFormatFlags.SingleLine |
                    TextFormatFlags.VerticalCenter |
                    TextFormatFlags.HorizontalCenter;
                if (RightToLeft == RightToLeft.Yes)
                    return textFormat | TextFormatFlags.RightToLeft;
                else
                    return textFormat;
            }
        }
        
        private int m_startDisplayingTab = 0;
        private int StartDisplayingTab
        {
            get { return m_startDisplayingTab; }
            set
            {
                m_startDisplayingTab = value;
                Invalidate();
            }
        }

        private int m_endDisplayingTab = 0;
        private int EndDisplayingTab
        {
            get { return m_endDisplayingTab; }
            set { m_endDisplayingTab = value; }
        }

        private int m_firstDisplayingTab = 0;
        private int FirstDisplayingTab
        {
            get { return m_firstDisplayingTab; }
            set { m_firstDisplayingTab = value; }
        }

        private bool m_documentTabsOverflow = false;
        private bool DocumentTabsOverflow
        {
            set
            {
                if (m_documentTabsOverflow == value)
                    return;

                m_documentTabsOverflow = value;
                if (value)
                    ButtonWindowList.ImageCategory = 1;
                else
                    ButtonWindowList.ImageCategory = 0;
            }
        }
        
        #endregion

        private GraphicsPath _graphicsPath = new GraphicsPath();
        private GraphicsPath GraphicsPath
        {
            get
            {
                return _graphicsPath;
            }
        }
        private int MeasureHeight()
		{
			if (Appearance == AppearanceStyle.ToolWindow)
				return MeasureHeight_ToolWindow();
			else
				return MeasureHeight_Document();
		}
		private void OnRefreshChanges()
		{
			SetInertButtons();
			Invalidate();
            this.PerformLayout();
		}

        #region Paint & Layout

        private int MeasureHeight_ToolWindow()
        {
            if (
                //DockPane.IsAutoHide || 
                this.DisplayingContents.Count <= 1)
                return 0;

            int height =
                Math.Max(Skin.TextFont.Height,
                Skin.ToolWindowImageHeight + Skin.ToolWindowImageGapTop + Skin.ToolWindowImageGapBottom)
                + Skin.ToolWindowStripGapTop + Skin.ToolWindowStripGapBottom;

            return height;
        }

        private int MeasureHeight_Document()
        {
            int height = Math.Max(Skin.TextFont.Height + Skin.DocumentTabGapTop,
                ButtonClose.Height + Skin.DocumentButtonGapTop + Skin.DocumentButtonGapBottom)
                + Skin.DocumentStripGapBottom + Skin.DocumentStripGapTop;

            return height;
        }

        private GraphicsPath GetOutline_Document(int index)
        {
            Rectangle rectTab = GetTabRectangle(index);
            rectTab.X -= rectTab.Height / 2;
            rectTab.Intersect(TabsRectangle);
            rectTab = RectangleToScreen(DrawHelper.RtlTransform(this, rectTab));
            //Rectangle rectPaneClient = DockPane.RectangleToScreen(DockPane.ClientRectangle);
            Rectangle rectPaneClient = this.Parent.RectangleToScreen(this.Parent.ClientRectangle);

            GraphicsPath path = new GraphicsPath();
            GraphicsPath pathTab = GetTabOutline_Document(this.DisplayingContents[index], true, true, true);
            path.AddPath(pathTab, true);

            if (this.DocumentTabStripLocation == DocumentTabStripLocation.Bottom)
            {
                path.AddLine(rectTab.Right, rectTab.Top, rectPaneClient.Right, rectTab.Top);
                path.AddLine(rectPaneClient.Right, rectTab.Top, rectPaneClient.Right, rectPaneClient.Top);
                path.AddLine(rectPaneClient.Right, rectPaneClient.Top, rectPaneClient.Left, rectPaneClient.Top);
                path.AddLine(rectPaneClient.Left, rectPaneClient.Top, rectPaneClient.Left, rectTab.Top);
                path.AddLine(rectPaneClient.Left, rectTab.Top, rectTab.Right, rectTab.Top);
            }
            else
            {
                path.AddLine(rectTab.Right, rectTab.Bottom, rectPaneClient.Right, rectTab.Bottom);
                path.AddLine(rectPaneClient.Right, rectTab.Bottom, rectPaneClient.Right, rectPaneClient.Bottom);
                path.AddLine(rectPaneClient.Right, rectPaneClient.Bottom, rectPaneClient.Left, rectPaneClient.Bottom);
                path.AddLine(rectPaneClient.Left, rectPaneClient.Bottom, rectPaneClient.Left, rectTab.Bottom);
                path.AddLine(rectPaneClient.Left, rectTab.Bottom, rectTab.Right, rectTab.Bottom);
            }
            return path;
        }

        private GraphicsPath GetOutline_ToolWindow(int index)
        {
            Rectangle rectTab = GetTabRectangle(index);
            rectTab.Intersect(TabsRectangle);
            rectTab = RectangleToScreen(DrawHelper.RtlTransform(this, rectTab));
            int y = rectTab.Top;
            //Rectangle rectPaneClient = DockPane.RectangleToScreen(DockPane.ClientRectangle);
            Rectangle rectPaneClient = this.Parent.RectangleToScreen(Parent.ClientRectangle);

            GraphicsPath path = new GraphicsPath();
            GraphicsPath pathTab = GetTabOutline(this.DisplayingContents[index], true, true);
            path.AddPath(pathTab, true);
            path.AddLine(rectTab.Left, rectTab.Top, rectPaneClient.Left, rectTab.Top);
            path.AddLine(rectPaneClient.Left, rectTab.Top, rectPaneClient.Left, rectPaneClient.Top);
            path.AddLine(rectPaneClient.Left, rectPaneClient.Top, rectPaneClient.Right, rectPaneClient.Top);
            path.AddLine(rectPaneClient.Right, rectPaneClient.Top, rectPaneClient.Right, rectTab.Top);
            path.AddLine(rectPaneClient.Right, rectTab.Top, rectTab.Right, rectTab.Top);
            return path;
        }

        private void CalculateTabs()
        {
            if (Appearance == AppearanceStyle.ToolWindow)
                CalculateTabs_ToolWindow();
            else
                CalculateTabs_Document();
        }

        private void CalculateTabs_ToolWindow()
        {
            if (this.DisplayingContents.Count <= 1
                //|| DockPane.IsAutoHide
                )
                return;

            Rectangle rectTabStrip = TabStripRectangle;

            // Calculate tab widths
            int countTabs = this.DisplayingContents.Count;
            foreach (DockContent content in this.DisplayingContents)
            {
                var tab = content.Tab;
                tab.MaxWidth = GetMaxTabWidth(this.DisplayingContents.IndexOf(content));
                tab.Flag = false;
            }

            // Set tab whose max width less than average width
            bool anyWidthWithinAverage = true;
            int totalWidth = rectTabStrip.Width - Skin.ToolWindowStripGapLeft - Skin.ToolWindowStripGapRight;
            int totalAllocatedWidth = 0;
            int averageWidth = totalWidth / countTabs;
            int remainedTabs = countTabs;
            for (anyWidthWithinAverage = true; anyWidthWithinAverage && remainedTabs > 0; )
            {
                anyWidthWithinAverage = false;
                foreach (DockContent content in this.DisplayingContents)
                {
                    var tab = content.Tab;
                    if (tab.Flag)
                        continue;

                    if (tab.MaxWidth <= averageWidth)
                    {
                        tab.Flag = true;
                        tab.TabWidth = tab.MaxWidth;
                        totalAllocatedWidth += tab.TabWidth;
                        anyWidthWithinAverage = true;
                        remainedTabs--;
                    }
                }
                if (remainedTabs != 0)
                    averageWidth = (totalWidth - totalAllocatedWidth) / remainedTabs;
            }

            // If any tab width not set yet, set it to the average width
            if (remainedTabs > 0)
            {
                int roundUpWidth = (totalWidth - totalAllocatedWidth) - (averageWidth * remainedTabs);
                foreach (DockContent content in this.DisplayingContents)
                {
                    var tab = content.Tab;
                    if (tab.Flag)
                        continue;

                    tab.Flag = true;
                    if (roundUpWidth > 0)
                    {
                        tab.TabWidth = averageWidth + 1;
                        roundUpWidth--;
                    }
                    else
                        tab.TabWidth = averageWidth;
                }
            }

            // Set the X position of the tabs
            int x = rectTabStrip.X + Skin.ToolWindowStripGapLeft;
            foreach (DockContent content in this.DisplayingContents)
            {
                var tab = content.Tab;
                tab.TabX = x;
                x += tab.TabWidth;
            }
        }

        private bool CalculateDocumentTab(Rectangle rectTabStrip, ref int x, int index)
        {
            bool overflow = false;

            Tab tab = this.DisplayingContents[index].Tab;
            tab.MaxWidth = GetMaxTabWidth(index);
            int width = Math.Min(tab.MaxWidth, Skin.DocumentTabMaxWidth);
            if (x + width < rectTabStrip.Right || index == StartDisplayingTab)
            {
                tab.TabX = x;
                tab.TabWidth = width;
                EndDisplayingTab = index;
            }
            else
            {
                tab.TabX = 0;
                tab.TabWidth = 0;
                overflow = true;
            }
            x += width;

            return overflow;
        }

        private void CalculateTabs_Document()
        {
            if (m_startDisplayingTab >= this.DisplayingContents.Count)
                m_startDisplayingTab = 0;

            Rectangle rectTabStrip = TabsRectangle;

            int x = rectTabStrip.X + rectTabStrip.Height / 2;
            bool overflow = false;

            // Originally all new documents that were considered overflow
            // (not enough pane strip space to show all tabs) were added to
            // the far left (assuming not right to left) and the tabs on the
            // right were dropped from view. If StartDisplayingTab is not 0
            // then we are dealing with making sure a specific tab is kept in focus.
            if (m_startDisplayingTab > 0)
            {
                int tempX = x;
                Tab tab = this.DisplayingContents[m_startDisplayingTab].Tab;
                tab.MaxWidth = GetMaxTabWidth(m_startDisplayingTab);
                int width = Math.Min(tab.MaxWidth, Skin.DocumentTabMaxWidth);

                // Add the active tab and tabs to the left
                for (int i = StartDisplayingTab; i >= 0; i--)
                    CalculateDocumentTab(rectTabStrip, ref tempX, i);

                // Store which tab is the first one displayed so that it
                // will be drawn correctly (without part of the tab cut off)
                FirstDisplayingTab = EndDisplayingTab;

                tempX = x; // Reset X location because we are starting over

                // Start with the first tab displayed - name is a little misleading.
                // Loop through each tab and set its location. If there is not enough
                // room for all of them overflow will be returned.
                for (int i = EndDisplayingTab; i < this.DisplayingContents.Count; i++)
                    overflow = CalculateDocumentTab(rectTabStrip, ref tempX, i);

                // If not all tabs are shown then we have an overflow.
                if (FirstDisplayingTab != 0)
                    overflow = true;
            }
            else
            {
                for (int i = StartDisplayingTab; i < this.DisplayingContents.Count; i++)
                    overflow = CalculateDocumentTab(rectTabStrip, ref x, i);
                for (int i = 0; i < StartDisplayingTab; i++)
                    overflow = CalculateDocumentTab(rectTabStrip, ref x, i);

                FirstDisplayingTab = StartDisplayingTab;
            }

            if (!overflow)
            {
                m_startDisplayingTab = 0;
                FirstDisplayingTab = 0;
                x = rectTabStrip.X + rectTabStrip.Height / 2;
                foreach (DockContent content in this.DisplayingContents)
                {
                    var tab = content.Tab;
                    tab.TabX = x;
                    x += tab.TabWidth;
                }
            }
            DocumentTabsOverflow = overflow;
        }

        private void EnsureTabVisible(DockContent content)
        {
            if (Appearance != AppearanceStyle.Document || !this.DisplayingContents.Contains(content))
                return;

            CalculateTabs();
            EnsureDocumentTabVisible(content, true);
        }

        private bool EnsureDocumentTabVisible(DockContent content, bool repaint)
        {
            int index = this.DisplayingContents.IndexOf(content);
            Tab tab = this.DisplayingContents[index].Tab;
            if (tab.TabWidth != 0)
                return false;

            StartDisplayingTab = index;
            if (repaint)
                Invalidate();

            return true;
        }

        private int GetMaxTabWidth(int index)
        {
            if (Appearance == AppearanceStyle.ToolWindow)
                return GetMaxTabWidth_ToolWindow(index);
            else
                return GetMaxTabWidth_Document(index);
        }

        private int GetMaxTabWidth_ToolWindow(int index)
        {
            DockContent content = this.DisplayingContents[index];
            Size sizeString = TextRenderer.MeasureText(content.Text, Skin.TextFont);
            return Skin.ToolWindowImageWidth + sizeString.Width + Skin.ToolWindowImageGapLeft
                + Skin.ToolWindowImageGapRight + Skin.ToolWindowTextGapRight;
        }

        private int GetMaxTabWidth_Document(int index)
        {
            DockContent content = this.DisplayingContents[index];

            int height = GetTabRectangle_Document(index).Height;

            Size sizeText = TextRenderer.MeasureText(content.Text, Skin.BoldFont, new Size(Skin.DocumentTabMaxWidth, height), DocumentTextFormat);

            if (this.ShowDocumentIcon)
                return sizeText.Width + Skin.DocumentIconWidth + Skin.DocumentIconGapLeft + Skin.DocumentIconGapRight + Skin.DocumentTextGapRight;
            else
                return sizeText.Width + Skin.DocumentIconGapLeft + Skin.DocumentTextGapRight;
        }

        private void DrawTabStrip(Graphics g)
        {
            if (Appearance == AppearanceStyle.Document)
                DrawTabStrip_Document(g);
            else
                DrawTabStrip_ToolWindow(g);
        }

        private void DrawTabStrip_Document(Graphics g)
        {
            int count = this.DisplayingContents.Count;
            if (count == 0)
                return;

            Rectangle rectTabStrip = TabStripRectangle;

            // Draw the tabs
            Rectangle rectTabOnly = TabsRectangle;
            Rectangle rectTab = Rectangle.Empty;
            Tab tabActive = null;
            DockContent activeContent = null;
            g.SetClip(DrawHelper.RtlTransform(this, rectTabOnly));
            for (int i = 0; i < count; i++)
            {
                rectTab = GetTabRectangle(i);
                if (this.DisplayingContents[i] == this.ActiveContent)
                {
                    tabActive = this.DisplayingContents[i].Tab;
                    activeContent = this.DisplayingContents[i];
                    continue;
                }
                if (rectTab.IntersectsWith(rectTabOnly))
                    DrawTab(g, this.DisplayingContents[i], rectTab);
            }

            g.SetClip(rectTabStrip);

            if (this.DocumentTabStripLocation == DocumentTabStripLocation.Bottom)
                g.DrawLine(Skin.PenDocumentTabActiveBorder, rectTabStrip.Left, rectTabStrip.Top + 1,
                    rectTabStrip.Right, rectTabStrip.Top + 1);
            else
                g.DrawLine(Skin.PenDocumentTabActiveBorder, rectTabStrip.Left, rectTabStrip.Bottom - 1,
                    rectTabStrip.Right, rectTabStrip.Bottom - 1);

            g.SetClip(DrawHelper.RtlTransform(this, rectTabOnly));
            if (tabActive != null)
            {
                rectTab = GetTabRectangle(this.DisplayingContents.IndexOf(activeContent));
                if (rectTab.IntersectsWith(rectTabOnly))
                    DrawTab(g, activeContent, rectTab);
            }
        }

        private void DrawTabStrip_ToolWindow(Graphics g)
        {
            Rectangle rectTabStrip = TabStripRectangle;

            g.DrawLine(Skin.PenToolWindowTabBorder, rectTabStrip.Left, rectTabStrip.Top,
                rectTabStrip.Right, rectTabStrip.Top);

            for (int i = 0; i < this.DisplayingContents.Count; i++)
                DrawTab(g, this.DisplayingContents[i], GetTabRectangle(i));
        }

        private Rectangle GetTabRectangle(int index)
        {
            if (Appearance == AppearanceStyle.ToolWindow)
                return GetTabRectangle_ToolWindow(index);
            else
                return GetTabRectangle_Document(index);
        }

        private Rectangle GetTabRectangle_ToolWindow(int index)
        {
            Rectangle rectTabStrip = TabStripRectangle;

            Tab tab = this.DisplayingContents[index].Tab;
            return new Rectangle(tab.TabX, rectTabStrip.Y, tab.TabWidth, rectTabStrip.Height);
        }

        private Rectangle GetTabRectangle_Document(int index)
        {
            Rectangle rectTabStrip = TabStripRectangle;
            Tab tab = this.DisplayingContents[index].Tab;

            Rectangle rect = new Rectangle();
            rect.X = tab.TabX;
            rect.Width = tab.TabWidth;
            rect.Height = rectTabStrip.Height - Skin.DocumentTabGapTop;

            if (this.DocumentTabStripLocation == DocumentTabStripLocation.Bottom)
                rect.Y = rectTabStrip.Y + Skin.DocumentStripGapBottom;
            else
                rect.Y = rectTabStrip.Y + Skin.DocumentTabGapTop;

            return rect;
        }

        private void DrawTab(Graphics g, DockContent content, Rectangle rect)
        {
            if (Appearance == AppearanceStyle.ToolWindow)
                DrawTab_ToolWindow(g, content, rect);
            else
                DrawTab_Document(g, content, rect);
        }

        private GraphicsPath GetTabOutline(DockContent content, bool rtlTransform, bool toScreen)
        {
            if (Appearance == AppearanceStyle.ToolWindow)
                return GetTabOutline_ToolWindow(content, rtlTransform, toScreen);
            else
                return GetTabOutline_Document(content, rtlTransform, toScreen, false);
        }

        private GraphicsPath GetTabOutline_ToolWindow(DockContent content, bool rtlTransform, bool toScreen)
        {
            Rectangle rect = GetTabRectangle(this.DisplayingContents.IndexOf(content));
            if (rtlTransform)
                rect = DrawHelper.RtlTransform(this, rect);
            if (toScreen)
                rect = RectangleToScreen(rect);

            DrawHelper.GetRoundedCornerTab(GraphicsPath, rect, false);
            return GraphicsPath;
        }

        private GraphicsPath GetTabOutline_Document(DockContent content, bool rtlTransform, bool toScreen, bool full)
        {
            int curveSize = 6;

            GraphicsPath.Reset();
            Rectangle rect = GetTabRectangle(this.DisplayingContents.IndexOf(content));
            if (rtlTransform)
                rect = DrawHelper.RtlTransform(this, rect);
            if (toScreen)
                rect = RectangleToScreen(rect);

            // Draws the full angle piece for active content (or first tab)
            if (content == this.ActiveContent || full || this.DisplayingContents.IndexOf(content) == FirstDisplayingTab)
            {
                if (RightToLeft == RightToLeft.Yes)
                {
                    if (this.DocumentTabStripLocation == DocumentTabStripLocation.Bottom)
                    {
                        // For some reason the next line draws a line that is not hidden like it is when drawing the tab strip on top.
                        // It is not needed so it has been commented out.
                        //GraphicsPath.AddLine(rect.Right, rect.Bottom, rect.Right + rect.Height / 2, rect.Bottom);
                        GraphicsPath.AddLine(rect.Right + rect.Height / 2, rect.Top, rect.Right - rect.Height / 2 + curveSize / 2, rect.Bottom - curveSize / 2);
                    }
                    else
                    {
                        GraphicsPath.AddLine(rect.Right, rect.Bottom, rect.Right + rect.Height / 2, rect.Bottom);
                        GraphicsPath.AddLine(rect.Right + rect.Height / 2, rect.Bottom, rect.Right - rect.Height / 2 + curveSize / 2, rect.Top + curveSize / 2);
                    }
                }
                else
                {
                    if (this.DocumentTabStripLocation == DocumentTabStripLocation.Bottom)
                    {
                        // For some reason the next line draws a line that is not hidden like it is when drawing the tab strip on top.
                        // It is not needed so it has been commented out.
                        //GraphicsPath.AddLine(rect.Left, rect.Top, rect.Left - rect.Height / 2, rect.Top);
                        GraphicsPath.AddLine(rect.Left - rect.Height / 2, rect.Top, rect.Left + rect.Height / 2 - curveSize / 2, rect.Bottom - curveSize / 2);
                    }
                    else
                    {
                        GraphicsPath.AddLine(rect.Left, rect.Bottom, rect.Left - rect.Height / 2, rect.Bottom);
                        GraphicsPath.AddLine(rect.Left - rect.Height / 2, rect.Bottom, rect.Left + rect.Height / 2 - curveSize / 2, rect.Top + curveSize / 2);
                    }
                }
            }
            // Draws the partial angle for non-active content
            else
            {
                if (RightToLeft == RightToLeft.Yes)
                {
                    if (this.DocumentTabStripLocation == DocumentTabStripLocation.Bottom)
                    {
                        GraphicsPath.AddLine(rect.Right, rect.Top, rect.Right, rect.Top + rect.Height / 2);
                        GraphicsPath.AddLine(rect.Right, rect.Top + rect.Height / 2, rect.Right - rect.Height / 2 + curveSize / 2, rect.Bottom - curveSize / 2);
                    }
                    else
                    {
                        GraphicsPath.AddLine(rect.Right, rect.Bottom, rect.Right, rect.Bottom - rect.Height / 2);
                        GraphicsPath.AddLine(rect.Right, rect.Bottom - rect.Height / 2, rect.Right - rect.Height / 2 + curveSize / 2, rect.Top + curveSize / 2);
                    }
                }
                else
                {
                    if (this.DocumentTabStripLocation == DocumentTabStripLocation.Bottom)
                    {
                        GraphicsPath.AddLine(rect.Left, rect.Top, rect.Left, rect.Top + rect.Height / 2);
                        GraphicsPath.AddLine(rect.Left, rect.Top + rect.Height / 2, rect.Left + rect.Height / 2 - curveSize / 2, rect.Bottom - curveSize / 2);
                    }
                    else
                    {
                        GraphicsPath.AddLine(rect.Left, rect.Bottom, rect.Left, rect.Bottom - rect.Height / 2);
                        GraphicsPath.AddLine(rect.Left, rect.Bottom - rect.Height / 2, rect.Left + rect.Height / 2 - curveSize / 2, rect.Top + curveSize / 2);
                    }
                }
            }

            if (RightToLeft == RightToLeft.Yes)
            {
                if (this.DocumentTabStripLocation == DocumentTabStripLocation.Bottom)
                {
                    // Draws the bottom horizontal line (short side)
                    GraphicsPath.AddLine(rect.Right - rect.Height / 2 - curveSize / 2, rect.Bottom, rect.Left + curveSize / 2, rect.Bottom);

                    // Drawing the rounded corner is not necessary. The path is automatically connected
                    //GraphicsPath.AddArc(new Rectangle(rect.Left, rect.Top, curveSize, curveSize), 180, 90);
                }
                else
                {
                    // Draws the bottom horizontal line (short side)
                    GraphicsPath.AddLine(rect.Right - rect.Height / 2 - curveSize / 2, rect.Top, rect.Left + curveSize / 2, rect.Top);
                    GraphicsPath.AddArc(new Rectangle(rect.Left, rect.Top, curveSize, curveSize), 180, 90);
                }
            }
            else
            {
                if (this.DocumentTabStripLocation == DocumentTabStripLocation.Bottom)
                {
                    // Draws the bottom horizontal line (short side)
                    GraphicsPath.AddLine(rect.Left + rect.Height / 2 + curveSize / 2, rect.Bottom, rect.Right - curveSize / 2, rect.Bottom);

                    // Drawing the rounded corner is not necessary. The path is automatically connected
                    //GraphicsPath.AddArc(new Rectangle(rect.Right - curveSize, rect.Bottom, curveSize, curveSize), 90, -90);
                }
                else
                {
                    // Draws the top horizontal line (short side)
                    GraphicsPath.AddLine(rect.Left + rect.Height / 2 + curveSize / 2, rect.Top, rect.Right - curveSize / 2, rect.Top);

                    // Draws the rounded corner oppposite the angled side
                    GraphicsPath.AddArc(new Rectangle(rect.Right - curveSize, rect.Top, curveSize, curveSize), -90, 90);
                }
            }

            if (this.DisplayingContents.IndexOf(content) != EndDisplayingTab &&
                (this.DisplayingContents.IndexOf(content) != this.DisplayingContents.Count - 1 &&
                this.DisplayingContents[this.DisplayingContents.IndexOf(content) + 1] == this.ActiveContent) && !full)
            {
                if (RightToLeft == RightToLeft.Yes)
                {
                    if (this.DocumentTabStripLocation == DocumentTabStripLocation.Bottom)
                    {
                        GraphicsPath.AddLine(rect.Left, rect.Bottom - curveSize / 2, rect.Left, rect.Bottom - rect.Height / 2);
                        GraphicsPath.AddLine(rect.Left, rect.Bottom - rect.Height / 2, rect.Left + rect.Height / 2, rect.Top);
                    }
                    else
                    {
                        GraphicsPath.AddLine(rect.Left, rect.Top + curveSize / 2, rect.Left, rect.Top + rect.Height / 2);
                        GraphicsPath.AddLine(rect.Left, rect.Top + rect.Height / 2, rect.Left + rect.Height / 2, rect.Bottom);
                    }
                }
                else
                {
                    if (this.DocumentTabStripLocation == DocumentTabStripLocation.Bottom)
                    {
                        GraphicsPath.AddLine(rect.Right, rect.Bottom - curveSize / 2, rect.Right, rect.Bottom - rect.Height / 2);
                        GraphicsPath.AddLine(rect.Right, rect.Bottom - rect.Height / 2, rect.Right - rect.Height / 2, rect.Top);
                    }
                    else
                    {
                        GraphicsPath.AddLine(rect.Right, rect.Top + curveSize / 2, rect.Right, rect.Top + rect.Height / 2);
                        GraphicsPath.AddLine(rect.Right, rect.Top + rect.Height / 2, rect.Right - rect.Height / 2, rect.Bottom);
                    }
                }
            }
            else
            {
                // Draw the vertical line opposite the angled side
                if (RightToLeft == RightToLeft.Yes)
                {
                    if (this.DocumentTabStripLocation == DocumentTabStripLocation.Bottom)
                        GraphicsPath.AddLine(rect.Left, rect.Bottom - curveSize / 2, rect.Left, rect.Top);
                    else
                        GraphicsPath.AddLine(rect.Left, rect.Top + curveSize / 2, rect.Left, rect.Bottom);
                }
                else
                {
                    if (this.DocumentTabStripLocation == DocumentTabStripLocation.Bottom)
                        GraphicsPath.AddLine(rect.Right, rect.Bottom - curveSize / 2, rect.Right, rect.Top);
                    else
                        GraphicsPath.AddLine(rect.Right, rect.Top + curveSize / 2, rect.Right, rect.Bottom);
                }
            }

            return GraphicsPath;
        }

        private void DrawTab_ToolWindow(Graphics g, DockContent content, Rectangle rect)
        {
            Rectangle rectIcon = new Rectangle(
                rect.X + Skin.ToolWindowImageGapLeft,
                rect.Y + rect.Height - 1 - Skin.ToolWindowImageGapBottom - Skin.ToolWindowImageHeight,
                Skin.ToolWindowImageWidth, Skin.ToolWindowImageHeight);
            Rectangle rectText = rectIcon;
            rectText.X += rectIcon.Width + Skin.ToolWindowImageGapRight;
            rectText.Width = rect.Width - rectIcon.Width - Skin.ToolWindowImageGapLeft -
                Skin.ToolWindowImageGapRight - Skin.ToolWindowTextGapRight;

            Rectangle rectTab = DrawHelper.RtlTransform(this, rect);
            rectText = DrawHelper.RtlTransform(this, rectText);
            rectIcon = DrawHelper.RtlTransform(this, rectIcon);
            GraphicsPath path = GetTabOutline(content, true, false);
            if (this.ActiveContent == content)
            {
                Color startColor = Skin.ToolWindowGradient.ActiveTabGradient.StartColor;
                Color endColor = Skin.ToolWindowGradient.ActiveTabGradient.EndColor;
                LinearGradientMode gradientMode = Skin.ToolWindowGradient.ActiveTabGradient.LinearGradientMode;
                g.FillPath(new LinearGradientBrush(rectTab, startColor, endColor, gradientMode), path);
                g.DrawPath(Skin.PenToolWindowTabBorder, path);

                Color textColor = Skin.ToolWindowGradient.ActiveTabGradient.TextColor;
                TextRenderer.DrawText(g, content.Text, Skin.TextFont, rectText, textColor, ToolWindowTextFormat);
            }
            else
            {
                Color startColor = Skin.ToolWindowGradient.InactiveTabGradient.StartColor;
                Color endColor = Skin.ToolWindowGradient.InactiveTabGradient.EndColor;
                LinearGradientMode gradientMode = Skin.ToolWindowGradient.InactiveTabGradient.LinearGradientMode;
                g.FillPath(new LinearGradientBrush(rectTab, startColor, endColor, gradientMode), path);

                if (this.DisplayingContents.IndexOf(this.ActiveContent) != this.DisplayingContents.IndexOf(content) + 1)
                {
                    Point pt1 = new Point(rect.Right, rect.Top + Skin.ToolWindowTabSeperatorGapTop);
                    Point pt2 = new Point(rect.Right, rect.Bottom - Skin.ToolWindowTabSeperatorGapBottom);
                    g.DrawLine(Skin.PenToolWindowTabBorder, DrawHelper.RtlTransform(this, pt1), DrawHelper.RtlTransform(this, pt2));
                }

                Color textColor = Skin.ToolWindowGradient.InactiveTabGradient.TextColor;
                TextRenderer.DrawText(g, content.Text, Skin.TextFont, rectText, textColor, ToolWindowTextFormat);
            }

            if (rectTab.Contains(rectIcon))
                g.DrawIcon(content.Icon, rectIcon);
        }

        private void DrawTab_Document(Graphics g, DockContent content, Rectangle rect)
        {
            var tab = content.Tab;
            if (tab.TabWidth == 0)
                return;

            Rectangle rectIcon = new Rectangle(
                rect.X + Skin.DocumentIconGapLeft,
                rect.Y + rect.Height - 1 - Skin.DocumentIconGapBottom - Skin.DocumentIconHeight,
                Skin.DocumentIconWidth, Skin.DocumentIconHeight);
            Rectangle rectText = rectIcon;

            if (this.ShowDocumentIcon)
            {
                rectText.X += rectIcon.Width + Skin.DocumentIconGapRight;
                rectText.Y = rect.Y;
                rectText.Width = rect.Width - rectIcon.Width - Skin.DocumentIconGapLeft -
                    Skin.DocumentIconGapRight - Skin.DocumentTextGapRight;
                rectText.Height = rect.Height;
            }
            else
                rectText.Width = rect.Width - Skin.DocumentIconGapLeft - Skin.DocumentTextGapRight;

            Rectangle rectTab = DrawHelper.RtlTransform(this, rect);
            Rectangle rectBack = DrawHelper.RtlTransform(this, rect);
            rectBack.Width += rect.X;
            rectBack.X = 0;

            rectText = DrawHelper.RtlTransform(this, rectText);
            rectIcon = DrawHelper.RtlTransform(this, rectIcon);
            GraphicsPath path = GetTabOutline(content, true, false);
            if (this.ActiveContent == content)
            {
                Color startColor = Skin.DocumentGradient.ActiveTabGradient.StartColor;
                Color endColor = Skin.DocumentGradient.ActiveTabGradient.EndColor;
                LinearGradientMode gradientMode = Skin.DocumentGradient.ActiveTabGradient.LinearGradientMode;
                g.FillPath(new LinearGradientBrush(rectBack, startColor, endColor, gradientMode), path);
                g.DrawPath(Skin.PenDocumentTabActiveBorder, path);

                Color textColor = Skin.DocumentGradient.ActiveTabGradient.TextColor;
                if (this.IsActivated)
                    TextRenderer.DrawText(g, content.Text, Skin.BoldFont, rectText, textColor, DocumentTextFormat);
                else
                    TextRenderer.DrawText(g, content.Text, Skin.TextFont, rectText, textColor, DocumentTextFormat);
            }
            else
            {
                Color startColor = Skin.DocumentGradient.InactiveTabGradient.StartColor;
                Color endColor = Skin.DocumentGradient.InactiveTabGradient.EndColor;
                LinearGradientMode gradientMode = Skin.DocumentGradient.InactiveTabGradient.LinearGradientMode;
                g.FillPath(new LinearGradientBrush(rectBack, startColor, endColor, gradientMode), path);
                g.DrawPath(Skin.PenDocumentTabInactiveBorder, path);

                Color textColor = Skin.DocumentGradient.InactiveTabGradient.TextColor;
                TextRenderer.DrawText(g, content.Text, Skin.TextFont, rectText, textColor, DocumentTextFormat);
            }

            if (content.Icon != null && rectTab.Contains(rectIcon) && 
                this.ShowDocumentIcon)
                g.DrawIcon(content.Icon, rectIcon);
        }

        private Rectangle TabStripRectangle
        {
            get
            {
                if (Appearance == AppearanceStyle.Document)
                    return TabStripRectangle_Document;
                else
                    return TabStripRectangle_ToolWindow;
            }
        }

        private Rectangle TabStripRectangle_ToolWindow
        {
            get
            {
                Rectangle rect = ClientRectangle;
                return new Rectangle(rect.X, rect.Top + Skin.ToolWindowStripGapTop, rect.Width, rect.Height - Skin.ToolWindowStripGapTop - Skin.ToolWindowStripGapBottom);
            }
        }

        private Rectangle TabStripRectangle_Document
        {
            get
            {
                Rectangle rect = ClientRectangle;
                return new Rectangle(rect.X, rect.Top + Skin.DocumentStripGapTop, rect.Width, rect.Height - Skin.DocumentStripGapTop - Skin.ToolWindowStripGapBottom);
            }
        }

        private Rectangle TabsRectangle
        {
            get
            {
                if (Appearance == AppearanceStyle.ToolWindow)
                    return TabStripRectangle;

                Rectangle rectWindow = TabStripRectangle;
                int x = rectWindow.X;
                int y = rectWindow.Y;
                int width = rectWindow.Width;
                int height = rectWindow.Height;

                x += Skin.DocumentTabGapLeft;
                width -= Skin.DocumentTabGapLeft +
                        Skin.DocumentTabGapRight +
                        Skin.DocumentButtonGapRight +
                        ButtonClose.Width +
                        ButtonWindowList.Width +
                        2 * Skin.DocumentButtonGapBetween;

                return new Rectangle(x, y, width, height);
            }
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

        private InertButton m_buttonWindowList;
        private InertButton ButtonWindowList
        {
            get
            {
                if (m_buttonWindowList == null)
                {
                    m_buttonWindowList = new InertButton(Skin.ImageButtonWindowList, Skin.ImageButtonWindowListOverflow);
                    ToolTip.SetToolTip(m_buttonWindowList, Skin.ToolTipSelect);
                    m_buttonWindowList.Click += new EventHandler(WindowList_Click);
                    Controls.Add(m_buttonWindowList);
                }

                return m_buttonWindowList;
            }
        }

        private void WindowList_Click(object sender, EventArgs e)
        {
            int x = 0;
            int y = ButtonWindowList.Location.Y + ButtonWindowList.Height;

            SelectMenu.Items.Clear();
            foreach (DockContent content in this.DisplayingContents)
            {
                ToolStripItem item = SelectMenu.Items.Add(content.Text, content.Icon.ToBitmap());
                item.Tag = content;
                item.Click += new EventHandler(ContextMenuItem_Click);
            }
            SelectMenu.Show(ButtonWindowList, x, y);
        }

        private void ContextMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            if (item != null)
            {
                DockContent content = (DockContent)item.Tag;
                this.ActiveContent = content;
            }
        }

        private void SetInertButtons()
        {
            if (Appearance == AppearanceStyle.ToolWindow)
            {
                if (m_buttonClose != null)
                    m_buttonClose.Left = -m_buttonClose.Width;

                if (m_buttonWindowList != null)
                    m_buttonWindowList.Left = -m_buttonWindowList.Width;
            }
            else
            {
                ButtonClose.Enabled = this.ActiveContent == null ? true : this.ActiveContent.CloseButtonEnable;
                ButtonClose.Visible = this.ActiveContent == null ? true : this.ActiveContent.CloseButtonVisible;
                ButtonClose.RefreshChanges();
                ButtonWindowList.RefreshChanges();
            }
        }

        //private void Close_Click(object sender, EventArgs e)
        //{
        //    OnCloseButtonClick(sender, e);
        //}

        #endregion

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

        private bool HasTabPageContextMenu
        {
            get { return TabPageContextMenu != null; }
        }

        private void ShowTabPageContextMenu(Control control, Point position)
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

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (e.Button == MouseButtons.Right)
                ShowTabPageContextMenu(this, new Point(e.X, e.Y));
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            int index = HitTest();
            if (index != -1)
            {
                this.ActiveContent = this.DisplayingContents[index];
            }
            if (e.Button == MouseButtons.Left)
            {
                OnDockContentClicked(new DockContentEventArgs(this.ActiveContent));
            }
        }
        protected override void OnDragOver(DragEventArgs drgevent)
        {
            base.OnDragOver(drgevent);

            int index = HitTest();
            if (index != -1)
            {
                this.ActiveContent = this.DisplayingContents[index];
            }
        }
        protected override void OnMouseHover(EventArgs e)
        {
            int index = HitTest(PointToClient(Control.MousePosition));
            string toolTip = string.Empty;

            base.OnMouseHover(e);

            if (index != -1)
            {
                DockContent content = this.DisplayingContents[index];
                var tab = content.Tab;

                if (!String.IsNullOrEmpty(content.ToolTipText))
                    toolTip = content.ToolTipText;
                else if (tab.MaxWidth > tab.TabWidth)
                    toolTip = content.Text;
            }

            if (ToolTip.GetToolTip(this) != toolTip)
            {
                ToolTip.Active = false;
                ToolTip.SetToolTip(this, toolTip);
                ToolTip.Active = true;
            }

            // requires further tracking of mouse hover behavior,
            ResetMouseEventArgs();
        }
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            this.Height = this.MeasureHeight();
        }
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == (int)Win32.Msgs.WM_LBUTTONDBLCLK)
            {
                base.WndProc(ref m);

                int index = HitTest();
                if (
                    //this.AllowEndUserDocking && 
                    index != -1)
                {
                    //DockContent content = this.DisplayingContents[index];
                    //if (content.CheckDockState(!content.IsFloat) != DockState.Unknown)
                    //    content.IsFloat = !content.IsFloat;
                }

                return;
            }

            base.WndProc(ref m);
            return;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (this.Bounds.IsEmpty)
            {
                return;
            }

            Rectangle rect = TabsRectangle;

            if (Appearance == AppearanceStyle.Document)
            {
                rect.X -= Skin.DocumentTabGapLeft;

                // Add these values back in so that the DockStrip color is drawn
                // beneath the close button and window list button.
                rect.Width += Skin.DocumentTabGapLeft +
                    Skin.DocumentTabGapRight +
                    Skin.DocumentButtonGapRight +
                    ButtonClose.Width +
                    ButtonWindowList.Width;

                // It is possible depending on the DockPanel DocumentStyle to have
                // a Document without a DockStrip.
                if (rect.Width > 0 && rect.Height > 0)
                {
                    Color startColor = Skin.DocumentGradient.DockStripGradient.StartColor;
                    Color endColor = Skin.DocumentGradient.DockStripGradient.EndColor;
                    LinearGradientMode gradientMode = Skin.DocumentGradient.DockStripGradient.LinearGradientMode;
                    using (LinearGradientBrush brush = new LinearGradientBrush(rect, startColor, endColor, gradientMode))
                    {
                        e.Graphics.FillRectangle(brush, rect);
                    }
                }
            }
            else
            {
                Color startColor = Skin.ToolWindowGradient.DockStripGradient.StartColor;
                Color endColor = Skin.ToolWindowGradient.DockStripGradient.EndColor;
                LinearGradientMode gradientMode = Skin.ToolWindowGradient.DockStripGradient.LinearGradientMode;
                using (LinearGradientBrush brush = new LinearGradientBrush(rect, startColor, endColor, gradientMode))
                {
                    e.Graphics.FillRectangle(brush, rect);
                }
            }

            CalculateTabs();

            if (Appearance == AppearanceStyle.Document && this.ActiveContent != null)
            {
                if (EnsureDocumentTabVisible(this.ActiveContent, false))
                    CalculateTabs();
            }

            DrawTabStrip(e.Graphics);
        }
        protected override void OnLayout(LayoutEventArgs levent)
        {
            if (Appearance != AppearanceStyle.Document)
            {
                base.OnLayout(levent);
                return;
            }

            Rectangle rectTabStrip = TabStripRectangle;

            // Set position and size of the buttons
            int buttonWidth = ButtonClose.Image.Width;
            int buttonHeight = ButtonClose.Image.Height;
            int height = rectTabStrip.Height - Skin.DocumentButtonGapTop - Skin.DocumentButtonGapBottom;
            if (buttonHeight < height)
            {
                buttonWidth = buttonWidth * (height / buttonHeight);
                buttonHeight = height;
            }
            Size buttonSize = new Size(buttonWidth, buttonHeight);

            int x = rectTabStrip.X + rectTabStrip.Width - Skin.DocumentTabGapLeft
                - Skin.DocumentButtonGapRight - buttonWidth;
            int y = rectTabStrip.Y + Skin.DocumentButtonGapTop;
            Point point = new Point(x, y);
            ButtonClose.Bounds = DrawHelper.RtlTransform(this, new Rectangle(point, buttonSize));

            // If the close button is not visible draw the window list button overtop.
            // Otherwise it is drawn to the left of the close button.
            if (ButtonClose.Visible)
                point.Offset(-(Skin.DocumentButtonGapBetween + buttonWidth), 0);

            ButtonWindowList.Bounds = DrawHelper.RtlTransform(this, new Rectangle(point, buttonSize));

            OnRefreshChanges();

            base.OnLayout(levent);
        }

        private void _dockContentCollection_DockContentRemoved(object sender, DockContentEventArgs e)
        {
            this.Height = Math.Max(this.MeasureHeight(), this.Height);
            //如果当前的现实的Content
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
        private void _dockContentCollection_DockContentAdded(object sender, DockContentEventArgs e)
        {
            this.ActiveContent = e.Content;
            this.Height = Math.Max(this.MeasureHeight(), this.Height);
            this.RefreshChanges();
        }
        private void this_ActiveContentChanged(object sender, DockContentChangedEventArgs e)
        {
            DockContent oldValue = e.OldContent;
            DockContent newValue = e.OldContent;

            if (newValue != null)
                newValue.Visible = true;

            if (oldValue != null && DisplayingContents.Contains(oldValue))
                oldValue.Visible = false;

            if (newValue != null)
                this.EnsureTabVisible(newValue);

            RefreshChanges();
        }

        public VS2005DockPageStrip()
        {
            InitializeComponent();

            m_toolTip = new ToolTip(this.components);
            m_selectMenu = new ContextMenuStrip(this.components);

            m_displayingContents = new VisibleContentCollection(this.DockContents);

            this.ActiveContentChanged += new DockContentChangedEventHandler(this_ActiveContentChanged);
            DockContents.DockContentRemoved += new DockContentEventHandler(_dockContentCollection_DockContentRemoved);
            DockContents.DockContentAdded += new DockContentEventHandler(_dockContentCollection_DockContentAdded);

            DockContent dummyContent1 = new DockContent();
            dummyContent1.Text = "Content1";
            this.AddDockContent(dummyContent1);

            DockContent dummyContent2 = new DockContent();
            dummyContent2.Text = "Content2";
            this.AddDockContent(dummyContent2);

            this.Height = this.MeasureHeight();
            this.Appearance = AppearanceStyle.Document;
            this.Appearance = AppearanceStyle.ToolWindow;

        }

        #region ToolTip & ContextMenuStrip

        private ToolTip m_toolTip;
        protected ToolTip ToolTip
        {
            get { return m_toolTip; }
        }

        private ContextMenuStrip m_selectMenu;
        protected ContextMenuStrip SelectMenu
        {
            get { return m_selectMenu; }
        }

        #endregion

        #region IDockPageStripControl

        public override Control CloseButton
        {
            get { return this.ButtonClose; }
        }

        public override GraphicsPath GetOutline(int index)
        {

            if (Appearance == AppearanceStyle.Document)
                return GetOutline_Document(index);
            else
                return GetOutline_ToolWindow(index);

        }

        public override int HitTest(Point ptMouse)
        {
            Rectangle rectTabStrip = TabsRectangle;
            if (!TabsRectangle.Contains(ptMouse))
                return -1;

            foreach (DockContent content in this.DisplayingContents)
            {
                GraphicsPath path = GetTabOutline(content, true, false);
                if (path.IsVisible(ptMouse))
                    return this.DisplayingContents.IndexOf(content);
            }
            return -1;
        }

        public override void RefreshChanges()
        {
            if (IsDisposed)
                return;
            this.Width += 1;
            this.Width -= 1;
            OnRefreshChanges();
        }

        #endregion
    }
}
