using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Guoyongrong.WinFormsUI.Docking
{
    public class SplitterBase : Control
    {
        public event EventHandler SplitterMove;
        private void OnMoveSplitter(EventArgs e)
        {
            if (SplitterMove != null)
            {
                SplitterMove(this, e);
            }
        }


        public SplitterBase()
        {
            SetStyle(ControlStyles.Selectable, false);
        }

        public override DockStyle Dock
        {
            get { return base.Dock; }
            set
            {
                SuspendLayout();
                base.Dock = value;

                if (Dock == DockStyle.Left || Dock == DockStyle.Right)
                    Width = SplitterSize;
                else if (Dock == DockStyle.Top || Dock == DockStyle.Bottom)
                    Height = SplitterSize;
                else
                    Bounds = Rectangle.Empty;

                if (Dock == DockStyle.Left || Dock == DockStyle.Right)
                    Cursor = Cursors.VSplit;
                else if (Dock == DockStyle.Top || Dock == DockStyle.Bottom)
                    Cursor = Cursors.HSplit;
                else
                    Cursor = Cursors.Default;

                ResumeLayout();
            }
        }

        protected virtual int SplitterSize
        {
            get { return Measures.SplitterSize; }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button != MouseButtons.Left)
                return;

            StartDrag();

            OnMoveSplitter(e);
        }

        protected virtual void StartDrag()
        {
        }

        protected override void WndProc(ref Message m)
        {
            // eat the WM_MOUSEACTIVATE message
            if (m.Msg == (int)Win32.Msgs.WM_MOUSEACTIVATE)
                return;

            base.WndProc(ref m);
        }
    }
}
