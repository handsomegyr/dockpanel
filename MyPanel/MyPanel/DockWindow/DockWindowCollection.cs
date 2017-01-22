using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Guoyongrong.WinFormsUI.Docking
{
	internal class DockWindowCollection : ReadOnlyCollection<DockWindow>
	{
        public event DockWindowEventHandler DockWindowAdded;
        private void onDockWindowAdded(DockWindowEventArgs e)
        {
            if (DockWindowAdded != null)
            {
                DockWindowAdded(null, e);
            }
        }
        public event DockWindowEventHandler DockWindowRemoved;
        private void onDockWindowRemoved(DockWindowEventArgs e)
        {
            if (DockWindowRemoved != null)
            {
                DockWindowRemoved(null, e);
            }
        }
        
        internal DockWindowCollection()
            : base(new List<DockWindow>())
		{
		}

        internal void AddDockWindow(DockWindow window)
        {
            Items.Add(window);
            onDockWindowAdded(new DockWindowEventArgs(window));
        }

        internal void RemoveDockWindow(DockWindow window)
        {
            Items.Remove(window);
            onDockWindowRemoved(new DockWindowEventArgs(window));
        }

        internal DockWindow this[DockState dockState]
		{
			get
			{
                if (Items.Count == 0)
                {
                    DockWindow Document = new DockWindow();
                    Document.DockState = DockState.Document;
                    Document.RemoveAll();

                    DockWindow DockLeft = new DockWindow();
                    DockLeft.DockState = DockState.DockLeft;
                    DockLeft.RemoveAll();

                    DockWindow DockRight = new DockWindow();
                    DockRight.DockState = DockState.DockRight;
                    DockRight.RemoveAll();

                    DockWindow DockTop = new DockWindow();
                    DockTop.DockState = DockState.DockTop;
                    DockTop.RemoveAll();

                    DockWindow DockBottom = new DockWindow();
                    DockBottom.DockState = DockState.DockBottom;
                    DockBottom.RemoveAll();

                    AddDockWindow(Document);
                    AddDockWindow(DockLeft);
                    AddDockWindow(DockRight);
                    AddDockWindow(DockTop);
                    AddDockWindow(DockBottom);
                }

                if (dockState == DockState.Document)
					return Items[0];
				else if (dockState == DockState.DockLeft || dockState == DockState.DockLeftAutoHide)
					return Items[1];
				else if (dockState == DockState.DockRight || dockState == DockState.DockRightAutoHide)
					return Items[2];
				else if (dockState == DockState.DockTop || dockState == DockState.DockTopAutoHide)
					return Items[3];
				else if (dockState == DockState.DockBottom || dockState == DockState.DockBottomAutoHide)
					return Items[4];

				throw (new ArgumentOutOfRangeException());
			}
		}

    }
}
