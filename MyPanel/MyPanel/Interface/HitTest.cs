using System;
using System.Collections.Generic;
using System.Text;

namespace Guoyongrong.WinFormsUI.Docking
{
     #region HitTest
        
        public enum HitTestArea
        {
            Caption,
            TabStrip,
            Content,
            None
        }

        public struct HitTestResult
        {
            public HitTestArea HitArea;
            public int Index;

            public HitTestResult(HitTestArea hitTestArea, int index)
            {
                HitArea = hitTestArea;
                Index = index;
            }
        }

        #endregion
}
