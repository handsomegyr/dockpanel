using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;

namespace Guoyongrong.WinFormsUI.Docking
{
    partial class DockPanel
    {
        public sealed class SplitterDragHandler : DragHandler
        {
            private class SplitterOutline
            {
                public SplitterOutline()
                {
                    m_dragForm = new DragForm();
                    SetDragForm(Rectangle.Empty);
                    DragForm.BackColor = Color.Black;
                    DragForm.Opacity = 0.7;
                    DragForm.Show(false);
                }

                DragForm m_dragForm;
                private DragForm DragForm
                {
                    get { return m_dragForm; }
                }

                public void Show(Rectangle rect)
                {
                    SetDragForm(rect);
                }

                public void Close()
                {
                    DragForm.Close();
                }

                private void SetDragForm(Rectangle rect)
                {
                    DragForm.Bounds = rect;
                    if (rect == Rectangle.Empty)
                        DragForm.Region = new Region(Rectangle.Empty);
                    else if (DragForm.Region != null)
                        DragForm.Region = null;
                }
            }

            private DockPanel m_dockPanel;
            public DockPanel DockPanel
            {
                get { return m_dockPanel; }
            }

            public ISplitterDragProcessor DockDragProcessor
            {
                get { return m_dockPanel as ISplitterDragProcessor; }
            }

            public SplitterDragHandler(DockPanel dockPanel)
                : base()
            {
                m_dockPanel = dockPanel;
            }

            public new IDragSource DragSource
            {
                get { return base.DragSource; }
                private set { base.DragSource = value; }
            }

            private SplitterOutline m_outline;
            private SplitterOutline Outline
            {
                get { return m_outline; }
                set { m_outline = value; }
            }

            private Rectangle m_rectSplitter;
            private Rectangle RectSplitter
            {
                get { return m_rectSplitter; }
                set { m_rectSplitter = value; }
            }

            public void BeginDrag(IDragSource dragSource, Rectangle rectSplitter)
            {
                DragSource = dragSource;
                RectSplitter = rectSplitter;

                if (!BeginDrag())
                {
                    DragSource = null;
                    return;
                }

                Outline = new SplitterOutline();
                Outline.Show(rectSplitter);
                DockDragProcessor.BeginDrag(DragSource, rectSplitter);
                //DragSource.BeginDrag(rectSplitter);
            }

            protected override void OnDragging()
            {
                Outline.Show(GetSplitterOutlineBounds(Control.MousePosition));
            }

            protected override void OnEndDrag(bool abort)
            {
                DockPanel.SuspendLayout(true);

                Outline.Close();

                if (!abort)
                {
                    DockDragProcessor.MoveSplitter(DragSource, GetMovingOffset(Control.MousePosition));
                    //DragSource.MoveSplitter(GetMovingOffset(Control.MousePosition));
                }

                DockDragProcessor.EndDrag(DragSource);
                //DragSource.EndDrag();

                DockPanel.ResumeLayout(true, true);
            }

            private int GetMovingOffset(Point ptMouse)
            {
                Rectangle rect = GetSplitterOutlineBounds(ptMouse);
                if (DockDragProcessor.IsVertical(DragSource))
                    //if (DragSource.IsVertical)
                    return rect.X - RectSplitter.X;
                else
                    return rect.Y - RectSplitter.Y;
            }

            private Rectangle GetSplitterOutlineBounds(Point ptMouse)
            {
                Rectangle rectLimit = DockDragProcessor.DragLimitBounds(DragSource);
                //Rectangle rectLimit = DragSource.DragLimitBounds;

                Rectangle rect = RectSplitter;
                if (rectLimit.Width <= 0 || rectLimit.Height <= 0)
                    return rect;

                if (DockDragProcessor.IsVertical(DragSource))
                //if (DragSource.IsVertical)
                {
                    rect.X += ptMouse.X - StartMousePosition.X;
                    rect.Height = rectLimit.Height;
                }
                else
                {
                    rect.Y += ptMouse.Y - StartMousePosition.Y;
                    rect.Width = rectLimit.Width;
                }

                if (rect.Left < rectLimit.Left)
                    rect.X = rectLimit.X;
                if (rect.Top < rectLimit.Top)
                    rect.Y = rectLimit.Y;
                if (rect.Right > rectLimit.Right)
                    rect.X -= rect.Right - rectLimit.Right;
                if (rect.Bottom > rectLimit.Bottom)
                    rect.Y -= rect.Bottom - rectLimit.Bottom;

                return rect;
            }
        }
    }
}
