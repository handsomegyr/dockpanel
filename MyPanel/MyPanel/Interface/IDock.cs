using System;
using System.Collections.Generic;
using System.Text;

namespace Guoyongrong.WinFormsUI.Docking
{
    public interface IDock
    {
        Guid GUID { get; }
        DockState DockState { get; }
        bool IsAutoHide { get; }
        bool IsFloat { get; }
    }
}
