using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Guoyongrong.WinFormsUI.Docking
{
    public partial class DockWindow : Panel, IDragSource,IDockPageContainer
    {
        private SplitterBase m_splitter;
        public SplitterBase Splitter
        {
            get { return m_splitter; }
        }

        private NestedPageCollection m_nestedPanes;
        internal NestedPageCollection NestedPanes
        {
            get { return m_nestedPanes; }
        }
        internal VisibleNestedPageCollection VisibleNestedPanes
        {
            get { return NestedPanes.VisibleNestedPanes; }
        }
        public DockPage GetDefaultPreviousPane(DockPage pane)
        {
            return NestedPanes.GetDefaultPreviousPane(pane);
        }
        public DockWindow()
        {
            InitializeComponent();

            this.Visible = false;
            m_nestedPanes = new NestedPageCollection(this);

            SuspendLayout();

            m_splitter = new SplitterBase();
            Controls.Add(m_splitter);

            DockPage page1 = new DockPage();
            this.AddDockPage(page1);

            RefreshChanges();

            ResumeLayout();
        }
     
        protected override void OnPaint(PaintEventArgs e)
        {
            // if DockWindow is document, draw the border
            if (DockState == DockState.Document)
                e.Graphics.DrawRectangle(
                    SystemPens.ControlDark, 
                    ClientRectangle.X, ClientRectangle.Y, 
                    ClientRectangle.Width - 1, ClientRectangle.Height - 1);

            base.OnPaint(e);
        }
        protected override void OnLayout(LayoutEventArgs levent)
        {
            VisibleNestedPanes.Refresh();
            if (VisibleNestedPanes.Count == 0)
            {
                if (Visible) Visible = false;
            }
            else if (!Visible)
            {
                Visible = true;
                VisibleNestedPanes.Refresh();
            }

            base.OnLayout(levent);
        }
                
        #region IDockPageContainer

        private Guid m_Guid = Guid.NewGuid();
        public Guid GUID
        {
            get { return m_Guid; }
        }

        public bool IsAutoHide
        {
            get { return false; }
        }
        public bool IsFloat
        {
            get { return false; }
        }
        public Rectangle DisplayingRectangle
        {
            get
            {
                Rectangle rect = ClientRectangle;
                // if DockWindow is document, exclude the border
                if (DockState == DockState.Document)
                {
                    rect.X += 1;
                    rect.Y += 1;
                    rect.Width -= 2;
                    rect.Height -= 2;
                }
                // exclude the splitter
                else if (DockState == DockState.DockLeft)
                    rect.Width -= Measures.SplitterSize;
                else if (DockState == DockState.DockRight)
                {
                    rect.X += Measures.SplitterSize;
                    rect.Width -= Measures.SplitterSize;
                }
                else if (DockState == DockState.DockTop)
                    rect.Height -= Measures.SplitterSize;
                else if (DockState == DockState.DockBottom)
                {
                    rect.Y += Measures.SplitterSize;
                    rect.Height -= Measures.SplitterSize;
                }

                return rect;
            }
        }

        internal void SetDockPageParent(DockPage page)
        {
            page.SetParent(this);
            page.Splitter.Parent = page.Parent;
        }
        public void AddDockPage(DockPage page)
        {
            NestedDockingStatus status = page.NestedDockingStatus;
            AddDockPage(page, status.PreviousPane, status.Alignment, status.Proportion, -1);
        }

        public void AddDockPage(DockPage page, DockPage previousPage, DockAlignment alignment, double proportion, int contentIndex)
        {
            //如果page为空
            if (page == null)
            {
                return;
            }

            page.DockState = this.DockState;
            page.DockControl = this;
            SetDockPageParent(page);
                        
            this.NestedPanes.Add(page, previousPage, alignment, proportion, contentIndex);
            this.DockPageCollection.Add(page);
            
            if (this.NestedPanes.Contains(page)) this.PerformLayout();

        }
    
        public void RemoveDockPage(DockPage page)
        {
            //pan.Visible = false;
            //如果page为空
            if (page == null)
            {
                return;
            }
            
            bool isContained = this.NestedPanes.Contains(page);
            if (isContained)
            {
                page.DockControl = null;
                page.SetParent(null);
                page.Splitter.Parent = null;
            }

            this.NestedPanes.Remove(page);
            this.DockPageCollection.Remove(page);
            
            if (isContained) this.PerformLayout();
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

        private DockState m_dockState = DockState.Document;
        public DockState DockState
        {
            get { return m_dockState; }
            set
            {
                if (value == m_dockState)
                {
                    return;
                }

                if (value == DockState.DockLeft ||
                    value == DockState.DockRight ||
                    value == DockState.DockTop ||
                    value == DockState.DockBottom ||
                    value == DockState.Document)
                {
                    m_dockState = value;
                    RefreshChanges();
                }

            }
        }
        
        #endregion

        public void RefreshChanges()
        {
            if (DockState == DockState.DockLeft ||
                DockState == DockState.DockRight ||
                DockState == DockState.DockTop ||
                DockState == DockState.DockBottom)
            {
                m_splitter.Visible = true;
            }
            else
            {
                m_splitter.Visible = false;
            }

            if (DockState == DockState.DockLeft)
            {
                Dock = DockStyle.Left;
                m_splitter.Dock = DockStyle.Right;
            }
            else if (DockState == DockState.DockRight)
            {
                Dock = DockStyle.Right;
                m_splitter.Dock = DockStyle.Left;
            }
            else if (DockState == DockState.DockTop)
            {
                Dock = DockStyle.Top;
                m_splitter.Dock = DockStyle.Bottom;
            }
            else if (DockState == DockState.DockBottom)
            {
                Dock = DockStyle.Bottom;
                m_splitter.Dock = DockStyle.Top;
            }
            else if (DockState == DockState.Document)
            {
                Dock = DockStyle.Fill;
            }
            foreach (DockPage page in this.NestedPanes)
            {
                page.DockState = this.DockState;
            }
        }
        
        #region ISplitterDragSource Members

        #region IDragSource Members

        Control IDragSource.DragControl
        {
            get { return this; }
        }

        #endregion
        #endregion

    }
}
