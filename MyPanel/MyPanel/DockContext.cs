using System;
using System.Collections.Generic;
using System.Text;

namespace Guoyongrong.WinFormsUI.Docking
{
    public class DockContext
    {
        public DockContext(DockContent content)
        {
            m_DockContent = content;
        }

        private DockContent m_DockContent;
        internal DockContent DockContent
        {
            get { return m_DockContent; }
            set { m_DockContent = value; }
        }

        private DockPage m_PrevPanelPage;
        public DockPage PrevPanelPage
        {
            get
            {
                return m_PrevPanelPage;
            }
            set
            {
                if (m_PrevPanelPage == value)
                    return;
                if (m_PrevPanelPage != null)
                {
                    m_PrevPanelPage.RemoveDockContent(this.DockContent);
                }
                m_PrevPanelPage = value;
            }
        }

        private DockPage m_PrevFloatPage;
        public DockPage PrevFloatPage
        {
            get
            {
                return m_PrevFloatPage;
            }
            set
            {
                if (m_PrevFloatPage == value)
                    return;
                if (m_PrevFloatPage != null)
                {
                    m_PrevFloatPage.RemoveDockContent(this.DockContent);
                }
                m_PrevFloatPage = value;
            }
        }
                
    }
}
