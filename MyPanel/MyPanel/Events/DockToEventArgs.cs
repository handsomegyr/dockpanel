using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Guoyongrong.WinFormsUI.Docking
{
    public delegate void DockToEventHandler(object sender, DockToEventArgs e);

    public class DockToEventArgs : EventArgs
    {
        private DockPage m_dockPan;
        private DockPage m_prevPan;
        private DockAlignment m_alignment = DockAlignment.Unknown;
        private DockStyle m_dockStyle = DockStyle.None;
        private double m_proportion = 0.5;
        private int m_contentIndex = -1;
        private Rectangle m_floatWindowBounds;

        public DockPage DockPan
        {
            get { return m_dockPan; }
        }
        public DockPage PrevDockPan
        {
            get { return m_prevPan; }
        }
        public DockAlignment Alignment
        {
            get { return m_alignment; }
        }
        public double Proportion
        {
            get { return m_proportion; }
        }
        public DockStyle DockStyle
        {
            get { return m_dockStyle; }
        }
        public int ContentIndex
        {
            get { return m_contentIndex; }
        }
        public Rectangle FloatWindowBounds
        {
            get { return m_floatWindowBounds; }
        }

        
        public DockToEventArgs()
        {
        }

        public DockToEventArgs(DockPage dockPan,Rectangle floatWindowBounds)
        {
            m_floatWindowBounds = floatWindowBounds;
            m_dockPan = dockPan;
        }

        public DockToEventArgs(DockPage dockPan)
        {
            m_dockPan = dockPan;
        }

        public DockToEventArgs(DockPage dockPan, DockStyle dockStyle)
        {
            m_dockPan = dockPan;
            m_dockPan.DockState = DockHelper.GetDockState(dockStyle);
            //m_alignment = DockHelper.GetDockAlignment(dockStyle);
            m_dockStyle = dockStyle;
        }

        public DockToEventArgs(DockPage dockPan, DockPage prevPan, DockAlignment alignment, double proportion)
        {
            m_dockPan = dockPan;
            m_prevPan = prevPan;
            m_alignment = alignment;
            m_proportion = proportion;
            m_contentIndex = -1;
        }

        public DockToEventArgs(DockPage dockPan, DockPage prevPan, DockStyle dockStyle, int contentIndex)
        {
            m_dockPan = dockPan;
            m_prevPan = prevPan;
            m_alignment = DockHelper.GetDockAlignment(dockStyle);
            m_proportion = 0.5;
            m_contentIndex = contentIndex;
            m_dockStyle = dockStyle;
            
        }

       
    }
}
