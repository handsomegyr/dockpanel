using System;
using System.Collections;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Text;

namespace Guoyongrong.WinFormsUI.Docking
{
    public abstract partial class AutoHideStripBase : Panel, IDockPageContainer
    {
        public event DockContentEventHandler ActiveContentChanged;
        protected virtual void OnActiveContentChanged(DockContentEventArgs e)
        {
            if (ActiveContentChanged != null)
                ActiveContentChanged(this, e);
        }

        private DockContent m_activeContent = null;
        public DockContent ActiveContent
        {
            get { return m_activeContent; }
            private set
            {
                if (value == null)
                {
                    throw (new InvalidOperationException(Strings.DockPane_ActiveContent_InvalidValue));
                }

                m_activeContent = value;

                DockContentEventArgs e = new DockContentEventArgs(m_activeContent);

                OnActiveContentChanged(e);

            }
        }

        
        protected AutoHideStripBase()
        {
            SetStyle(ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer, true);
            BackColor = SystemColors.ControlLight;
            SetStyle(ControlStyles.Selectable, false);
        }
        
        #region Paint & Layout

        private bool GetDockPageByState(DockState state, DockPage page)
        {
            if (page != null && state == page.DockState && !page.IsHidden)
            {
                return true;
            }
            return false;
        }
        private bool GetDockPageByDockTopAutoHide(DockPage page)
        {
            return GetDockPageByState(DockState.DockTopAutoHide,page);
        }
        private bool GetDockPageByDockLeftAutoHide(DockPage page)
        {
            return GetDockPageByState(DockState.DockLeftAutoHide, page);
        }
        private bool GetDockPageByDockRightAutoHide(DockPage page)
        {
            return GetDockPageByState(DockState.DockRightAutoHide, page);
        }
        private bool GetDockPageByDockBottomAutoHide(DockPage page)
        {
            return GetDockPageByState(DockState.DockBottomAutoHide, page);
        }
        protected List<DockPage> GetDockPages(DockState state)
        {
            GetDockPageFunction getDockPage = null;
            switch (state)
            {
                case DockState.DockTopAutoHide:
                    getDockPage = GetDockPageByDockTopAutoHide;
                    break;
                case DockState.DockLeftAutoHide:
                    getDockPage = GetDockPageByDockLeftAutoHide;
                    break;
                case DockState.DockRightAutoHide:
                    getDockPage = GetDockPageByDockRightAutoHide;
                    break;
                case DockState.DockBottomAutoHide:
                    getDockPage = GetDockPageByDockBottomAutoHide;
                    break;
            }

            List<DockPage> list = new List<DockPage>(DockPageCollection.GetDockPages(getDockPage));

            return list;
        }

        public List<DockPage> PagesTop
        {
            get
            {
                return GetDockPages(DockState.DockTopAutoHide);
            }
        }

        public List<DockPage> PagesBottom
        {
            get
            {
                return GetDockPages(DockState.DockBottomAutoHide);
            }
        }

        public List<DockPage> PagesLeft
        {
            get
            {
                return GetDockPages(DockState.DockLeftAutoHide);
            }
        }

        public List<DockPage> PagesRight
        {
            get
            {
                return GetDockPages(DockState.DockRightAutoHide);
            }
        }

        protected Rectangle RectangleTopLeft
        {
            get
            {
                int height = MeasureHeight();
                return PagesTop.Count > 0 && PagesLeft.Count > 0 ? new Rectangle(0, 0, height, height) : Rectangle.Empty;
            }
        }

        protected Rectangle RectangleTopRight
        {
            get
            {
                int height = MeasureHeight();
                return PagesTop.Count > 0 && PagesRight.Count > 0 ? new Rectangle(Width - height, 0, height, height) : Rectangle.Empty;
            }
        }

        protected Rectangle RectangleBottomLeft
        {
            get
            {
                int height = MeasureHeight();
                return PagesBottom.Count > 0 && PagesLeft.Count > 0 ? new Rectangle(0, Height - height, height, height) : Rectangle.Empty;
            }
        }

        protected Rectangle RectangleBottomRight
        {
            get
            {
                int height = MeasureHeight();
                return PagesBottom.Count > 0 && PagesRight.Count > 0 ? new Rectangle(Width - height, Height - height, height, height) : Rectangle.Empty;
            }
        }

        private GraphicsPath m_displayingArea = null;
        private GraphicsPath DisplayingArea
        {
            get
            {
                if (m_displayingArea == null)
                    m_displayingArea = new GraphicsPath();

                return m_displayingArea;
            }
        }

        private void SetRegion()
        {
            DisplayingArea.Reset();
            DisplayingArea.AddRectangle(RectangleTopLeft);
            DisplayingArea.AddRectangle(RectangleTopRight);
            DisplayingArea.AddRectangle(RectangleBottomLeft);
            DisplayingArea.AddRectangle(RectangleBottomRight);
            DisplayingArea.AddRectangle(GetTabStripRectangle(DockState.DockTopAutoHide));
            DisplayingArea.AddRectangle(GetTabStripRectangle(DockState.DockBottomAutoHide));
            DisplayingArea.AddRectangle(GetTabStripRectangle(DockState.DockLeftAutoHide));
            DisplayingArea.AddRectangle(GetTabStripRectangle(DockState.DockRightAutoHide));
            Region = new Region(DisplayingArea);
        }

        #endregion

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.CompositingMode = CompositingMode.SourceOver;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button != MouseButtons.Left)
                return;

            DockContent content = HitTest();
            if (content == null)
                return;
        }
        protected override void OnMouseHover(EventArgs e)
        {
            base.OnMouseHover(e);

            DockContent content = HitTest();
            
            if (content == null)
                return;

            ActiveContent = content;

            // requires further tracking of mouse hover behavior,
            ResetMouseEventArgs();
        }
        protected override void OnLayout(LayoutEventArgs levent)
        {
            RefreshChanges();
            base.OnLayout(levent);
        }

        protected virtual void OnRefreshChanges(){}
        private DockContent HitTest()
        {
            Point ptMouse = PointToClient(Control.MousePosition);
            return HitTest(ptMouse);
        }
        protected abstract DockContent HitTest(Point point);

        #region IDockPageContainer

        private Guid m_Guid = Guid.NewGuid();
        public Guid GUID
        {
            get { return m_Guid; }
            set { m_Guid = value; }
        }

        public bool IsAutoHide
        {
            get { return true; }
        }
        public bool IsFloat
        {
            get { return false; }
        }

        private DockState m_dockState = DockState.DockAutoHide;
        public DockState DockState
        {
            get { return m_dockState; }
        }

        public Rectangle DisplayingRectangle
        {
            get { return this.ClientRectangle; }
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

            page.Bounds = Rectangle.Empty;

            DockPageCollection.Add(page);

            //page.DockControl = this;

            RefreshChanges();

            this.PerformLayout();
        }

        public void RemoveDockPage(DockPage page)
        {
            //如果page为空
            if (page == null)
            {
                return;
            }

            DockPageCollection.Remove(page);
            //page.DockControl = null;

            RefreshChanges();
            this.PerformLayout();
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

        public void RefreshChanges()
        {
            if (IsDisposed)
                return;

            SetRegion();
            OnRefreshChanges();
        }

        #endregion

        private AutoHideStripSkin m_skin = new AutoHideStripSkin();
        public AutoHideStripSkin Skin
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

        public Rectangle GetTabStripRectangle(DockState dockState)
        {
            int height = MeasureHeight();
            if (dockState == DockState.DockTopAutoHide && PagesTop.Count > 0)
                return new Rectangle(RectangleTopLeft.Width, 0, Width - RectangleTopLeft.Width - RectangleTopRight.Width, height);
            else if (dockState == DockState.DockBottomAutoHide && PagesBottom.Count > 0)
                return new Rectangle(RectangleBottomLeft.Width, Height - height, Width - RectangleBottomLeft.Width - RectangleBottomRight.Width, height);
            else if (dockState == DockState.DockLeftAutoHide && PagesLeft.Count > 0)
                return new Rectangle(0, RectangleTopLeft.Width, height, Height - RectangleTopLeft.Height - RectangleBottomLeft.Height);
            else if (dockState == DockState.DockRightAutoHide && PagesRight.Count > 0)
                return new Rectangle(Width - height, RectangleTopRight.Width, height, Height - RectangleTopRight.Height - RectangleBottomRight.Height);
            else
                return Rectangle.Empty;
        }

        public abstract int MeasureHeight();

    }
}
