using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Forms;

namespace Guoyongrong.WinFormsUI.Docking
{
    public sealed class VisibleNestedPageCollection : ReadOnlyCollection<DockPage>
    {
        private NestedPageCollection m_nestedPanes;

        internal VisibleNestedPageCollection(NestedPageCollection nestedPanes)
            : base(new List<DockPage>())
        {
            m_nestedPanes = nestedPanes;
        }

        internal NestedPageCollection NestedPanes
        {
            get { return m_nestedPanes; }
        }

        public IDockPageContainer Container
        {
            get { return NestedPanes.Container; }
        }

        public DockState DockState
        {
            get { return NestedPanes.DockState; }
        }

        internal void Refresh()
        {
            Items.Clear();
            for (int i = 0; i < NestedPanes.Count; i++)
            {
                DockPage pane = NestedPanes[i];
                NestedDockingStatus status = pane.NestedDockingStatus;
                status.SetDisplayingStatus(true, status.PreviousPane, status.Alignment, status.Proportion);
                Items.Add(pane);

            }

            foreach (DockPage pane in NestedPanes)
                if (pane.Parent != this.Container || pane.DockState != DockState || pane.IsHidden)
                {
                    pane.Bounds = Rectangle.Empty;
                    pane.SplitterBounds = Rectangle.Empty;
                    Remove(pane);
                }

            CalculateBounds();

            foreach (DockPage pane in this)
            {
                NestedDockingStatus status = pane.NestedDockingStatus;
                pane.Bounds = status.PaneBounds;
                pane.SplitterBounds = status.SplitterBounds;
                pane.SplitterAlignment = status.Alignment;
            }
        }

        private void Remove(DockPage pane)
        {
            if (!Contains(pane))
                return;

            NestedDockingStatus statusPane = pane.NestedDockingStatus;
            DockPage lastNestedPane = null;
            for (int i = Count - 1; i > IndexOf(pane); i--)
            {
                NestedDockingStatus statusi = this[i].NestedDockingStatus;
                if (statusi.DisplayingPreviousPane == pane)
                {
                    lastNestedPane = this[i];
                    break;
                }
            }

            if (lastNestedPane != null)
            {
                int indexLastNestedPane = IndexOf(lastNestedPane);
                Items.Remove(lastNestedPane);
                Items[IndexOf(pane)] = lastNestedPane;

                NestedDockingStatus lastNestedDock = lastNestedPane.NestedDockingStatus;
                lastNestedDock.SetDisplayingStatus(true, statusPane.DisplayingPreviousPane, statusPane.DisplayingAlignment, statusPane.DisplayingProportion);
                for (int i = indexLastNestedPane - 1; i > IndexOf(lastNestedPane); i--)
                {
                    NestedDockingStatus status = this[i].NestedDockingStatus;
                    if (status.DisplayingPreviousPane == pane)
                        status.SetDisplayingStatus(true, lastNestedPane, status.DisplayingAlignment, status.DisplayingProportion);
                }
            }
            else
                Items.Remove(pane);

            statusPane.SetDisplayingStatus(false, null, DockAlignment.Unknown, 0.5);
        }

        private void CalculateBounds()
        {
            if (Count == 0)
                return;
            
            this[0].NestedDockingStatus.SetDisplayingBounds(Container.DisplayingRectangle, Container.DisplayingRectangle, Rectangle.Empty);
            
            for (int i = 1; i < Count; i++)
            {
                DockPage pane = this[i];
                NestedDockingStatus status = pane.NestedDockingStatus;
                
                DockPage prevPane = status.DisplayingPreviousPane;
                NestedDockingStatus statusPrev = prevPane.NestedDockingStatus;
                
                Rectangle rect = statusPrev.PaneBounds;
                bool bVerticalSplitter = (status.DisplayingAlignment == DockAlignment.Left || status.DisplayingAlignment == DockAlignment.Right);

                Rectangle rectThis = rect;
                Rectangle rectPrev = rect;
                Rectangle rectSplitter = rect;
                if (status.DisplayingAlignment == DockAlignment.Left)
                {
                    rectThis.Width = (int)((double)rect.Width * status.DisplayingProportion) - (Measures.SplitterSize / 2);
                    rectSplitter.X = rectThis.X + rectThis.Width;
                    rectSplitter.Width = Measures.SplitterSize;
                    rectPrev.X = rectSplitter.X + rectSplitter.Width;
                    rectPrev.Width = rect.Width - rectThis.Width - rectSplitter.Width;
                }
                else if (status.DisplayingAlignment == DockAlignment.Right)
                {
                    rectPrev.Width = (rect.Width - (int)((double)rect.Width * status.DisplayingProportion)) - (Measures.SplitterSize / 2);
                    rectSplitter.X = rectPrev.X + rectPrev.Width;
                    rectSplitter.Width = Measures.SplitterSize;
                    rectThis.X = rectSplitter.X + rectSplitter.Width;
                    rectThis.Width = rect.Width - rectPrev.Width - rectSplitter.Width;
                }
                else if (status.DisplayingAlignment == DockAlignment.Top)
                {
                    rectThis.Height = (int)((double)rect.Height * status.DisplayingProportion) - (Measures.SplitterSize / 2);
                    rectSplitter.Y = rectThis.Y + rectThis.Height;
                    rectSplitter.Height = Measures.SplitterSize;
                    rectPrev.Y = rectSplitter.Y + rectSplitter.Height;
                    rectPrev.Height = rect.Height - rectThis.Height - rectSplitter.Height;
                }
                else if (status.DisplayingAlignment == DockAlignment.Bottom)
                {
                    rectPrev.Height = (rect.Height - (int)((double)rect.Height * status.DisplayingProportion)) - (Measures.SplitterSize / 2);
                    rectSplitter.Y = rectPrev.Y + rectPrev.Height;
                    rectSplitter.Height = Measures.SplitterSize;
                    rectThis.Y = rectSplitter.Y + rectSplitter.Height;
                    rectThis.Height = rect.Height - rectPrev.Height - rectSplitter.Height;
                }
                else
                    rectThis = Rectangle.Empty;

                rectSplitter.Intersect(rect);
                rectThis.Intersect(rect);
                rectPrev.Intersect(rect);
                status.SetDisplayingBounds(rect, rectThis, rectSplitter);
                statusPrev.SetDisplayingBounds(statusPrev.LogicalBounds, rectPrev, statusPrev.SplitterBounds);
            }
        }
    }
}
