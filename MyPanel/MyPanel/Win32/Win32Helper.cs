using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Guoyongrong.WinFormsUI.Docking
{
    internal static class Win32Helper
    {
        public static Control ControlAtPoint(Point pt)
        {
            return Control.FromChildHandle(NativeMethods.WindowFromPoint(pt));
        }

        public static uint MakeLong(int low, int high)
        {
            return (uint)((high << 16) + low);
        }
    }
}
