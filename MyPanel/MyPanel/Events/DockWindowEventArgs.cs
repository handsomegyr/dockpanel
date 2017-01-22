using System;
using System.Collections.Generic;
using System.Text;

namespace Guoyongrong.WinFormsUI.Docking
{
    public delegate void DockWindowEventHandler(object sender, DockWindowEventArgs e);  

    public class DockWindowEventArgs : EventArgs
    {
        private DockWindow m_window;

        public DockWindowEventArgs(DockWindow window)
        {
            m_window = window;
        }

        public DockWindow DockWindow
        {
            get { return m_window; }
        }
    }
}
