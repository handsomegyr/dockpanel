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
    public partial class DockPanel : Panel, IDockDragProcessor, ISplitterDragProcessor
    {
        #region Propery

        public DockContent[] DocumentsToArray()
        {
            int count = DocumentsCount;
            DockContent[] documents = new DockContent[count];
            int i = 0;
            foreach (DockContent content in Documents)
            {
                documents[i] = content;
                i++;
            }

            return documents;
        }

        public IEnumerable<DockContent> Documents
        {
            get
            {
                foreach (DockContent content in DockContents)
                {
                    if (content.DockState == DockState.Document)
                        yield return content;
                }
            }
        }
        
        public int DocumentsCount
        {
            get
            {
                int count = 0;
                foreach (DockContent content in Documents)
                    count++;

                return count;
            }
        }
        
        private DockPage m_ActivePage = null;
        [Browsable(false)]
        public DockPage ActivePage
        {
            get { return m_ActivePage; }
            set
            {
                if (m_ActivePage == value || value == null || value.DockState == DockState.Document)
                {
                    return;
                }
                if (m_ActivePage != null)
                    m_ActivePage.IsActivated = false;
                m_ActivePage = value;
                if (m_ActivePage != null)
                    m_ActivePage.IsActivated = true;
            }
        }

        private DockContent m_ActiveDocument = null;
        [Browsable(false)]
        public DockContent ActiveDocument
        {
            get { return m_ActiveDocument; }
            set
            {
                if (m_ActiveDocument == value || value == null || value.DockState != DockState.Document)
                {
                    return;
                }
                m_ActiveDocument = value;
            }
        }

        private DockPage m_ActiveDocumentPage = null;
        [Browsable(false)]
        public DockPage ActiveDocumentPage
        {
            get { return m_ActiveDocumentPage; }
            set
            {
                if (m_ActiveDocumentPage == value || value == null || value.DockState != DockState.Document)
                {
                    return;
                }
                if (m_ActiveDocumentPage != null)
                    m_ActiveDocumentPage.IsActivated = false;
                m_ActiveDocumentPage = value;
                if (m_ActiveDocumentPage != null)
                    m_ActiveDocumentPage.IsActivated = true;
            }
        }
        #endregion

        #region Collections

        private DockPageCollection m_pages = new DockPageCollection();
        [Browsable(false)]
        internal DockPageCollection DockPages
        {
            get { return m_pages; }
        }

        private DockContentCollection m_contents = new DockContentCollection();
        [Browsable(false)]
        internal DockContentCollection DockContents
        {
            get { return m_contents; }
        }

        private FloatWindowCollection m_floatWindows;
        [Browsable(false)]
        internal FloatWindowCollection FloatWindows
        {
            get { return m_floatWindows; }
        }

        private DockWindowCollection m_dockWindows;
        [Browsable(false)]
        internal DockWindowCollection DockWindows
        {
            get
            {
                return m_dockWindows;
            }
        }

        #endregion

        #region ISplitterDragProcessor

        private SplitterDragHandler m_splitterDragHandler = null;
        private SplitterDragHandler GetSplitterDragHandler()
        {
            if (m_splitterDragHandler == null)
                m_splitterDragHandler = new SplitterDragHandler(this);
            return m_splitterDragHandler;
        }

        public void SplitterMove(IDragSource DragSource, Rectangle rectSplitter)
        {
            GetSplitterDragHandler().BeginDrag(DragSource, rectSplitter);
        }

        public void BeginDrag(IDragSource DragSource, Rectangle rectSplitter)
        {
            #region DockPageSplitter
            if (DragSource is DockPageSplitter) //DockPageSplitter
            {
                DockPageSplitter splitter = DragSource as DockPageSplitter;
                DockPage DockPane = splitter.DockPane;
            }
            #endregion
            #region SplitterBase
            else if (DragSource is SplitterBase) //SplitterBase
            {
                SplitterBase splitter = DragSource as SplitterBase;
                if (splitter.Parent is DockWindow) //DockWindow
                {
                    DockWindow window = splitter.Parent as DockWindow;
                }
                else if (splitter.Parent is AutoHideWindow)//AutoHideWindow
                {
                    AutoHideWindow window = splitter.Parent as AutoHideWindow;
                    window.FlagDragging = true;
                }
            }
            #endregion
        }

        public void EndDrag(IDragSource DragSource)
        {
            #region DockPageSplitter
            if (DragSource is DockPageSplitter) //DockPageSplitter
            {
                DockPageSplitter splitter = DragSource as DockPageSplitter;
                DockPage DockPane = splitter.DockPane;
            }
            #endregion
            #region SplitterBase
            else if (DragSource is SplitterBase) //SplitterBase
            {
                SplitterBase splitter = DragSource as SplitterBase;
                if (splitter.Parent is DockWindow) //DockWindow
                {
                    DockWindow window = splitter.Parent as DockWindow;
                }
                else if (splitter.Parent is AutoHideWindow)//AutoHideWindow
                {
                    AutoHideWindow window = splitter.Parent as AutoHideWindow;
                    window.FlagDragging = false;
                }
            }
            #endregion
        }

        public bool IsVertical(IDragSource DragSource)
        {
            #region DockPageSplitter
            if (DragSource is DockPageSplitter) //DockPageSplitter
            {
                DockPageSplitter splitter = DragSource as DockPageSplitter;
                DockPage DockPane = splitter.DockPane;
                NestedDockingStatus status = DockPane.NestedDockingStatus;
                return (status.DisplayingAlignment == DockAlignment.Left ||
                    status.DisplayingAlignment == DockAlignment.Right);
            }
            #endregion
            #region DockWindow
            else if (DragSource is DockWindow) //DockWindow
            {
                DockWindow window = DragSource as DockWindow;
                return (window.DockState == DockState.DockLeft || window.DockState == DockState.DockRight);

            }
            #endregion
            #region AutoHideWindow
            else if (DragSource is AutoHideWindow)//AutoHideWindow
            {
                AutoHideWindow window = DragSource as AutoHideWindow;
                return (window.DockState == DockState.DockLeftAutoHide || window.DockState == DockState.DockRightAutoHide);
            }
            #endregion
            return false;
        }

        public Rectangle DragLimitBounds(IDragSource DragSource)
        {
            #region DockPageSplitter
            if (DragSource is DockPageSplitter) //DockPageSplitter
            {
                DockPageSplitter splitter = DragSource as DockPageSplitter;
                DockPage DockPane = splitter.DockPane;

                NestedDockingStatus status = DockPane.NestedDockingStatus;
                Rectangle rectLimit = splitter.Parent.RectangleToScreen(status.LogicalBounds);
                if (this.IsVertical(DragSource))
                {
                    rectLimit.X += MeasurePane.MinSize;
                    rectLimit.Width -= 2 * MeasurePane.MinSize;
                }
                else
                {
                    rectLimit.Y += MeasurePane.MinSize;
                    rectLimit.Height -= 2 * MeasurePane.MinSize;
                }

                return rectLimit;
            }
            #endregion
            #region DockWindow
            else if (DragSource is DockWindow) //DockWindow
            {
                DockWindow window = DragSource as DockWindow;
                Rectangle rectLimit = this.DockArea;
                Point location;
                if ((Control.ModifierKeys & Keys.Shift) == 0)
                    location = window.Location;
                else
                    location = this.DockArea.Location;

                if (this.IsVertical(DragSource))
                {
                    rectLimit.X += MeasurePane.MinSize;
                    rectLimit.Width -= 2 * MeasurePane.MinSize;
                    rectLimit.Y = location.Y;
                    if ((Control.ModifierKeys & Keys.Shift) == 0)
                        rectLimit.Height = window.Height;
                }
                else
                {
                    rectLimit.Y += MeasurePane.MinSize;
                    rectLimit.Height -= 2 * MeasurePane.MinSize;
                    rectLimit.X = location.X;
                    if ((Control.ModifierKeys & Keys.Shift) == 0)
                        rectLimit.Width = window.Width;
                }

                return this.RectangleToScreen(rectLimit);

            }
            #endregion
            #region AutoHideWindow
            else if (DragSource is AutoHideWindow)//AutoHideWindow
            {
                AutoHideWindow window = DragSource as AutoHideWindow;
                Rectangle rectLimit = this.DockArea;

                if (this.IsVertical(DragSource))
                {
                    rectLimit.X += MeasurePane.MinSize;
                    rectLimit.Width -= 2 * MeasurePane.MinSize;
                }
                else
                {
                    rectLimit.Y += MeasurePane.MinSize;
                    rectLimit.Height -= 2 * MeasurePane.MinSize;
                }

                return this.RectangleToScreen(rectLimit);
            }
            #endregion
            return Rectangle.Empty;
        }

        public void MoveSplitter(IDragSource DragSource, int offset)
        {
            #region DockPageSplitter
            if (DragSource is DockPageSplitter) //DockPageSplitter
            {
                DockPageSplitter splitter = DragSource as DockPageSplitter;
                DockPage DockPane = splitter.DockPane;

                NestedDockingStatus status = DockPane.NestedDockingStatus;
                double proportion = status.Proportion;
                if (status.LogicalBounds.Width <= 0 || status.LogicalBounds.Height <= 0)
                    return;
                else if (status.DisplayingAlignment == DockAlignment.Left)
                    proportion += ((double)offset) / (double)status.LogicalBounds.Width;
                else if (status.DisplayingAlignment == DockAlignment.Right)
                    proportion -= ((double)offset) / (double)status.LogicalBounds.Width;
                else if (status.DisplayingAlignment == DockAlignment.Top)
                    proportion += ((double)offset) / (double)status.LogicalBounds.Height;
                else
                    proportion -= ((double)offset) / (double)status.LogicalBounds.Height;

                //DockPane.SetNestedDockingProportion(proportion);
                status.SetStatus(
                    status.Container,
                    status.PreviousPane,
                    status.Alignment, proportion);

                if (DockPane.DockControl != null)
                    ((Control)DockPane.DockControl).PerformLayout();

            }
            #endregion
            #region DockWindow
            else if (DragSource is DockWindow) //DockWindow
            {
                DockWindow window = DragSource as DockWindow;
                if ((Control.ModifierKeys & Keys.Shift) != 0)
                    window.SendToBack();

                Rectangle rectDockArea = this.DockArea;
                if (window.DockState == DockState.DockLeft && rectDockArea.Width > 0)
                {
                    if (this.DockLeftPortion > 1)
                        this.DockLeftPortion = window.Width + offset;
                    else
                        this.DockLeftPortion += ((double)offset) / (double)rectDockArea.Width;
                }
                else if (window.DockState == DockState.DockRight && rectDockArea.Width > 0)
                {
                    if (this.DockRightPortion > 1)
                        this.DockRightPortion = window.Width - offset;
                    else
                        this.DockRightPortion -= ((double)offset) / (double)rectDockArea.Width;
                }
                else if (window.DockState == DockState.DockBottom && rectDockArea.Height > 0)
                {
                    if (this.DockBottomPortion > 1)
                        this.DockBottomPortion = window.Height - offset;
                    else
                        this.DockBottomPortion -= ((double)offset) / (double)rectDockArea.Height;
                }
                else if (window.DockState == DockState.DockTop && rectDockArea.Height > 0)
                {
                    if (this.DockTopPortion > 1)
                        this.DockTopPortion = window.Height + offset;
                    else
                        this.DockTopPortion += ((double)offset) / (double)rectDockArea.Height;
                }

            }
            #endregion
            #region AutoHideWindow
            else if (DragSource is AutoHideWindow)//AutoHideWindow
            {
                AutoHideWindow window = DragSource as AutoHideWindow;
                Rectangle rectDockArea = this.DockArea;
                DockContent content = window.ActiveContent;
                if (window.DockState == DockState.DockLeftAutoHide && rectDockArea.Width > 0)
                {
                    if (content.AutoHidePortion < 1)
                        content.AutoHidePortion += ((double)offset) / (double)rectDockArea.Width;
                    else
                        content.AutoHidePortion = Width + offset;
                }
                else if (window.DockState == DockState.DockRightAutoHide && rectDockArea.Width > 0)
                {
                    if (content.AutoHidePortion < 1)
                        content.AutoHidePortion -= ((double)offset) / (double)rectDockArea.Width;
                    else
                        content.AutoHidePortion = Width - offset;
                }
                else if (window.DockState == DockState.DockBottomAutoHide && rectDockArea.Height > 0)
                {
                    if (content.AutoHidePortion < 1)
                        content.AutoHidePortion -= ((double)offset) / (double)rectDockArea.Height;
                    else
                        content.AutoHidePortion = Height - offset;
                }
                else if (window.DockState == DockState.DockTopAutoHide && rectDockArea.Height > 0)
                {
                    if (content.AutoHidePortion < 1)
                        content.AutoHidePortion += ((double)offset) / (double)rectDockArea.Height;
                    else
                        content.AutoHidePortion = Height + offset;
                }
            }
            #endregion
        }

        #endregion

        #region IDockDragProcessor

        private bool IsDockStateValid(DockContent content, DockState dockState)
        {
            if (dockState == DockState.Document &&
                    this.DocumentStyle == DocumentStyle.SystemMdi)
                return false;
            else
                return DockHelper.IsDockStateValid(dockState, content.DockAreas);
        }

        public bool IsDockStateValid(IDragSource DragSource, DockState dockState)
        {
            #region DockContent
            if (DragSource is DockContent) //DockContent
            {
                DockContent content = DragSource as DockContent;
                return IsDockStateValid(content, dockState);
                
            }
            #endregion
            #region DockPage
            else if (DragSource is DockPage) //DockPage
            {
                DockPage page = DragSource as DockPage;
                foreach (DockContent content in page.DockContents)
                    if (!IsDockStateValid(content,dockState))
                        return false;

                return true;
            }
            #endregion
            #region DockWindow
            else if (DragSource is DockWindow) //DockWindow
            {
                DockWindow window = DragSource as DockWindow;
                return true;
            }
            #endregion
            #region FloatWindow
            else if (DragSource is FloatWindow) // FloatWindow
            {
                FloatWindow window = DragSource as FloatWindow;
                foreach (DockPage pane in window.NestedPanes)
                    foreach (DockContent content in pane.DockContents)
                        if (!DockHelper.IsDockStateValid(dockState, content.DockAreas))
                            return false;

                return true;
            }
            #endregion
            #region Other
            else
            {
                return false;
            }
            #endregion
        }

        public bool CanDockTo(IDragSource DragSource, DockPage pane)
        {
            #region DockContent
            if (DragSource is DockContent) //DockContent
            {
                DockContent content = DragSource as DockContent;

                if (!IsDockStateValid(DragSource, pane.DockState))
                    return false;

                if (content.DockPage == pane && pane.DisplayingContents.Count == 1)
                    return false;

                return true;
            }
            #endregion
            #region DockPage
            else if (DragSource is DockPage) //DockPage
            {
                DockPage page = DragSource as DockPage;
                if (!IsDockStateValid(DragSource,pane.DockState))
                    return false;

                if (pane == page)
                    return false;

                return true;
            }
            #endregion
            #region FloatWindow
            else if (DragSource is FloatWindow) // FloatWindow
            {
                FloatWindow window = DragSource as FloatWindow;

                if (!IsDockStateValid(DragSource,pane.DockState))
                    return false;

                if (pane.DockControl == window)
                    return false;

                return true;
            }
            #endregion
            return false;
        }

        public void TestDrop(IDragSource TestDropSource, IDragSource dragSource, DockOutlineBase dockOutline)
        {
            #region DockPage
            if (TestDropSource is DockPage) //DockPage
            {
                DockPage page = TestDropSource as DockPage;
                if (!this.CanDockTo(dragSource, page))
                    return;

                Point ptMouse = Control.MousePosition;

                HitTestResult hitTestResult = page.GetHitTest(ptMouse);
                if (hitTestResult.HitArea == HitTestArea.Caption)
                    dockOutline.Show(page, -1);
                else if (hitTestResult.HitArea == HitTestArea.TabStrip && hitTestResult.Index != -1)
                    dockOutline.Show(page, hitTestResult.Index);
                
            }
            #endregion
            #region FloatWindow
            else if (TestDropSource is FloatWindow) // FloatWindow
            {
                FloatWindow window = TestDropSource as FloatWindow;
                if (window.VisibleNestedPanes.Count == 1)
                {
                    DockPage pane = window.VisibleNestedPanes[0];
                    if (!this.CanDockTo(dragSource,pane))
                        return;

                    Point ptMouse = Control.MousePosition;
                    uint lParam = Win32Helper.MakeLong(ptMouse.X, ptMouse.Y);
                    if (NativeMethods.SendMessage(Handle, (int)Win32.Msgs.WM_NCHITTEST, 0, lParam) == (uint)Win32.HitTest.HTCAPTION)
                        dockOutline.Show(window.VisibleNestedPanes[0], -1);
                }
                
            }
            #endregion          
        }

        public Rectangle BeginDrag(IDragSource DragSource, Point ptMouse)
        {
            #region DockContent
            if (DragSource is DockContent) //DockContent
            {
                DockContent content = DragSource as DockContent;

                Size size;
                DockPage floatPane = null;
                FloatWindow fw = this.FloatWindows.CreateOrGetFloatWindow();
                if (content.DockState == DockState.Float || floatPane == null || fw.NestedPanes.Count != 1)
                    size = this.DefaultFloatWindowSize;
                else
                    size = fw.Size;

                Point location;
                DockPage Pane = content.DockPage;
                Rectangle rectPane = Pane.ClientRectangle;
                if (content.DockState == DockState.Document)
                {
                    if (this.DocumentTabStripLocation == DocumentTabStripLocation.Bottom)
                        location = new Point(rectPane.Left, rectPane.Bottom - size.Height);
                    else
                        location = new Point(rectPane.Left, rectPane.Top);
                }
                else
                {
                    location = new Point(rectPane.Left, rectPane.Bottom);
                    location.Y -= size.Height;
                }
                location = Pane.PointToScreen(location);

                if (ptMouse.X > location.X + size.Width)
                    location.X += ptMouse.X - (location.X + size.Width) + Measures.SplitterSize;

                return new Rectangle(location, size);

            }
            #endregion
            #region DockPage
            else if (DragSource is DockPage) //DockPage
            {
                DockPage page = DragSource as DockPage;
                Point location = PointToScreen(new Point(0, 0));
                Size size;

                DockPage floatPane = null;
                FloatWindow fw = this.FloatWindows.CreateOrGetFloatWindow();
                if (page.DockState == DockState.Float || floatPane == null || fw.NestedPanes.Count != 1)
                    size = this.DefaultFloatWindowSize;
                else
                    size = fw.Size;

                if (ptMouse.X > location.X + size.Width)
                    location.X += ptMouse.X - (location.X + size.Width) + Measures.SplitterSize;

                return new Rectangle(location, size);
            }
            #endregion
            #region FloatWindow
            else if (DragSource is FloatWindow) // FloatWindow
            {
                FloatWindow window = DragSource as FloatWindow;
                return window.Bounds;
            }
            #endregion
            return Rectangle.Empty;
        }

        public void FloatAt(IDragSource DragSource, Rectangle floatWindowBounds)
        {
            #region DockContent FloatAt
            if (DragSource is DockContent)
            {
                DockContent dragContent = DragSource as DockContent;
                DockPage page = processShow(dragContent, DockState.Float, null, DockAlignment.Unknown, 0.5, -1, floatWindowBounds, true);

            }
            #endregion
            #region DockPage FloatAt
            else if (DragSource is DockPage)
            {
                DockPage dragPage = DragSource as DockPage;
                DockPage page = processShow(dragPage, DockState.Float, null, DockAlignment.Unknown, 0.5, -1, floatWindowBounds, true);
            }
            #endregion
            #region FloatWindow FloatAt
            else if (DragSource is FloatWindow)
            {
                FloatWindow dragFloatWindow = DragSource as FloatWindow;
                dragFloatWindow.Bounds = floatWindowBounds;
            }
            #endregion
        }

        private void doDock(DockContent dragContent, DockState stateTo, DockPage pane, DockStyle dockStyle, int contentIndex)
        {
            DockPage dragPage = dragContent.DockPage;
            //dragPage's DockContents  more than 1 
            if (dragPage.DockContents.Count < 1)
            {
                throw new Exception("Error in DockContents count");
            }
            DockPage page = processShow(dragContent, stateTo, pane, DockHelper.GetDockAlignment(dockStyle), 0.5, contentIndex, Rectangle.Empty, true);

        }

        private void doDock(DockPage dragPage, DockState stateTo, DockPage pane, DockStyle dockStyle, int contentIndex)
        {
            //dragPage's DockContents less than 1 
            if (dragPage.DisplayingContents.Count < 1)
            {
                throw new Exception("Error in DockContents count");
            }
            DockPage page = processShow(dragPage, stateTo, pane, DockHelper.GetDockAlignment(dockStyle), 0.5, contentIndex, Rectangle.Empty, true);

        }

        public void DockTo(IDragSource DragSource, DockPage pane, DockStyle dockStyle, int contentIndex)
        {
            #region DockContent DockTo
            if (DragSource is DockContent)
            {
                DockContent dragContent = DragSource as DockContent;

                #region DockTo(DockPage pane, DockStyle dockStyle, int contentIndex)
                
                DockState stateTo = pane.DockState;
                doDock(dragContent, stateTo, pane, dockStyle, contentIndex);

                #endregion

            }
            #endregion
            #region DockPage DockTo
            else if (DragSource is DockPage)
            {
                DockPage dragPage = DragSource as DockPage;

                #region DockTo(DockPage pane, DockStyle dockStyle, int contentIndex)
                DockState stateTo = pane.DockState;
                doDock(dragPage, stateTo, pane, dockStyle, contentIndex);
                #endregion

            }
            #endregion
            #region FloatWindow DockTo
            else if (DragSource is FloatWindow)
            {
                FloatWindow dragFloatWindow = DragSource as FloatWindow;

                #region DockTo(DockPage pane, DockStyle dockStyle, int contentIndex)
                DockState stateTo = pane.DockState;

                if (dockStyle == DockStyle.Fill)
                {
                    for (int i = 0; i < dragFloatWindow.NestedPanes.Count; i++)
                    {
                        DockPage paneFrom = dragFloatWindow.NestedPanes[i];
                        this.processShow(paneFrom, stateTo, pane, DockAlignment.Fill, 0.5, contentIndex, Rectangle.Empty, true);
                    }
                }
                else
                {
                    DockAlignment alignment = DockHelper.GetDockAlignment(dockStyle);
                    MergeNestedPanes(dragFloatWindow.VisibleNestedPanes, pane.DockControl, pane, alignment, 0.5, contentIndex);
                }
                #endregion

            }
            #endregion
        }

        public void DockTo(IDragSource DragSource, DockPanel panel, DockStyle dockStyle)
        {
            #region DockContent DockTo
            if (DragSource is DockContent)
            {
                DockContent dragContent = DragSource as DockContent;

                #region DockTo(DockPanel panel, DockStyle dockStyle)
                DockState stateTo = DockHelper.GetDockState(dockStyle);
                DockPage dragPage = dragContent.DockPage;
                DockPage pane = this.DockWindows[stateTo].NestedPanes.GetDefaultPreviousPane(dragContent.DockContext.PrevPanelPage);
                doDock(dragContent, stateTo, pane, dockStyle, -1);
                #endregion
            }
            #endregion
            #region DockPage DockTo
            else if (DragSource is DockPage)
            {
                DockPage dragPage = DragSource as DockPage;

                #region DockTo(DockPanel panel, DockStyle dockStyle)
                DockState stateFrom = dragPage.DockState;
                DockState stateTo = DockHelper.GetDockState(dockStyle);
                DockPage pane = this.DockWindows[stateTo].NestedPanes.GetDefaultPreviousPane(dragPage);
                doDock(dragPage, stateTo, pane, dockStyle, -1);
                #endregion
            }
            #endregion
            #region FloatWindow DockTo
            else if (DragSource is FloatWindow)
            {
                FloatWindow dragFloatWindow = DragSource as FloatWindow;
                
                #region DockTo(DockPanel panel, DockStyle dockStyle)
                //only one situation
                DockState state = DockHelper.GetDockState(dockStyle);
                NestedPageCollection nestedPanesTo = this.DockWindows[state].NestedPanes;

                DockPage prevPane = null;
                for (int i = nestedPanesTo.Count - 1; i >= 0; i--)
                    if (nestedPanesTo[i] != dragFloatWindow.VisibleNestedPanes[0] && nestedPanesTo[i].IsAutoHide)
                        prevPane = nestedPanesTo[i];
                MergeNestedPanes(dragFloatWindow.VisibleNestedPanes, nestedPanesTo.Container, prevPane, DockAlignment.Left, 0.5, -1);
                #endregion

            }
            #endregion
        }

        private DockDragHandler m_dockDragHandler = null;
        private DockDragHandler GetDockDragHandler()
        {
            if (m_dockDragHandler == null)
                m_dockDragHandler = new DockDragHandler(this);
            return m_dockDragHandler;
        }

        internal void BeginDrag(IDragSource dragSource)
        {
            Console.WriteLine("DockPanel:BeginDrag");
            GetDockDragHandler().BeginDrag(dragSource);
        }

        #endregion

        #region AutoHideWindow

        private AutoHideWindow m_autoHideWindow;
        private AutoHideWindow AutoHideWindow
        {
            get { return m_autoHideWindow; }
        }

        internal Control AutoHideControl
        {
            get { return m_autoHideWindow; }
        }

        internal void RefreshActiveAutoHideContent()
        {
            AutoHideWindow.RefreshActiveContent();
        }

        internal Rectangle AutoHideWindowRectangle
        {
            get
            {
                DockState state = AutoHideWindow.DockState;
                Rectangle rectDockArea = DockArea;
                if (ActiveAutoHideContent == null)
                    return Rectangle.Empty;

                if (Parent == null)
                    return Rectangle.Empty;

                Rectangle rect = Rectangle.Empty;
                double autoHideSize = ActiveAutoHideContent.AutoHidePortion;
                if (state == DockState.DockLeftAutoHide)
                {
                    if (autoHideSize < 1)
                        autoHideSize = rectDockArea.Width * autoHideSize;
                    if (autoHideSize > rectDockArea.Width - MeasurePane.MinSize)
                        autoHideSize = rectDockArea.Width - MeasurePane.MinSize;
                    rect.X = rectDockArea.X;
                    rect.Y = rectDockArea.Y;
                    rect.Width = (int)autoHideSize;
                    rect.Height = rectDockArea.Height;
                }
                else if (state == DockState.DockRightAutoHide)
                {
                    if (autoHideSize < 1)
                        autoHideSize = rectDockArea.Width * autoHideSize;
                    if (autoHideSize > rectDockArea.Width - MeasurePane.MinSize)
                        autoHideSize = rectDockArea.Width - MeasurePane.MinSize;
                    rect.X = rectDockArea.X + rectDockArea.Width - (int)autoHideSize;
                    rect.Y = rectDockArea.Y;
                    rect.Width = (int)autoHideSize;
                    rect.Height = rectDockArea.Height;
                }
                else if (state == DockState.DockTopAutoHide)
                {
                    if (autoHideSize < 1)
                        autoHideSize = rectDockArea.Height * autoHideSize;
                    if (autoHideSize > rectDockArea.Height - MeasurePane.MinSize)
                        autoHideSize = rectDockArea.Height - MeasurePane.MinSize;
                    rect.X = rectDockArea.X;
                    rect.Y = rectDockArea.Y;
                    rect.Width = rectDockArea.Width;
                    rect.Height = (int)autoHideSize;
                }
                else if (state == DockState.DockBottomAutoHide)
                {
                    if (autoHideSize < 1)
                        autoHideSize = rectDockArea.Height * autoHideSize;
                    if (autoHideSize > rectDockArea.Height - MeasurePane.MinSize)
                        autoHideSize = rectDockArea.Height - MeasurePane.MinSize;
                    rect.X = rectDockArea.X;
                    rect.Y = rectDockArea.Y + rectDockArea.Height - (int)autoHideSize;
                    rect.Width = rectDockArea.Width;
                    rect.Height = (int)autoHideSize;
                }

                return rect;
            }
        }

        internal Rectangle GetAutoHideWindowBounds(Rectangle rectAutoHideWindow)
        {
            if (DocumentStyle == DocumentStyle.SystemMdi ||
                DocumentStyle == DocumentStyle.DockingMdi)
                return (Parent == null) ? Rectangle.Empty : Parent.RectangleToClient(RectangleToScreen(rectAutoHideWindow));
            else
                return rectAutoHideWindow;
        }

        internal void RefreshAutoHideStrip()
        {
            AutoHideStripWindow.RefreshChanges();
        }

        internal Rectangle GetTabStripRectangle(DockState dockState)
        {
            return AutoHideStripWindow.GetTabStripRectangle(dockState);
        }

        [Browsable(false)]
        public DockContent ActiveAutoHideContent
        {
            get { return AutoHideWindow.ActiveContent; }
            set { AutoHideWindow.ActiveContent = value; }
        }

        #endregion

        #region AutoHideStripWindow
        private AutoHideStripBase m_autoHideStripWindow = null;
        internal AutoHideStripBase AutoHideStripWindow
        {
            get
            {
                return m_autoHideStripWindow;
            }
        }
        #endregion

        private DockPanelHandler m_DockPanelHandler = null;
        internal DockPanelHandler DockPanelHandler
        {
            get
            {
                return m_DockPanelHandler;
            }
        }

        public DockPanel()
        {
            SetStyle(
                ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer, true);

            m_DockPanelHandler = new DockPanelHandler(this);

            DockPanelHandler.DockContentRemoved += new DockContentEventHandler(DockPanelHandler_DockContentRemoved);
            DockPanelHandler.DockContentAdded += new DockContentEventHandler(DockPanelHandler_DockContentAdded);
            DockPanelHandler.DockPageRemoved += new DockPageEventHandler(DockPanelHandler_DockPageRemoved);
            DockPanelHandler.DockPageAdded += new DockPageEventHandler(DockPanelHandler_DockPageAdded);

            InitializeComponent();

            SuspendLayout();

            m_autoHideWindow = new AutoHideWindow(this);
            m_autoHideWindow.Visible = false;
            m_autoHideWindow.Splitter.SplitterMove += new EventHandler(AutoHideWindowSplitter_SplitterMove);
            SetAutoHideWindowParent();

            m_autoHideStripWindow = new AutoHideStripWindow();
            m_autoHideStripWindow.RemoveAll();
            m_autoHideStripWindow.ActiveContentChanged += new DockContentEventHandler(m_autoHideStripWindow_ActiveContentChanged);
            Controls.Add(m_autoHideStripWindow);

            this.m_floatWindows = new FloatWindowCollection();
            this.m_floatWindows.FloatWindowRemoved += new FloatWindowEventHandler(m_floatWindows_FloatWindowRemoved);
            this.m_floatWindows.FloatWindowAdded += new FloatWindowEventHandler(m_floatWindows_FloatWindowAdded);
            
            this.m_dockWindows = new DockWindowCollection();
            this.m_dockWindows.DockWindowRemoved += new DockWindowEventHandler(m_dockWindows_DockWindowRemoved);
            this.m_dockWindows.DockWindowAdded += new DockWindowEventHandler(m_dockWindows_DockWindowAdded);
            
            this.Controls.AddRange(new Control[]{
				                    DockWindows[DockState.Document],
				                    DockWindows[DockState.DockLeft],
				                    DockWindows[DockState.DockRight],
				                    DockWindows[DockState.DockTop],
				                    DockWindows[DockState.DockBottom]
				                    });

            m_dummyControl = new DummyControl();
            m_dummyControl.Bounds = new Rectangle(0, 0, 1, 1);
            Controls.Add(m_dummyControl);

            ResumeLayout();
            
          }

        #region AutoHideStripWindow Events
        void m_autoHideStripWindow_ActiveContentChanged(object sender, DockContentEventArgs e)
        {
            DockContent content = e.Content;
            if (content != null && this.ActiveAutoHideContent != content)
                this.ActiveAutoHideContent = content;
        }

        #endregion

        #region AutoHideWindow & Splitter Events

        void AutoHideWindowSplitter_SplitterMove(object sender, EventArgs e)
        {
            SplitterBase m_splitter = sender as SplitterBase;
            AutoHideWindow window = m_splitter.Parent as AutoHideWindow;
            if (window == null)
                return;

            this.SplitterMove(window, window.RectangleToScreen(m_splitter.Bounds));

        }
        #endregion

        #region DockWindow & Splitter Events

        void m_dockWindows_DockWindowRemoved(object sender, DockWindowEventArgs e)
        {
        }

        void m_dockWindows_DockWindowAdded(object sender, DockWindowEventArgs e)
        {
            DockWindow window = e.DockWindow;
            window.Splitter.SplitterMove += new EventHandler(DockWindowSplitter_SplitterMove);
        }

        void DockWindowSplitter_SplitterMove(object sender, EventArgs e)
        {
            SplitterBase m_splitter = sender as SplitterBase;
            DockWindow window = m_splitter.Parent as DockWindow;
            if (window == null)
                return;

            this.SplitterMove(window, window.RectangleToScreen(m_splitter.Bounds));

        }

        #endregion

        #region FloatWindow Events
        
        void m_floatWindows_FloatWindowRemoved(object sender, FloatWindowEventArgs e)
        {
        }

        void m_floatWindows_FloatWindowAdded(object sender, FloatWindowEventArgs e)
        {
            FloatWindow fw = e.FloatWindow;
            fw.Visible = false;
            fw.Owner = this.FindForm();
            fw.StartPosition = FormStartPosition.WindowsDefaultLocation;
            fw.Size = this.DefaultFloatWindowSize;
            fw.CaptionClick += new EventHandler(fw_CaptionClick);
            fw.CaptionDoubleClick += new EventHandler(fw_CaptionDoubleClick);
            fw.FormCloseClick += new EventHandler(fw_FormCloseClick);
        }

        void fw_FormCloseClick(object sender, EventArgs e)
        {
            FloatWindow fw = sender as FloatWindow;
            for (int i = fw.NestedPanes.Count - 1; i >= 0; i--)
            {
                DockPage page = fw.NestedPanes[i];
                DockContentCollection contents = page.DockContents;
                for (int j = contents.Count - 1; j >= 0; j--)
                {
                    DockContent content = contents[j];
                    if (content.DockState != DockState.Float)
                        continue;

                    this.DockPanelHandler.CloseContent(content);
                }
            }
        }

        void fw_CaptionDoubleClick(object sender, EventArgs e)
        {
            FloatWindow fw = sender as FloatWindow;
            this.SuspendLayout(true);

            // Restore to panel
            foreach (DockPage pane in fw.NestedPanes)
            {
                if (pane.DockState != DockState.Float)
                    continue;
                DockPanelHandler.RestoreToPanel(pane);
            }
            this.DoRefresh();
            this.ResumeLayout(true, true);
            
        }

        void fw_CaptionClick(object sender, EventArgs e)
        {
            FloatWindow fw = sender as FloatWindow;
            if (this.AllowEndUserDocking && fw.AllowEndUserDocking)
            {
                this.BeginDrag(fw);
            }
        }

        #endregion
       
        #region DockContent Events

        void DockPanelHandler_DockContentRemoved(object sender, DockContentEventArgs e)
        {
            DockContent content = e.Content;
            DockContents.Remove(content);
        }
        void DockPanelHandler_DockContentAdded(object sender, DockContentEventArgs e)
        {
            DockContent content = e.Content;
            content.AutoHidePortionChanged += new DockContentEventHandler(content_AutoHidePortionChanged);
            content.CloseButtonEnableChanged += new DockContentEventHandler(content_CloseButtonEnableChanged);
            DockContents.Add(content);
        }

        void content_CloseButtonEnableChanged(object sender, DockContentEventArgs e)
        {
            DockContent content = e.Content;
            DockPage page = content.DockPage;
            if (page != null)
            {
                if (page.ActiveContent == content)
                {
                    page.RefreshChanges();
                }
            }
        }

        void content_AutoHidePortionChanged(object sender, DockContentEventArgs e)
        {
            DockContent content = e.Content;

            if (this.ActiveAutoHideContent == content)
                this.PerformLayout();
        }
        
        #endregion

        #region DockPage Events

        void DockPanelHandler_DockPageRemoved(object sender, DockPageEventArgs e)
        {
            DockPage page = e.DockPage;
            this.DockPages.Remove(page);
        }
        void DockPanelHandler_DockPageAdded(object sender, DockPageEventArgs e)
        {
            DockPage page = e.DockPage;
            this.DockPages.Add(page);
            page.ActiveContentChanged += new DockContentChangedEventHandler(page_ActiveContentChanged);
            page.DockPageCaptionControl.DockButton.Click += new EventHandler(page_DockButton_Click);
            page.DockPageCaptionControl.CloseButton.Click += new EventHandler(m_captionControl_CloseButtonClick);
            page.DockPageCaptionControl.MouseDown += new MouseEventHandler(m_captionControl_MouseDown);
            page.DockPageCaptionControl.DockContentClicked += new DockContentEventHandler(m_captionControl_DockContentClicked);
            page.DockPageCaptionControl.DockContentDoubleClicked += new DockContentEventHandler(m_captionControl_DockContentDoubleClicked);

            page.DockPageStripControl.MouseDown += new MouseEventHandler(m_tabStripControl_MouseDown);
            page.DockPageStripControl.CloseButton.Click += new EventHandler(m_tabStripControl_CloseButtonClick);
            page.DockPageStripControl.DockContentClicked += new DockContentEventHandler(m_tabStripControl_DockContentClicked);
            page.DockPageStripControl.DockContentDoubleClicked += new DockContentEventHandler(m_tabStripControl_DockContentDoubleClicked);
            page.Splitter.SplitterMove += new EventHandler(DockPageSplitter_SplitterMove);
        }

        void page_ActiveContentChanged(object sender, DockContentChangedEventArgs e)
        {
            DockContent newContent = e.NewContent;
            DockContent oldContent = e.OldContent;
            if (newContent != null)
            {
                if (newContent.DockPage != null)
                {
                    if (newContent.DockPage.DockControl != null)
                    {
                        FloatWindow fw = newContent.DockPage.DockControl as FloatWindow;
                        if (fw != null)
                        {
                            fw.SetText();
                        }
                    }
                }
            }
        }

        #region DockPage Splitter Events
        void DockPageSplitter_SplitterMove(object sender, EventArgs e)
        {
            SplitterBase m_splitter = sender as SplitterBase;
            Control page = m_splitter.Parent as Control;
            if (page == null)
                return;
            this.SplitterMove(m_splitter as IDragSource, page.RectangleToScreen(m_splitter.Bounds));
        }
        
        #endregion
        
        #region DockButton Events
        void page_DockButton_Click(object sender, EventArgs e)
        {
            DockPageCaptionBase caption = (sender as Control).Parent as DockPageCaptionBase;
            DockPage pane = caption.Parent as DockPage;
            DockState state = DockHelper.ToggleAutoHideState(pane.DockState);

            this.SuspendLayout(true);

            if (DockHelper.IsDockStateAutoHide(state))
            {
                pane.DockState = state;
                this.ActiveAutoHideContent = pane.ActiveContent;
                this.ActiveAutoHideContent = null;
            }
            else
            {
                this.DockWindows[pane.DockState].AddDockPage(pane);
            }
            this.ResumeLayout(true,true);
            this.DoRefresh();
        }

        private void DoRefresh()
        {
            this.RefreshActiveAutoHideContent();
            this.RefreshAutoHideStrip();
            this.PerformLayout();
            this.Invalidate(true);
        }

        #endregion

        #region DockPageStripControl Events

        void m_tabStripControl_MouseDown(object sender, MouseEventArgs e)
        {
            Control c = sender as Control;
            DockPage page = c.Parent as DockPage;
            ActivePage = page;
            ActiveDocumentPage = page;
        }

        void m_tabStripControl_DockContentClicked(object sender, DockContentEventArgs e)
        {
            DockContent content = e.Content;
            if (content == null) return;
            ActiveDocument = content;
            DockPage page = content.DockPage;
            if (this.AllowEndUserDocking &&
                page.AllowDockDragAndDrop &&
                content != null &&
                content.AllowEndUserDocking)
            {
                BeginDrag(content);
            }
        }

        void m_tabStripControl_DockContentDoubleClicked(object sender, DockContentEventArgs e)
        {
            DockContent content = e.Content;
            if (content == null) return;
            DockState stateTo = DockState.Unknown;

            DockPage page = null;
            if (content.IsFloat)
            {
                page = content.DockContext.PrevPanelPage;
                if (page == null)
                {
                    stateTo = content.DefaultDockState;
                }
            }
            else
            {
                page = content.DockContext.PrevFloatPage;
                stateTo = DockState.Float;
            }
            
            if (page != null)
            {
                page.AddDockContent(content);
                FloatWindow fw = page.DockControl as FloatWindow;
                fw.Visible = true;
            }
            else
            {
                this.processShow(content,stateTo,null, DockAlignment.Unknown,0.5, -1,Rectangle.Empty,true);
            }
              
        }

        void m_tabStripControl_CloseButtonClick(object sender, EventArgs e)
        {
            Control c = sender as Control;
            if (c.Parent != null)
            {
                DockPage page = c.Parent.Parent as DockPage;
                this.DockPanelHandler.CloseContent(page.ActiveContent);
                if (page.IsAutoHide)
                {
                    this.ActiveAutoHideContent = null;
                }
            }
        }

        #endregion

        #region DockPageCaptionControl Events

        void m_captionControl_MouseDown(object sender, MouseEventArgs e)
        {
            Control c = sender as Control;
            DockPage page = c.Parent as DockPage;
            ActivePage = page;
            ActiveDocumentPage = page;
        }

        void m_captionControl_DockContentClicked(object sender, DockContentEventArgs e)
        {
            DockContent content = e.Content;
            if (content == null) return;
            DockPage page = content.DockPage;
            if (this.AllowEndUserDocking &&
                page.AllowDockDragAndDrop &&
                content != null &&
                !DockHelper.IsDockStateAutoHide(content.DockState))
            {
                BeginDrag(page);
            }

        }

        void m_captionControl_DockContentDoubleClicked(object sender, DockContentEventArgs e)
        {
            DockContent content = e.Content;
            if (content == null) return;
            DockPage dragPage = content.DockPage;

            if (DockHelper.IsDockStateAutoHide(dragPage.DockState))
            {
                this.ActiveAutoHideContent = null;
                return;
            }

            this.SuspendLayout(true);

            if (dragPage.IsFloat)
            {
                DockPanelHandler.RestoreToPanel(dragPage);
            }
            else
            {
                DockPage floatPage = DockPanelHandler.FloatAt(dragPage);
                FloatWindow fw = floatPage.DockControl as FloatWindow;
                if (fw == null)
                {
                    fw = this.FloatWindows.CreateOrGetFloatWindow();
                }
                NestedDockingStatus status = floatPage.NestedDockingStatus;
                DockToFloatWindowAndShow(fw,
                    floatPage, status.PreviousPane,
                    status.Alignment, status.Proportion, -1,
                    fw.Bounds, true);
            }
            this.ResumeLayout(true, true);
            this.DoRefresh();
        }

        void m_captionControl_CloseButtonClick(object sender, EventArgs e)
        {
            Control c = sender as Control;
            if(c.Parent != null){
                DockPage page = c.Parent.Parent as DockPage;
                this.DockPanelHandler.CloseContent(page.ActiveContent);
                if (page.IsAutoHide)
                {
                    this.ActiveAutoHideContent = null;
                }
            }

        }

        #endregion
       
        #endregion

        #region Show

        public void Show(IContent content)
        {
            Show(content, DockState.Unknown);
        }
        public void Show(IContent content, DockStyle dockStyle)
        {
            Show(content, DockHelper.GetDockState(dockStyle));
        }
        public void Show(IContent content, DockState state)
        {
            Show(content, state, Rectangle.Empty, true);
        }
        public void Show(IContent content, Rectangle floatWindowBounds)
        {
            DockState state = DockState.Float;
            Show(content, state, floatWindowBounds, true);
        }

        public void Show(IContent current, IContent previous, DockAlignment alignment)
        {
            Show(current, previous, alignment, 0.5);
        }
        public void Show(IContent current, IContent previous, DockAlignment alignment, double proportion)
        {
            Show(current, previous, alignment, proportion, -1, Rectangle.Empty, true);
        }
        public void Show(IContent current, IContent previous, DockStyle dockStyle, int contentIndex)
        {
            Show(current, previous, DockHelper.GetDockAlignment(dockStyle), 0.5, contentIndex, Rectangle.Empty, true);
        }

        internal void Show(IContent content, DockState state, Rectangle floatWindowBounds, bool visible)
        {
            DockContent dc = DockPanelHandler.GetOrCreateDockContent(content);
            Show(dc, state, floatWindowBounds, visible);
        }
        internal void Show(DockContent content, DockState state, Rectangle floatWindowBounds, bool visible)
        {
            if (!DockPanelHandler.IsDockContentExisted(content.GUID))
            {
                throw new Exception("Dockcontent is not existed!");
            }

            if (content.IsHidden)
            {
                content.IsHidden = false;
            }

            if (content.DockPage == null)
            {
                if (state == DockState.Unknown)
                {
                    state = content.DefaultShowState;
                }
                processShow(content, state, null, DockAlignment.Unknown, 0.5, -1, floatWindowBounds, visible);
            }
            else
            {
                content.DockPage.RefreshChanges();
                if (content.DockPage.IsAutoHide)
                {
                    this.ActiveAutoHideContent = content;
                }
                else if(content.DockPage.IsFloat)
                {
                    FloatWindow fw = content.DockPage.DockControl as FloatWindow;
                    fw.Visible = true;
                    this.Parent.Focus();
                }

            }
        }


        #region MergeNestedPanes

        private void MergeNestedPanes(VisibleNestedPageCollection nestedPanesFrom, IDockPageContainer nestedPanesTo, DockPage prevPane, DockAlignment alignment, double proportion, int contentIndex)
        {
            if (nestedPanesFrom.Count == 0)
                return;

            int count = nestedPanesFrom.Count;
            DockPage[] panes = new DockPage[count];
            DockPage[] prevPanes = new DockPage[count];
            DockAlignment[] alignments = new DockAlignment[count];
            double[] proportions = new double[count];

            for (int i = 0; i < count; i++)
            {
                panes[i] = nestedPanesFrom[i];
                prevPanes[i] = nestedPanesFrom[i].NestedDockingStatus.PreviousPane;
                alignments[i] = nestedPanesFrom[i].NestedDockingStatus.Alignment;
                proportions[i] = nestedPanesFrom[i].NestedDockingStatus.Proportion;
            }

            DockState stateTo = nestedPanesTo.DockState;
            DockPage page0 = panes[0];
            DockPage pane = this.processShow(page0, stateTo, prevPane, alignment, proportion, contentIndex, Rectangle.Empty, true);

            for (int i = 1; i < count; i++)
            {
                for (int j = i; j < count; j++)
                {
                    if (prevPanes[j] == panes[i - 1])
                        prevPanes[j] = pane;
                }
                DockPage pagei = pagei = panes[i];
                pane = this.processShow(pagei, stateTo, prevPanes[i], alignments[i], proportions[i], contentIndex, Rectangle.Empty, true);
            }
        }
        #endregion

        private void processBeginDock(DockContent[] contents, DockPage oldDockPage, DockPage newDockPage)
        {
            //DockContents处理
            if (oldDockPage.IsFloat == newDockPage.IsFloat)
            {
                foreach (DockContent content in contents)
                {
                    oldDockPage.RemoveDockContent(content);
                }
            }
        }

        private void processMiddleDock(DockContent[] contents, DockContent activeContent, DockPage oldDockPage, DockState stateTo, DockAlignment alignment)
        {
            if (alignment == DockAlignment.Fill)
            {
                return;
            }

            //DockControl处理
            IDockPageContainer oldContainer = null;
            DockState state = oldDockPage.DockState;
            if (DockHelper.IsFloat(state))
            {
                oldContainer = oldDockPage.DockControl;
                if (oldContainer != null)
                {
                    oldContainer.RemoveDockPage(oldDockPage);
                }
            }
            else
            {
                //AutoHideWindow 和AutoHideStripWindow的处理
                //AutoHideWindow.RemoveDockPage(oldDockPage);
                AutoHideStripWindow.RemoveDockPage(oldDockPage);

                //DockWindow的处理
                oldContainer = this.DockWindows[state];
                oldContainer.RemoveDockPage(oldDockPage);
            }

            //DockState的处理
            oldDockPage.DockState = stateTo;

            //DockContents处理
            //如果停靠的状态不是Fill停靠
            if (alignment != DockAlignment.Fill)
            {
                foreach (DockContent content in contents)
                {
                    oldDockPage.AddDockContent(content);
                }
                oldDockPage.ActiveContent = activeContent;
            }

        }

        private void DockToFloatWindowAndShow(FloatWindow fw,
                                            DockPage oldDockPage,
                                            DockPage newDockPage,
                                            DockAlignment alignment,
                                            double proportion,
                                            int contentIndex,
                                            Rectangle floatWindowBounds,
                                            bool visible)
        {
            //FloatWindow的处理
            if (fw.Visible) fw.Visible = false;

            fw.AddDockPage(oldDockPage, newDockPage, alignment, proportion, contentIndex);

            if (!floatWindowBounds.IsEmpty)
            {
                if (fw.StartPosition != FormStartPosition.Manual)
                {
                    fw.StartPosition = FormStartPosition.Manual;
                }
                if (fw.Bounds != floatWindowBounds)
                {
                    fw.SetBounds(floatWindowBounds.X, floatWindowBounds.Y,
                        floatWindowBounds.Width, floatWindowBounds.Height);
                }
            }
            //fw.Show();
            //this.FloatWindows.BringWindowToFront(fw);
            fw.Visible = visible;
        }

        private void processLastDock(DockContent[] contents,
                                    DockContent activeContent,
                                    DockPage oldDockPage,
                                    DockPage newDockPage,
                                    DockAlignment alignment,
                                    double proportion,
                                    int contentIndex,
                                    Rectangle floatWindowBounds,
                                    bool visible)
        {
            DockContent oldActiveContent = null;
            if (newDockPage != null)
            {
                oldActiveContent = newDockPage.ActiveContent;
            }

            //DockContents处理
            //如果停靠的状态是Fill停靠
            if (alignment == DockAlignment.Fill)
            {
                foreach (DockContent content in contents)
                {
                    newDockPage.AddDockContent(content);
                }
                return;
            }

            //DockControl处理
            IDockPageContainer newContainer = null;
            DockState state;
            if (newDockPage != null)
            {
                state = newDockPage.DockState;
                newContainer = newDockPage.DockControl;
            }
            else
            {
                state = oldDockPage.DockState;
                if (DockHelper.IsFloat(state))
                {
                    newContainer = this.FloatWindows.CreateOrGetFloatWindow();
                }
                else
                {
                    newContainer = this.DockWindows[state];
                }
            }

            this.SuspendLayout(true);
            //Dock的处理
            if (!newContainer.IsFloat)
            {
                //AutoHideWindow 和AutoHideStripWindow的处理
                //AutoHideWindow.AddDockPage(oldDockPage);
                AutoHideStripWindow.AddDockPage(oldDockPage);
                DockWindow dw = newContainer as DockWindow;
                dw.AddDockPage(oldDockPage, newDockPage, alignment, proportion, contentIndex);
            }
            else
            {
                FloatWindow fw = newContainer as FloatWindow;
                DockToFloatWindowAndShow(fw,
                    oldDockPage, newDockPage,
                    alignment, proportion, contentIndex,
                    floatWindowBounds, visible);
            }
            oldDockPage.DockState = state;
            this.DoRefresh();
            this.ResumeLayout(true, true);
        }

        private DockPage GetNewDockPage(DockState stateTo)
        {
            DockPage newDockPage = DockPanelHandler.GetOrCreateDockPage(stateTo);

            return newDockPage;

        }

        private DockPage processShow(DockPage dragPage,
                                    DockState stateTo,
                                    DockPage prevDockPage,
                                    DockAlignment alignment,
                                    double proportion,
                                    int contentIndex,
                                    Rectangle floatWindowBounds,
                                    bool visible)
        {
            //原来的状态
            DockState stateFrom = dragPage.DockState;
            //新的状态
            if (stateTo == DockState.Unknown)
            {
                throw (new Exception("DockState cannot be unknown"));
            }

            //DockContents
            DockContent activeContent = dragPage.ActiveContent;
            DockContent[] contents = new DockContent[dragPage.DisplayingContents.Count];
            dragPage.DisplayingContents.CopyTo(contents, 0);

            //原来的停靠Page
            DockPage oldDockPage = dragPage;
            //取得中间用的停靠PagedragPage
            DockPage tempDockPage = GetNewDockPage(stateTo);

            //处理现有的停靠
            if (oldDockPage != null)
            {
                //处理从现在停靠的DockPage转移到中间的DockPage
                processBeginDock(contents, oldDockPage, tempDockPage);

            }

            //以下发生的时机是
            //如果是从DockWindow--->FloatWindow或则是FloatWindow--->DockWindow
            //处理从中间的DockPage转移到新的DockPage
            processMiddleDock(contents, activeContent, tempDockPage, stateTo, alignment);

            //处理最终的停靠
            processLastDock(contents, activeContent, tempDockPage, prevDockPage, alignment, proportion, contentIndex, floatWindowBounds, visible);

            return tempDockPage;
        }

        private DockPage processShow(DockContent content,
                                    DockState stateTo,
                                    DockPage prevDockPage,
                                    DockAlignment alignment,
                                    double proportion,
                                    int contentIndex,
                                    Rectangle floatWindowBounds,
                                    bool visible)
        {
            //原来的状态
            DockState stateFrom = content.DockState;
            //新的状态
            if (stateTo == DockState.Unknown)
            {
                throw (new Exception("DockState cannot be unknown"));
            }

            //原来的停靠Page
            DockPage oldDockPage = content.DockPage;

            DockContent activeContent = content;
            DockContent[] contents = new DockContent[1];
            contents[0] = content;

            //取得中间用的停靠Page
            DockPage tempDockPage = GetNewDockPage(stateTo);

            //处理现有的停靠
            if (oldDockPage != null)
            {
                //处理从现在停靠的DockPage转移到中间的DockPage
                processBeginDock(contents, oldDockPage, tempDockPage);

            }

            //以下发生的时机是
            //如果是从DockWindow--->FloatWindow或则是FloatWindow--->DockWindow
            //处理从中间的DockPage转移到新的DockPage
            processMiddleDock(contents, activeContent, tempDockPage, stateTo, alignment);

            ////中间停靠的Page和最终的停靠Page是相同
            ////比如说从content1所在的Page中移动到该Page上
            ////仅仅是停靠的DockAlignment发生改变
            ////那么就上面保存下来的prevPage 作为真正的最终停靠Page
            processLastDock(contents, activeContent, tempDockPage, prevDockPage, alignment, proportion, contentIndex, floatWindowBounds, visible);

            return tempDockPage;
        }

        internal void Show(IContent current, IContent previous, DockAlignment alignment, double proportion, int contentIndex, Rectangle floatWindowBounds, bool visible)
        {
            if (!DockPanelHandler.IsDockContentExisted(previous.GUID))
            {
                throw (new Exception("previous content can not be null"));
            }

            DockContent previousContent = DockPanelHandler.GetOrCreateDockContent(previous);
            DockPage previousPage = previousContent.DockPage;

            DockContent currentContent = DockPanelHandler.GetOrCreateDockContent(current);
            DockPage currentPage = processShow(currentContent, previousPage.DockState, previousPage, alignment, proportion, contentIndex, floatWindowBounds, visible);

        }

        private int m_countRefreshStateChange = 0;
        private void SuspendRefreshStateChange()
        {
            m_countRefreshStateChange++;
            this.SuspendLayout(true);
        }

        private void ResumeRefreshStateChange()
        {
            m_countRefreshStateChange--;
            System.Diagnostics.Debug.Assert(m_countRefreshStateChange >= 0);
            this.ResumeLayout(true, true);
        }

        private bool IsRefreshStateChangeSuspended
        {
            get { return m_countRefreshStateChange != 0; }
        }

        private void ResumeRefreshStateChange(IDockPageContainer oldContainer, DockState oldDockState, DockState newDockState)
        {
            ResumeRefreshStateChange();
            RefreshStateChange(oldContainer, oldDockState, newDockState);
        }

        private void RefreshStateChange(IDockPageContainer oldContainer, DockState oldDockState, DockState newDockState)
        {
            lock (this)
            {
                if (IsRefreshStateChangeSuspended)
                    return;

                SuspendRefreshStateChange();
            }

            this.SuspendLayout(true);

            if (oldContainer != null)
            {
                Control oldContainerControl = (Control)oldContainer;
                if (oldContainer.DockState == oldDockState && !oldContainerControl.IsDisposed)
                    oldContainerControl.PerformLayout();
            }
            if (DockHelper.IsDockStateAutoHide(oldDockState))
                this.RefreshActiveAutoHideContent();


            if (!DockHelper.IsFloat(newDockState) && !DockHelper.IsDockStateAutoHide(newDockState))
                this.DockWindows[newDockState].PerformLayout();

            if (DockHelper.IsDockStateAutoHide(newDockState))
                this.RefreshActiveAutoHideContent();

            if (DockHelper.IsDockStateAutoHide(oldDockState) ||
                DockHelper.IsDockStateAutoHide(newDockState))
            {
                this.RefreshAutoHideStrip();
                this.PerformLayout();
            }

            ResumeRefreshStateChange();

            this.ResumeLayout(true, true);

        }

        #endregion

        #region Skin

        private DocumentTabStripLocation m_documentTabStripLocation = DocumentTabStripLocation.Top;
        [DefaultValue(DocumentTabStripLocation.Top)]
        [Category("Category_Docking")]
        [Description("DockPanel_DocumentTabStripLocation")]
        public DocumentTabStripLocation DocumentTabStripLocation
        {
            get { return m_documentTabStripLocation; }
            set { m_documentTabStripLocation = value; }
        }

        private DockPanelSkin m_dockPanelSkin = new DockPanelSkin();
        [Category("Category_Docking")]
        [Description("DockPanel_DockPanelSkin")]
        public DockPanelSkin Skin
        {
            get { return m_dockPanelSkin; }
            set { m_dockPanelSkin = value; }
        }

        private Color m_BackColor;
        public Color DockBackColor
        {
            get
            {
                return !m_BackColor.IsEmpty ? m_BackColor : base.BackColor;
            }
            set
            {
                m_BackColor = value;
            }
        }

        private bool m_allowEndUserDocking = true;
        [Category("Category_Docking")]
        [Description("DockPanel_AllowEndUserDocking_Description")]
        [DefaultValue(true)]
        public virtual bool AllowEndUserDocking
        {
            get { return m_allowEndUserDocking; }
            set { m_allowEndUserDocking = value; }
        }

        private bool m_allowEndUserNestedDocking = true;
        [Category("Category_Docking")]
        [Description("DockPanel_AllowEndUserNestedDocking_Description")]
        [DefaultValue(true)]
        public bool AllowEndUserNestedDocking
        {
            get { return m_allowEndUserNestedDocking; }
            set { m_allowEndUserNestedDocking = value; }
        }

        private Size m_defaultFloatWindowSize = new Size(300, 300);
        [Category("Layout")]
        [Description("DockPanel_DefaultFloatWindowSize_Description")]
        public Size DefaultFloatWindowSize
        {
            get { return m_defaultFloatWindowSize; }
            set { m_defaultFloatWindowSize = value; }
        }

        private DocumentStyle m_documentStyle = DocumentStyle.DockingWindow;
        [Category("Category_Docking")]
        [Description("DockPanel_DocumentStyle_Description")]
        [DefaultValue(DocumentStyle.DockingWindow)]
        public DocumentStyle DocumentStyle
        {
            get { return m_documentStyle; }
            set
            {
                if (value == m_documentStyle)
                    return;

                if (!Enum.IsDefined(typeof(DocumentStyle), value))
                    throw new InvalidEnumArgumentException();
            }
        }

        private double m_dockBottomPortion = 0.25;
        [Category("Category_Docking")]
        [Description("DockPanel_DockBottomPortion_Description")]
        [DefaultValue(0.25)]
        public double DockBottomPortion
        {
            get { return m_dockBottomPortion; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("value");

                if (value == m_dockBottomPortion)
                    return;

                m_dockBottomPortion = value;

                if (m_dockBottomPortion < 1 && m_dockTopPortion < 1)
                {
                    if (m_dockTopPortion + m_dockBottomPortion > 1)
                        m_dockTopPortion = 1 - m_dockBottomPortion;
                }

                PerformLayout();
            }
        }

        private double m_dockLeftPortion = 0.25;
        [Category("Category_Docking")]
        [Description("DockPanel_DockLeftPortion_Description")]
        [DefaultValue(0.25)]
        public double DockLeftPortion
        {
            get { return m_dockLeftPortion; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("value");

                if (value == m_dockLeftPortion)
                    return;

                m_dockLeftPortion = value;

                if (m_dockLeftPortion < 1 && m_dockRightPortion < 1)
                {
                    if (m_dockLeftPortion + m_dockRightPortion > 1)
                        m_dockRightPortion = 1 - m_dockLeftPortion;
                }
                PerformLayout();
            }
        }

        private double m_dockRightPortion = 0.25;
        [Category("Category_Docking")]
        [Description("DockPanel_DockRightPortion_Description")]
        [DefaultValue(0.25)]
        public double DockRightPortion
        {
            get { return m_dockRightPortion; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("value");

                if (value == m_dockRightPortion)
                    return;

                m_dockRightPortion = value;

                if (m_dockLeftPortion < 1 && m_dockRightPortion < 1)
                {
                    if (m_dockLeftPortion + m_dockRightPortion > 1)
                        m_dockLeftPortion = 1 - m_dockRightPortion;
                }
                PerformLayout();
            }
        }

        private double m_dockTopPortion = 0.25;
        [Category("Category_Docking")]
        [Description("DockPanel_DockTopPortion_Description")]
        [DefaultValue(0.25)]
        public double DockTopPortion
        {
            get { return m_dockTopPortion; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("value");

                if (value == m_dockTopPortion)
                    return;

                m_dockTopPortion = value;

                if (m_dockTopPortion < 1 && m_dockBottomPortion < 1)
                {
                    if (m_dockTopPortion + m_dockBottomPortion > 1)
                        m_dockBottomPortion = 1 - m_dockTopPortion;
                }
                PerformLayout();
            }
        }

        #endregion

        #region Layout && Paint
        
        protected override void OnLayout(LayoutEventArgs levent)
        {
            SuspendLayout(true);

            AutoHideStripWindow.Bounds = ClientRectangle;

            CalculateDockPadding();

            DockWindows[DockState.DockLeft].Width = GetDockWindowSize(DockState.DockLeft);
            DockWindows[DockState.DockRight].Width = GetDockWindowSize(DockState.DockRight);
            DockWindows[DockState.DockTop].Height = GetDockWindowSize(DockState.DockTop);
            DockWindows[DockState.DockBottom].Height = GetDockWindowSize(DockState.DockBottom);

            AutoHideWindow.Bounds = GetAutoHideWindowBounds(AutoHideWindowRectangle);

            DockWindows[DockState.Document].BringToFront();
            AutoHideWindow.BringToFront();

            base.OnLayout(levent);

            if (DocumentStyle == DocumentStyle.SystemMdi && MdiClientExists)
            {
                SetMdiClientBounds(SystemMdiClientBounds);
                InvalidateWindowRegion();
            }
            else if (DocumentStyle == DocumentStyle.DockingMdi)
                InvalidateWindowRegion();

            ResumeLayout(true, true);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.CompositingMode = CompositingMode.SourceOver;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            if (DockBackColor == BackColor) return;

            Graphics g = e.Graphics;
            SolidBrush bgBrush = new SolidBrush(DockBackColor);
            g.FillRectangle(bgBrush, ClientRectangle);
        }
        private Rectangle DockArea
        {
            get
            {
                return new Rectangle(DockPadding.Left, DockPadding.Top,
                    ClientRectangle.Width - DockPadding.Left - DockPadding.Right,
                    ClientRectangle.Height - DockPadding.Top - DockPadding.Bottom);
            }
        }

        private int GetDockWindowSize(DockState dockState)
        {
            if (dockState == DockState.DockLeft || dockState == DockState.DockRight)
            {
                int width = ClientRectangle.Width - DockPadding.Left - DockPadding.Right;
                int dockLeftSize = m_dockLeftPortion >= 1 ? (int)m_dockLeftPortion : (int)(width * m_dockLeftPortion);
                int dockRightSize = m_dockRightPortion >= 1 ? (int)m_dockRightPortion : (int)(width * m_dockRightPortion);

                if (dockLeftSize < MeasurePane.MinSize)
                    dockLeftSize = MeasurePane.MinSize;
                if (dockRightSize < MeasurePane.MinSize)
                    dockRightSize = MeasurePane.MinSize;

                if (dockLeftSize + dockRightSize > width - MeasurePane.MinSize)
                {
                    int adjust = (dockLeftSize + dockRightSize) - (width - MeasurePane.MinSize);
                    dockLeftSize -= adjust / 2;
                    dockRightSize -= adjust / 2;
                }

                return dockState == DockState.DockLeft ? dockLeftSize : dockRightSize;
            }
            else if (dockState == DockState.DockTop || dockState == DockState.DockBottom)
            {
                int height = ClientRectangle.Height - DockPadding.Top - DockPadding.Bottom;
                int dockTopSize = m_dockTopPortion >= 1 ? (int)m_dockTopPortion : (int)(height * m_dockTopPortion);
                int dockBottomSize = m_dockBottomPortion >= 1 ? (int)m_dockBottomPortion : (int)(height * m_dockBottomPortion);

                if (dockTopSize < MeasurePane.MinSize)
                    dockTopSize = MeasurePane.MinSize;
                if (dockBottomSize < MeasurePane.MinSize)
                    dockBottomSize = MeasurePane.MinSize;

                if (dockTopSize + dockBottomSize > height - MeasurePane.MinSize)
                {
                    int adjust = (dockTopSize + dockBottomSize) - (height - MeasurePane.MinSize);
                    dockTopSize -= adjust / 2;
                    dockBottomSize -= adjust / 2;
                }

                return dockState == DockState.DockTop ? dockTopSize : dockBottomSize;
            }
            else
                return 0;
        }
        private Rectangle DocumentWindowBounds
        {
            get
            {
                Rectangle rectDocumentBounds = DisplayRectangle;
                if (DockWindows[DockState.DockLeft].Visible)
                {
                    rectDocumentBounds.X += DockWindows[DockState.DockLeft].Width;
                    rectDocumentBounds.Width -= DockWindows[DockState.DockLeft].Width;
                }
                if (DockWindows[DockState.DockRight].Visible)
                    rectDocumentBounds.Width -= DockWindows[DockState.DockRight].Width;
                if (DockWindows[DockState.DockTop].Visible)
                {
                    rectDocumentBounds.Y += DockWindows[DockState.DockTop].Height;
                    rectDocumentBounds.Height -= DockWindows[DockState.DockTop].Height;
                }
                if (DockWindows[DockState.DockBottom].Visible)
                    rectDocumentBounds.Height -= DockWindows[DockState.DockBottom].Height;

                return rectDocumentBounds;

            }
        }
        private void CalculateDockPadding()
        {
            DockPadding.All = 0;

            int height = AutoHideStripWindow.MeasureHeight();

            if (this.AutoHideStripWindow.PagesLeft.Count > 0)
                DockPadding.Left = height;
            if (this.AutoHideStripWindow.PagesRight.Count > 0)
                DockPadding.Right = height;
            if (this.AutoHideStripWindow.PagesTop.Count > 0)
                DockPadding.Top = height;
            if (this.AutoHideStripWindow.PagesBottom.Count > 0)
                DockPadding.Bottom = height;
        }

        private void UpdateDockWindowZOrder(DockStyle dockStyle, bool fullPanelEdge)
        {
            if (dockStyle == DockStyle.Left)
            {
                if (fullPanelEdge)
                    DockWindows[DockState.DockLeft].SendToBack();
                else
                    DockWindows[DockState.DockLeft].BringToFront();
            }
            else if (dockStyle == DockStyle.Right)
            {
                if (fullPanelEdge)
                    DockWindows[DockState.DockRight].SendToBack();
                else
                    DockWindows[DockState.DockRight].BringToFront();
            }
            else if (dockStyle == DockStyle.Top)
            {
                if (fullPanelEdge)
                    DockWindows[DockState.DockTop].SendToBack();
                else
                    DockWindows[DockState.DockTop].BringToFront();
            }
            else if (dockStyle == DockStyle.Bottom)
            {
                if (fullPanelEdge)
                    DockWindows[DockState.DockBottom].SendToBack();
                else
                    DockWindows[DockState.DockBottom].BringToFront();
            }
        }

        #endregion

        private bool ShouldSerializeDefaultFloatWindowSize()
        {
            return DefaultFloatWindowSize != new Size(300, 300);
        }

        protected override void OnParentChanged(EventArgs e)
        {
            SetAutoHideWindowParent();
            GetMdiClientController().ParentForm = (this.Parent as Form);
            base.OnParentChanged(e);
        }
        private void SetAutoHideWindowParent()
        {
            Control parent;
            if (DocumentStyle == DocumentStyle.DockingMdi ||
                DocumentStyle == DocumentStyle.SystemMdi)
                parent = this.Parent;
            else
                parent = this;
            if (AutoHideWindow.Parent != parent)
            {
                AutoHideWindow.Parent = parent;
                AutoHideWindow.BringToFront();
            }
        }

        internal void Clean()
        {
            for (int i = this.DockContents.Count - 1; i >= 0; i--)
            {
                DockContent content = this.DockContents[i];
                if (content.DockState != DockState.Document)
                {
                    continue;
                }
                if (content.DockPage != null)
                {
                    content.DockPage.RemoveDockContent(content);
                }
                this.DockContents.Remove(content);
                content.Dispose();
            }

            for ( int i =this.DockPages.Count -1; i>=0;i-- )
            {
                DockPage page = this.DockPages[i];
                if (page.DockContents.Count > 0)
                {
                    continue;
                }
                if (page.DockControl != null)
                {
                    page.DockControl.RemoveDockPage(page);
                }
                this.DockPages.Remove(page);
                page.Dispose();
            }
            for (int i = this.FloatWindows.Count - 1; i >= 0; i--)
            {
                FloatWindow fw = this.FloatWindows[i];
                if (fw.NestedPanes.Count > 0)
                {
                    continue;
                }
                this.FloatWindows.Remove(fw);
                fw.Dispose();
            }
        }

        #region SuspendLayout & ResumeLayout

        public void SuspendLayout(bool allWindows)
        {
            //FocusManager.SuspendFocusTracking();

            SuspendLayout();

            if (allWindows)
                SuspendMdiClientLayout();
        }

        public void ResumeLayout(bool performLayout, bool allWindows)
        {
            //FocusManager.ResumeFocusTracking();

            ResumeLayout(performLayout);

            if (allWindows)
                ResumeMdiClientLayout(performLayout);
        }

        private void MdiClient_Layout(object sender, LayoutEventArgs e)
        {
            if (DocumentStyle != DocumentStyle.DockingMdi)
                return;

            foreach (DockPage pane in DockPages)
                if (pane.DockState == DockState.Document)
                    pane.SetContentBounds();

            InvalidateWindowRegion();
        }

        private bool IsParentFormValid()
        {
            if (DocumentStyle == DocumentStyle.DockingSdi || DocumentStyle == DocumentStyle.DockingWindow)
                return true;

            if (!MdiClientExists)
                GetMdiClientController().RenewMdiClient();

            return (MdiClientExists);
        }
        internal Form ParentForm
        {
            get
            {
                if (!IsParentFormValid())
                    throw new InvalidOperationException(Strings.DockPanel_ParentForm_Invalid);

                return GetMdiClientController().ParentForm;
            }
        }
        private Rectangle SystemMdiClientBounds
        {
            get
            {
                if (!IsParentFormValid() || !Visible)
                    return Rectangle.Empty;

                Rectangle rect = ParentForm.RectangleToClient(RectangleToScreen(DocumentWindowBounds));
                return rect;
            }
        }
        private Control m_dummyControl;
        private Control DummyControl
        {
            get { return m_dummyControl; }
        }


        private PaintEventHandler m_dummyControlPaintEventHandler = null;
        private void InvalidateWindowRegion()
        {
            if (DesignMode)
                return;

            if (m_dummyControlPaintEventHandler == null)
                m_dummyControlPaintEventHandler = new PaintEventHandler(DummyControl_Paint);

            DummyControl.Paint += m_dummyControlPaintEventHandler;
            DummyControl.Invalidate();
        }

        void DummyControl_Paint(object sender, PaintEventArgs e)
        {
            DummyControl.Paint -= m_dummyControlPaintEventHandler;
            UpdateWindowRegion();
        }

        private void UpdateWindowRegion()
        {
            if (this.DocumentStyle == DocumentStyle.DockingMdi)
                UpdateWindowRegion_ClipContent();
            else if (this.DocumentStyle == DocumentStyle.DockingSdi ||
                this.DocumentStyle == DocumentStyle.DockingWindow)
                UpdateWindowRegion_FullDocumentArea();
            else if (this.DocumentStyle == DocumentStyle.SystemMdi)
                UpdateWindowRegion_EmptyDocumentArea();
        }

        private void UpdateWindowRegion_FullDocumentArea()
        {
            SetRegion(null);
        }

        private void UpdateWindowRegion_EmptyDocumentArea()
        {
            Rectangle rect = DocumentWindowBounds;
            SetRegion(new Rectangle[] { rect });
        }

        private void UpdateWindowRegion_ClipContent()
        {
            int count = 0;
            foreach (DockPage pane in this.DockPages)
            {
                if (!pane.Visible || pane.DockState != DockState.Document)
                    continue;

                count++;
            }

            if (count == 0)
            {
                SetRegion(null);
                return;
            }

            Rectangle[] rects = new Rectangle[count];
            int i = 0;
            foreach (DockPage pane in this.DockPages)
            {
                if (!pane.Visible || pane.DockState != DockState.Document)
                    continue;

                rects[i] = RectangleToClient(pane.RectangleToScreen(pane.ContentRectangle));
                i++;
            }

            SetRegion(rects);
        }

        private Rectangle[] m_clipRects = null;
        private void SetRegion(Rectangle[] clipRects)
        {
            if (!IsClipRectsChanged(clipRects))
                return;

            m_clipRects = clipRects;

            if (m_clipRects == null || m_clipRects.GetLength(0) == 0)
                Region = null;
            else
            {
                Region region = new Region(new Rectangle(0, 0, this.Width, this.Height));
                foreach (Rectangle rect in m_clipRects)
                    region.Exclude(rect);
                Region = region;
            }
        }

        private bool IsClipRectsChanged(Rectangle[] clipRects)
        {
            if (clipRects == null && m_clipRects == null)
                return false;
            else if ((clipRects == null) != (m_clipRects == null))
                return true;

            foreach (Rectangle rect in clipRects)
            {
                bool matched = false;
                foreach (Rectangle rect2 in m_clipRects)
                {
                    if (rect == rect2)
                    {
                        matched = true;
                        break;
                    }
                }
                if (!matched)
                    return true;
            }

            foreach (Rectangle rect2 in m_clipRects)
            {
                bool matched = false;
                foreach (Rectangle rect in clipRects)
                {
                    if (rect == rect2)
                    {
                        matched = true;
                        break;
                    }
                }
                if (!matched)
                    return true;
            }
            return false;
        }

        private void MdiClientHandleAssigned(object sender, EventArgs e)
        {
            SetMdiClient();
            PerformLayout();
        }

        #endregion
    }
}
