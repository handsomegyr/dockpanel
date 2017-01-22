using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;

namespace Guoyongrong.WinFormsUI.Docking
{
    internal sealed class NestedPageCollection : ReadOnlyCollection<DockPage>
    {
        private VisibleNestedPageCollection m_visibleNestedPanes;
        private IDockPageContainer m_container;
        internal NestedPageCollection(IDockPageContainer container)
            : base(new List<DockPage>())
        {
            m_container = container;
            m_visibleNestedPanes = new VisibleNestedPageCollection(this);
        }

        public IDockPageContainer Container
        {
            get { return m_container; }
        }

        public VisibleNestedPageCollection VisibleNestedPanes
        {
            get { return m_visibleNestedPanes; }
        }

        public DockState DockState
        {
            get { return m_container.DockState; }
        }

        internal void Add(DockPage page, DockPage prevPage, DockAlignment alignment, double proportion, int contentIndex)
        {
            if (alignment == DockAlignment.Fill)
            {
                return;
            }

            //如果DockAlignment为未定的时候，先取得默认值
            if (alignment == DockAlignment.Unknown)
            {
                if (this.DockState == DockState.DockLeft ||
                    this.DockState == DockState.DockRight)
                    alignment = DockAlignment.Bottom;
                else
                    alignment = DockAlignment.Right;
            }

            //如果该page已经停靠在这个控件上的时候
            if (Items.Contains(page))
            {
                //取得原来的停靠状态
                NestedDockingStatus status = page.NestedDockingStatus;
                //如果原来的停靠状态和新的状态是一致的话
                if ((status.PreviousPane == prevPage &&
                     status.Alignment == alignment &&
                     status.Proportion == proportion))
                {
                    return;
                }
            }
            //如果是新来的停靠控件的话，当前个PAGE未指定的时候，先取得默认的PAGE
            if (prevPage == null)
            {
                prevPage = this.GetDefaultPreviousPane(page);
            }

            //double proportion = 0.5;
            //做一些检查
            int count = this.Count;
            if (this.Contains(page))
                count--;
            if (prevPage == null && count > 0)
                throw new InvalidOperationException(Strings.DockPane_DockTo_NullPrevPane);

            if (prevPage != null && !this.Contains(prevPage))
                throw new InvalidOperationException(Strings.DockPane_DockTo_NoPrevPane);

            if (prevPage == page)
                throw new InvalidOperationException(Strings.DockPane_DockTo_SelfPrevPane);

            //pan.Visible = true;
            if (alignment != DockAlignment.Fill)
            {
                Items.Add(page);
                page.NestedDockingStatus.SetStatus(this.Container, prevPage, alignment, proportion);
            }

        }

        internal void Remove(DockPage pane)
        {
            InternalRemove(pane);
        }

        private void InternalRemove(DockPage pane)
        {
            if (!Contains(pane))
                return;

            DockPage lastNestedPane = null;

            for (int i = Count - 1; i > IndexOf(pane); i--)
            {
                NestedDockingStatus statusi = this[i].NestedDockingStatus;
                if (statusi.PreviousPane == pane)
                {
                    lastNestedPane = this[i];
                    break;
                }
            }
            
            NestedDockingStatus statusPane = pane.NestedDockingStatus;
            if (lastNestedPane != null)
            {
                int indexLastNestedPane = IndexOf(lastNestedPane);
                Items.Remove(lastNestedPane);
                if (indexLastNestedPane == -1)
                {
                    indexLastNestedPane = Items.Count; 
                }
                Items[IndexOf(pane)] = lastNestedPane;
                NestedDockingStatus lastNestedDock = lastNestedPane.NestedDockingStatus;

                lastNestedDock.SetStatus(this.Container, statusPane.PreviousPane, statusPane.Alignment, statusPane.Proportion);
                for (int i = indexLastNestedPane - 1; i > IndexOf(lastNestedPane); i--)
                {
                    NestedDockingStatus status = this[i].NestedDockingStatus;
                    if (status.PreviousPane == pane)
                        status.SetStatus(this.Container, lastNestedPane, status.Alignment, status.Proportion);
                }
            }
            else
            {
                Items.Remove(pane);
            }

            statusPane.SetStatus(null, null, DockAlignment.Unknown, 0.5);
            statusPane.SetDisplayingStatus(false, null, DockAlignment.Unknown, 0.5);
            statusPane.SetDisplayingBounds(Rectangle.Empty, Rectangle.Empty, Rectangle.Empty);

        }

        public DockPage GetDefaultPreviousPane(DockPage pane)
        {
            for (int i = Count - 1; i >= 0; i--)
                if (this[i] != pane)
                    return this[i];

            return null;
        }
    }
}
