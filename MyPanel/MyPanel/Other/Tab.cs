using System;
using System.Collections.Generic;
using System.Text;

namespace Guoyongrong.WinFormsUI.Docking
{
    public class Tab
    {
        private int m_tabX;
        public int TabX
        {
            get { return m_tabX; }
            set { m_tabX = value; }
        }

        private int m_tabWidth;
        public int TabWidth
        {
            get { return m_tabWidth; }
            set { m_tabWidth = value; }
        }

        private int m_maxWidth;
        public int MaxWidth
        {
            get { return m_maxWidth; }
            set { m_maxWidth = value; }
        }

        private bool m_flag;
        protected internal bool Flag
        {
            get { return m_flag; }
            set { m_flag = value; }
        }

    }
}
