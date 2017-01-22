using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace Guoyongrong.WinFormsUI.Docking
{
    public delegate bool GetDockPageFunction(DockPage page);

    public class DockPageCollection : ReadOnlyCollection<DockPage>
    {
        internal event DockPageEventHandler DockPageAdded;
        private void onDockPageAdded(DockPageEventArgs e)
        {
            if (DockPageAdded != null)
            {
                DockPageAdded(null, e);
            }
        }
        
        internal event DockPageEventHandler DockPageRemoved;
        private void onDockPageRemoved(DockPageEventArgs e)
        {
            if (DockPageRemoved != null)
            {
                DockPageRemoved(null, e);
            }
        }

        internal DockPageCollection()
            : base(new List<DockPage>())
        {
        }

        internal IEnumerable<DockPage> GetDockPages(GetDockPageFunction getDockPage)
        {
            foreach (DockPage page in Items)
            {
                if (getDockPage(page))
                {
                    yield return page;
                }
            }
        }

        internal int Add(DockPage page)
        {
            if (Items.Contains(page))
                return Items.IndexOf(page);

            Items.Add(page);
            onDockPageAdded(new DockPageEventArgs(page));
            return Count - 1;
        }

        internal void AddAt(DockPage page, int index)
        {
            if (index < 0 || index > Items.Count - 1)
                return;

            if (Contains(page))
                return;

            Items.Insert(index, page);
            onDockPageAdded(new DockPageEventArgs(page));
        }

        internal void Remove(DockPage page)
        {
            if (!Items.Contains(page))
            {
                return;
            }
            Items.Remove(page);
            onDockPageRemoved(new DockPageEventArgs(page));
        }
        
    }
}
