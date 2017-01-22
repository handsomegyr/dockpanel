using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing;

namespace Guoyongrong.WinFormsUI.Docking
{
    public interface IDockPageStripControl
    {
        bool IsActivated { get;set; }
        DockPaneStripSkin Skin { get; set; }
        DocumentTabStripLocation DocumentTabStripLocation { get; set; }
        bool ShowDocumentIcon { get; set; }
        AppearanceStyle Appearance { get; set; }

        Control CloseButton { get; }

        int HitTest(Point point);
        GraphicsPath GetOutline(int index);
        void RefreshChanges();
    }
}
