using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Guoyongrong.WinFormsUI.Docking
{
    internal static class DockHelper
    {
        public static bool IsDockStateAutoHide(DockState dockState)
        {
            if (dockState == DockState.DockLeftAutoHide ||
                dockState == DockState.DockRightAutoHide ||
                dockState == DockState.DockTopAutoHide ||
                dockState == DockState.DockBottomAutoHide)
                return true;
            else
                return false;
        }

        public static bool IsFloat(DockState dockState)
        {
            if (dockState == DockState.Float)
                return true;
            else
                return false;
        }

        public static DockState GetDockState(DockStyle dockStyle)
        {
            DockState dockState = DockState.Unknown;

            if (dockStyle == DockStyle.Top)
                dockState = DockState.DockTop;
            else if (dockStyle == DockStyle.Bottom)
                dockState = DockState.DockBottom;
            else if (dockStyle == DockStyle.Left)
                dockState = DockState.DockLeft;
            else if (dockStyle == DockStyle.Right)
                dockState = DockState.DockRight;
            else if (dockStyle == DockStyle.Fill)
                dockState = DockState.Document;
            return dockState;

        }

        public static DockAlignment GetDockAlignment(DockStyle dockStyle)
        {
            DockAlignment alignment = DockAlignment.Unknown;

            if (dockStyle == DockStyle.Left)
            {
                alignment = DockAlignment.Left;
            }
            else if (dockStyle == DockStyle.Right)
            {
                alignment = DockAlignment.Right;
            }
            else if (dockStyle == DockStyle.Top)
            {
                alignment = DockAlignment.Top;
            }
            else if (dockStyle == DockStyle.Bottom)
            {
                alignment = DockAlignment.Bottom;
            }
            else if (dockStyle == DockStyle.Fill)
            {
                alignment = DockAlignment.Fill;
            }
            return alignment;

        }
        public static DockAlignment GetDockAlignment(DockState state)
        {
            //DockAlignment alignment;
            //if (container.DockState == DockState.DockLeft || container.DockState == DockState.DockRight)
            //    alignment = DockAlignment.Bottom;
            //else
            //    alignment = DockAlignment.Right;

            if (state == DockState.DockLeft || state == DockState.DockLeftAutoHide)
                return DockAlignment.Left;
            else if (state == DockState.DockRight || state == DockState.DockRightAutoHide)
                return DockAlignment.Right;
            else if (state == DockState.DockTop || state == DockState.DockTopAutoHide)
                return DockAlignment.Top;
            else if (state == DockState.DockBottom || state == DockState.DockBottomAutoHide)
                return DockAlignment.Bottom;
            else
            {
                return DockAlignment.Fill;
            }
        }
        public static DockState ToggleAutoHideState(DockState state)
        {
            if (state == DockState.DockLeft)
                return DockState.DockLeftAutoHide;
            else if (state == DockState.DockRight)
                return DockState.DockRightAutoHide;
            else if (state == DockState.DockTop)
                return DockState.DockTopAutoHide;
            else if (state == DockState.DockBottom)
                return DockState.DockBottomAutoHide;
            else if (state == DockState.DockLeftAutoHide)
                return DockState.DockLeft;
            else if (state == DockState.DockRightAutoHide)
                return DockState.DockRight;
            else if (state == DockState.DockTopAutoHide)
                return DockState.DockTop;
            else if (state == DockState.DockBottomAutoHide)
                return DockState.DockBottom;
            else
                return state;
        }

        public static bool IsDockStateValid(DockState dockState, DockAreas dockableAreas)
        {
            if (((dockableAreas & DockAreas.Float) == 0) &&
                (dockState == DockState.Float))
                return false;
            else if (((dockableAreas & DockAreas.Document) == 0) &&
                (dockState == DockState.Document))
                return false;
            else if (((dockableAreas & DockAreas.DockLeft) == 0) &&
                (dockState == DockState.DockLeft || dockState == DockState.DockLeftAutoHide))
                return false;
            else if (((dockableAreas & DockAreas.DockRight) == 0) &&
                (dockState == DockState.DockRight || dockState == DockState.DockRightAutoHide))
                return false;
            else if (((dockableAreas & DockAreas.DockTop) == 0) &&
                (dockState == DockState.DockTop || dockState == DockState.DockTopAutoHide))
                return false;
            else if (((dockableAreas & DockAreas.DockBottom) == 0) &&
                (dockState == DockState.DockBottom || dockState == DockState.DockBottomAutoHide))
                return false;
            else
                return true;
        }

        public static string GetPaneKey(DockState state)
        {
            if (state == DockState.DockLeft || state == DockState.DockLeftAutoHide)
                return "Left";
            else if (state == DockState.DockRight || state == DockState.DockRightAutoHide)
                return "Right";
            else if (state == DockState.DockTop || state == DockState.DockTopAutoHide)
                return "Top";
            else if (state == DockState.DockBottom || state == DockState.DockBottomAutoHide)
                return "Bottom";
            else if (state == DockState.Float)
                return "Float";
            else
            {
                return "Document";
            }
        }

        public static FloatWindow FloatWindowAtPoint(Point pt, DockPanel dockPanel)
        {
            for (Control control = Win32Helper.ControlAtPoint(pt); control != null; control = control.Parent)
            {
                FloatWindow floatWindow = control as FloatWindow;
                if (floatWindow != null 
                    //&& floatWindow.DockPanel == dockPanel
                    )
                    return floatWindow;
            }

            return null;
        }
        public static DockPage PaneAtPoint(Point pt, DockPanel dockPanel)
        {
            for (Control control = Win32Helper.ControlAtPoint(pt); control != null; control = control.Parent)
            {
                //DockContent content = control as DockContent;
                //if (content != null && content.DockPanel == dockPanel)
                //    return content.Pane;

                DockPage pane = control as DockPage;
                if (pane != null 
                    //&& pane.DockPanel == dockPanel
                    )
                    return pane;
            }

            return null;
        }
    }
}
