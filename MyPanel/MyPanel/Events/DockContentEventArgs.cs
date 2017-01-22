using System;
using System.Collections.Generic;
using System.Text;

namespace Guoyongrong.WinFormsUI.Docking
{
    public delegate void DockContentEventHandler(object sender, DockContentEventArgs e);

    public class DockContentEventArgs : EventArgs
    {
        private DockContent m_content;

        public DockContentEventArgs(DockContent content)
        {
            m_content = content;
        }

        public DockContent Content
        {
            get { return m_content; }
        }
    }
}
