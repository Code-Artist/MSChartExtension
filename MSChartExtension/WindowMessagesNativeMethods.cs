using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace System.Windows.Forms.DataVisualization.Charting
{
    static class WindowMessagesNativeMethods
    {
        #region [ Suspend / Resume Drawing ]
        [DllImport("user32.dll")]
        private static extern long SendMessage(IntPtr hWnd, Int32 wMsg, [MarshalAs(UnmanagedType.Bool)] bool wParam, Int32 lParam);

        private const int WM_SETREDRAW = 11;
        public static void SuspendDrawing(Control parent) { SendMessage(parent.Handle, WM_SETREDRAW, false, 0); }
        public static void ResumeDrawing(Control parent) { SendMessage(parent.Handle, WM_SETREDRAW, true, 0); parent.Refresh(); }
        #endregion

    }
}
