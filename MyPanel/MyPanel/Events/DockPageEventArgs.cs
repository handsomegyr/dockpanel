using System;
using System.Collections.Generic;
using System.Text;

namespace Guoyongrong.WinFormsUI.Docking
{
    public delegate void DockPageEventHandler(object sender, DockPageEventArgs e);  

    public class DockPageEventArgs : EventArgs
    {
        private DockPage m_page;

        public DockPageEventArgs(DockPage page)
        {
            m_page = page;
        }

        public DockPage DockPage
        {
            get { return m_page; }
        }
    }
}
