using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Forms;

namespace Guoyongrong.WinFormsUI.Docking
{
    internal class FloatWindowCollection : ReadOnlyCollection<FloatWindow>
	{
        public event FloatWindowEventHandler FloatWindowAdded;
        private void onFloatWindowAdded(FloatWindowEventArgs e)
        {
            if (FloatWindowAdded != null)
            {
                FloatWindowAdded(null, e);
            }
        }
        public event FloatWindowEventHandler FloatWindowRemoved;
        private void onFloatWindowRemoved(FloatWindowEventArgs e)
        {
            if (FloatWindowRemoved != null)
            {
                FloatWindowRemoved(null, e);
            }
        }

        private Dictionary<Guid, FloatWindow> FloatWindowDictionary = new Dictionary<Guid, FloatWindow>();
        internal FloatWindowCollection()
            : base(new List<FloatWindow>())
		{
		}

        internal FloatWindow CreateOrGetFloatWindow()
        {
            FloatWindow window = null;
            foreach (FloatWindow w in this.Items)
            {
                if (w.DockPageCollection.Count < 1)
                {
                    return w;
                }
            }
            window = new FloatWindow();
            Add(window);
            onFloatWindowAdded(new FloatWindowEventArgs(window));
            return window;

        }

		internal int Add(FloatWindow fw)
		{
			if (Items.Contains(fw))
				return Items.IndexOf(fw);

			Items.Add(fw);
            return Count - 1;
		}

		internal void Dispose()
		{
			for (int i=Count - 1; i>=0; i--)
				this[i].Close();
		}

		internal void Remove(FloatWindow fw)
		{
			Items.Remove(fw);
            onFloatWindowRemoved(new FloatWindowEventArgs(fw));
		}

		internal void BringWindowToFront(FloatWindow fw)
		{
			Items.Remove(fw);
			Items.Add(fw);
		}
	}
}
