using System.Diagnostics;
using System.Runtime.InteropServices;
using Avalonia.Controls;

namespace RevitAva.Extensions;

public static class WindowExtension
{
    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    [DllImport("user32.dll")]
    private static extern bool EnableWindow(IntPtr hWnd, bool bEnable);

    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern bool BringWindowToTop(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern bool GetMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

    [DllImport("user32.dll")]
    private static extern bool TranslateMessage(ref MSG lpMsg);

    [DllImport("user32.dll")]
    private static extern IntPtr DispatchMessage(ref MSG lpMsg);

    [StructLayout(LayoutKind.Sequential)]
    private struct MSG
    {
        public IntPtr hwnd;
        public uint message;
        public IntPtr wParam;
        public IntPtr lParam;
        public uint time;
        public POINT pt;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT
    {
        public int X;
        public int Y;
    }

    private const int GWL_HWNDPARENT = -8;

    private static IntPtr RevitMainWindowHandle => Process.GetCurrentProcess().MainWindowHandle;

    /// <summary>
    /// 以模态方式显示 Avalonia 窗口
    /// </summary>
    public static void ShowModal(this Window window)
    {
        var revitHandle = RevitMainWindowHandle;
        var closed = false;

        window.Opened += (_, _) =>
        {
            var handle = window.TryGetPlatformHandle()?.Handle;
            if (handle.HasValue && revitHandle != IntPtr.Zero)
            {
                SetWindowLong(handle.Value, GWL_HWNDPARENT, revitHandle);
            }
        };

        window.Closing += (_, _) =>
        {
            // 窗口关闭前先恢复 Revit 窗口并激活
            EnableWindow(revitHandle, true);
            SetForegroundWindow(revitHandle);
            BringWindowToTop(revitHandle);
        };

        window.Closed += (_, _) =>
        {
            closed = true;
        };

        EnableWindow(revitHandle, false);
        window.Show();

        while (!closed && GetMessage(out var msg, IntPtr.Zero, 0, 0))
        {
            TranslateMessage(ref msg);
            DispatchMessage(ref msg);
        }
    }
}
