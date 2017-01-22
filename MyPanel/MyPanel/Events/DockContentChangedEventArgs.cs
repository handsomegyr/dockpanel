using System;
using System.Collections.Generic;
using System.Text;

namespace Guoyongrong.WinFormsUI.Docking
{
    public delegate void DockContentChangedEventHandler(object sender, DockContentChangedEventArgs e);
    public class DockContentChangedEventArgs : EventArgs
    {
        private DockContent m_OldContent;
        private DockContent m_NewContent;
        public DockContentChangedEventArgs(DockContent oldContent, DockContent newContent)
        {
            m_OldContent = oldContent;
            m_NewContent = newContent;
        }
        
        public DockContent NewContent
        {
            get { return m_NewContent; }
        }

        public DockContent OldContent
        {
            get { return m_OldContent; }
        }
    }
}
