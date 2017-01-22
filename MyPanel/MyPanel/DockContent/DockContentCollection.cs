using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace Guoyongrong.WinFormsUI.Docking
{
    public class DockContentCollection : ReadOnlyCollection<DockContent>
    {
        public event DockContentEventHandler DockContentAdded;
        protected void onDockContentAdded(DockContentEventArgs e)
        {
            if (DockContentAdded!=null)
            {
                DockContentAdded(this, e);
            }
        }
        public event DockContentEventHandler DockContentRemoved;
        protected void onDockContentRemoved(DockContentEventArgs e)
        {
            if (DockContentRemoved != null)
            {
                DockContentRemoved(this, e);
            }
        }

        public DockContentCollection()
            : base(new List<DockContent>())
        {
        }

        internal int Add(DockContent content)
        {
            if (Items.Contains(content))
                return Items.IndexOf(content);

            Items.Add(content);
            onDockContentAdded(new DockContentEventArgs(content));
            return Count - 1;
        }

        internal void AddAt(DockContent content, int index)
        {
            if (index < 0 || index > Items.Count - 1)
                return;

            if (Contains(content))
                return;

            Items.Insert(index, content);
            onDockContentAdded(new DockContentEventArgs(content));
        }

        internal void Clear()
        {
            Items.Clear();
        }

        internal void Remove(DockContent content)
        {
            if(!Items.Contains(content)){
                return;
            }

            Items.Remove(content);

            onDockContentRemoved(new DockContentEventArgs(content));
        }
    }
}
