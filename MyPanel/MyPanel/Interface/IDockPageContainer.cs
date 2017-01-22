using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Guoyongrong.WinFormsUI.Docking
{
    public interface IDockPageContainer:IDock
    {
        DockPageCollection DockPageCollection { get; }
        void AddDockPage(DockPage page);
        void AddDockPage(DockPage page, DockPage previousPage, DockAlignment alignment, double proportion,int contentIndex);
        void RemoveDockPage(DockPage page);
        void RemoveAll();

        Rectangle DisplayingRectangle { get; }
        void RefreshChanges();
    }
}
