using System;
using System.Collections.Generic;
using System.Text;

namespace Guoyongrong.WinFormsUI.Docking
{
    public delegate void FloatWindowEventHandler(object sender, FloatWindowEventArgs e);  

    public class FloatWindowEventArgs : EventArgs
    {
        private FloatWindow m_window;

        public FloatWindowEventArgs(FloatWindow window)
        {
            m_window = window;
        }

        public FloatWindow FloatWindow
        {
            get { return m_window; }
        }
    }
}
