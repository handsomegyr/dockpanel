using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace Guoyongrong.WinFormsUI.Docking
{
    public delegate bool CheckIsVisibleFunction(DockContent c);
    public class VisibleContentCollection : IEnumerable<DockContent>
    {
        private DockContentCollection m_Contents = null;

        private CheckIsVisibleFunction m_IsVisibleFunc = null;
        public CheckIsVisibleFunction IsVisibleFunc
        {
            get
            {
                return m_IsVisibleFunc;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("IsVisibleFunc");
                }
                m_IsVisibleFunc = value;
            }
        }

        //Default Function
        private bool CheckIsVisibled(DockContent c)
        {
            return true;
        }
        
        public VisibleContentCollection(DockContentCollection Contents)
        {
            m_IsVisibleFunc = CheckIsVisibled;
            m_Contents = Contents;
        }
        public VisibleContentCollection(CheckIsVisibleFunction IsVisibleFunc,DockContentCollection Contents)
        {
            m_IsVisibleFunc = IsVisibleFunc;
            m_Contents = Contents;
        }
        
        public DockContent this[int index]
        {
            get
            {
                return GetVisibleContent(index);
            }
        }

        public bool Contains(DockContent content)
        {
            return (GetIndexOfVisibleContents(content) != -1);
        }

        public int Count
        {
            get
            {
                return CountOfVisibleContents;
            }
        }

        public int IndexOf(DockContent content)
        {
            return GetIndexOfVisibleContents(content);
        }

        private int CountOfVisibleContents
        {
            get
            {
                int count = 0;
                foreach (DockContent content in m_Contents)
                {
                    if (m_IsVisibleFunc(content))
                        count++;
                }
                return count;
            }
        }

        private DockContent GetVisibleContent(int index)
        {
            int currentIndex = -1;
            foreach (DockContent content in m_Contents)
            {
                if (m_IsVisibleFunc(content))
                    currentIndex++;

                if (currentIndex == index)
                    return content;
            }
            throw (new ArgumentOutOfRangeException());
        }

        private int GetIndexOfVisibleContents(DockContent content)
        {
            if (content == null)
                return -1;

            int index = -1;
            foreach (DockContent c in m_Contents)
            {
                if (m_IsVisibleFunc(c))
                {
                    index++;

                    if (c == content)
                        return index;
                }
            }
            return -1;
        }

        public void CopyTo(DockContent[] contents, int index)
        {
            if (index > this.Count || index < 0 )
            {
                throw new ArgumentOutOfRangeException("index");
            }

            for(int i = index;i < this.Count; i++)
            {
                contents[i] = this[i];
            }
        }

        public IEnumerator<DockContent> GetEnumerator()
        {
            foreach (DockContent c in m_Contents)
            {
                if (m_IsVisibleFunc(c))
                {
                    yield return c;
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            foreach (DockContent c in m_Contents)
            {
                if (m_IsVisibleFunc(c))
                {
                    yield return c;
                }
            }
        }
    }
}
