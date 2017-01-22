using System;
using System.Collections.Generic;
using System.Text;

namespace Guoyongrong.WinFormsUI.Docking
{
    public delegate void DockStateChangedEventHandler(object sender, DockStateChangedEventArgs e);

    public class DockStateChangedEventArgs : EventArgs
    {
        private DockState m_newState;
        private DockState m_oldState;

        public DockStateChangedEventArgs(DockState oldState, DockState newState)
        {
            m_oldState = oldState;
            m_newState = newState;
        }

        public DockState NewState
        {
            get { return m_newState; }
        }

        public DockState OldState
        {
            get { return m_oldState; }
        }
    }
}
