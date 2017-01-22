using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Guoyongrong.WinFormsUI.Docking
{
    internal interface INestedPanesContainer : IDockPageContainer
    {
        NestedPageCollection NestedPanes { get; }
        VisibleNestedPageCollection VisibleNestedPanes { get; }
    }
}
