using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Guoyongrong.WinFormsUI.Docking
{
    public interface IDockContentContainer
    {
        DockContentCollection DockContents{ get;}
        VisibleContentCollection DisplayingContents { get; }
        void AddDockContent(DockContent content);
        void RemoveDockContent(DockContent content);
        void RemoveAll();
    }
}
