using System;
using System.Drawing;
using System.Collections.Generic;

namespace Guoyongrong.WinFormsUI.Docking
{
    internal sealed class NestedDockingStatus
	{
		internal NestedDockingStatus()
		{
		}

        #region factual Docking info

        private IDockPageContainer m_container;
        public IDockPageContainer Container
        {
            get { return m_container; }
        }

        private DockPage m_previousPane = null;
        public DockPage PreviousPane
        {
            get { return m_previousPane; }
        }

        private DockAlignment m_alignment = DockAlignment.Unknown;
        public DockAlignment Alignment
        {
            get { return m_alignment; }
        }

        private double m_proportion = 0.5;
        public double Proportion
        {
            get { return m_proportion; }
        }

        #endregion

        #region Display Docking info
        
        private bool m_isDisplaying = false;
		public bool IsDisplaying
		{
			get	{	return m_isDisplaying;	}
		}

		private DockPage m_displayingPreviousPane = null;
        public DockPage DisplayingPreviousPane
		{
			get	{	return m_displayingPreviousPane;	}
		}

		private DockAlignment m_displayingAlignment = DockAlignment.Left;
		public DockAlignment DisplayingAlignment
		{
			get	{	return m_displayingAlignment;	}
		}

		private double m_displayingProportion = 0.5;
		public double DisplayingProportion
		{
			get	{	return m_displayingProportion;	}
		}
        #endregion

        private Rectangle m_logicalBounds = Rectangle.Empty; 
		public Rectangle LogicalBounds
		{
			get	{	return m_logicalBounds;	}
		}

		private Rectangle m_paneBounds = Rectangle.Empty;
		public Rectangle PaneBounds
		{
			get	{	return m_paneBounds;	}
		}

		private Rectangle m_splitterBounds = Rectangle.Empty;
		public Rectangle SplitterBounds
		{
			get	{	return m_splitterBounds;	}
		}

        internal void SetStatus(IDockPageContainer container, DockPage previousPane, DockAlignment alignment, double proportion)
        {
            m_container = container;
            m_previousPane = previousPane;
            m_alignment = alignment;
            m_proportion = proportion;
        }

        internal void SetDisplayingStatus(bool isDisplaying, DockPage displayingPreviousPane, DockAlignment displayingAlignment, double displayingProportion)
		{
			m_isDisplaying = isDisplaying;
			m_displayingPreviousPane = displayingPreviousPane;
			m_displayingAlignment = displayingAlignment;
			m_displayingProportion = displayingProportion;
		}

		internal void SetDisplayingBounds(Rectangle logicalBounds, Rectangle paneBounds, Rectangle splitterBounds)
		{
			m_logicalBounds = logicalBounds;
			m_paneBounds = paneBounds;
			m_splitterBounds = splitterBounds;
		}
	}
}
