using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Guoyongrong.WinFormsUI.Docking
{
    public delegate void DockDragEventHandler(object sender, DockDragEventArgs e);

    public class DockDragEventArgs : EventArgs
    {
        public DockDragEventArgs()
        {
        }
        public DockDragEventArgs(IDragSource DragSource, Rectangle FloatWindowBounds)
        {
            this.DockDragOperation = DockDragOperation.FloatAt;
            this.DragSource = DragSource;
            this.FloatWindowBounds = FloatWindowBounds;

        }

        public DockDragEventArgs(IDragSource DragSource, object DockTo, DockStyle DockStyle, int ContentIndex)
        {
            this.DockDragOperation = DockDragOperation.DockTo;
            this.DragSource = DragSource;
            this.DockTo = DockTo;
            this.DockStyle = DockStyle;
            this.ContentIndex = ContentIndex;

        }

        public DockDragEventArgs(IDragSource DragSource, object DockTo, DockStyle DockStyle, bool FlagFullEdge)
        {
            this.DockDragOperation = DockDragOperation.DockTo;
            this.DragSource = DragSource;
            this.DockTo = DockTo;
            this.DockStyle = DockStyle;
            this.FlagFullEdge = FlagFullEdge;
        }


        private DockDragOperation m_DockDragOperation = DockDragOperation.DockTo;
        public DockDragOperation DockDragOperation
        {
            get { return m_DockDragOperation; }
            set { m_DockDragOperation = value; }
        }

        private IDragSource m_DragSource;
        public IDragSource DragSource
        {
            get { return m_DragSource; }
            set { m_DragSource = value; }
        }

        private Rectangle m_FloatWindowBounds;
        public Rectangle FloatWindowBounds
        {
            get { return m_FloatWindowBounds; }
            set { m_FloatWindowBounds = value; }
        }

        private DockStyle m_DockStyle;
        public DockStyle DockStyle
        {
            get { return m_DockStyle; }
            set { m_DockStyle = value; }
        }

        private int m_ContentIndex;
        public int ContentIndex
        {
            get { return m_ContentIndex; }
            set { m_ContentIndex = value; }
        }

        private Object m_DockTo;
        public Object DockTo
        {
            get { return m_DockTo; }
            set { m_DockTo = value; }
        }

        private bool m_FlagFullEdge;
        public bool FlagFullEdge
        {
            get { return m_FlagFullEdge; }
            set { m_FlagFullEdge = value; }
        }

    }
}
