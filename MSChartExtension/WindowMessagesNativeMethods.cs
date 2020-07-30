using System.Runtime.InteropServices;

namespace System.Windows.Forms.DataVisualization.Charting
{
    static class WindowMessagesNativeMethods
    {
        #region [ Suspend / Resume Drawing ]
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, Int32 wMsg, IntPtr wParam, IntPtr lParam);

        private const int WM_SETREDRAW = 11;
        public static void SuspendDrawing(Control parent) { SendMessage(parent.Handle, WM_SETREDRAW, IntPtr.Zero, IntPtr.Zero); }
        public static void ResumeDrawing(Control parent) { SendMessage(parent.Handle, WM_SETREDRAW, new IntPtr(1), IntPtr.Zero); parent.Refresh(); }
        #endregion

    }
}
