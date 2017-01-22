using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Guoyongrong.WinFormsUI.Docking
{
    [ToolboxItem(false)]
    public class DockPageSplitter : SplitterBase, IDragSource
    {
        DockPage m_pane;

        public DockPageSplitter(DockPage pane)
        {
            m_pane = pane;
        }

        public DockPage DockPane
        {
            get { return m_pane; }
        }

        private DockAlignment m_alignment;
        public DockAlignment Alignment
        {
            get { return m_alignment; }
            set
            {
                m_alignment = value;
                if (m_alignment == DockAlignment.Left || m_alignment == DockAlignment.Right)
                    Cursor = Cursors.VSplit;
                else if (m_alignment == DockAlignment.Top || m_alignment == DockAlignment.Bottom)
                    Cursor = Cursors.HSplit;
                else
                    Cursor = Cursors.Default;

                if (DockPane.DockState == DockState.Document)
                    Invalidate();
            }
        }

        [Browsable(false)]
        public new DockStyle Dock
        {
            get { return base.Dock; }
            set
            { base.Dock = value; }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (DockPane.DockState != DockState.Document)
                return;

            Graphics g = e.Graphics;
            Rectangle rect = ClientRectangle;
            if (Alignment == DockAlignment.Top || Alignment == DockAlignment.Bottom)
                g.DrawLine(SystemPens.ControlDark, rect.Left, rect.Bottom - 1, rect.Right, rect.Bottom - 1);
            else if (Alignment == DockAlignment.Left || Alignment == DockAlignment.Right)
                g.DrawLine(SystemPens.ControlDarkDark, rect.Right - 1, rect.Top, rect.Right - 1, rect.Bottom);
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