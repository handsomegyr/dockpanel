using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Guoyongrong.WinFormsUI.Docking
{
    public interface IContent
    {
        string ToolTipText { get; set; }
        IContent Content { get; }
        ContextMenuStrip TabPageContextMenuStrip { get; set; }
        ContextMenu TabPageContextMenu { get; set; }
        Guid GUID { get; }
        void Activate();
        string GetPersistString();
        String Text { get; set; }
        Icon Icon { get; set; }
        bool CloseButtonEnable { get; set; }
        bool CloseButtonVisible { get; set; }
        DockAreas DockAreas { get; set; }
        DockState ShowHint { get; set; }
        bool AllowEndUserDocking { get; set; }
        bool HideOnClose { get; set; }
        void Close();
    }
}
