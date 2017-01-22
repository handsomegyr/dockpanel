using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Guoyongrong.WinFormsUI.Docking
{
    public interface IDragSource
    {
        Control DragControl { get; }
    }

    public interface IDockDragProcessor
    {
        Rectangle BeginDrag(IDragSource DragSource, Point ptMouse);
        void TestDrop(IDragSource TestDropSource, IDragSource dragSource, DockOutlineBase dockOutline);
        bool IsDockStateValid(IDragSource DragSource, DockState dockState);
        bool CanDockTo(IDragSource DragSource, DockPage pane);
        void FloatAt(IDragSource DragSource, Rectangle floatWindowBounds);
        void DockTo(IDragSource DragSource, DockPage pane, DockStyle dockStyle, int contentIndex);
        void DockTo(IDragSource DragSource, DockPanel panel, DockStyle dockStyle);
    }

    public interface ISplitterDragProcessor
    {
        void BeginDrag(IDragSource DragSource, Rectangle rectSplitter);
        void EndDrag(IDragSource DragSource);
        bool IsVertical(IDragSource DragSource);
        Rectangle DragLimitBounds(IDragSource DragSource);
        void MoveSplitter(IDragSource DragSource, int offset);
    }
}
