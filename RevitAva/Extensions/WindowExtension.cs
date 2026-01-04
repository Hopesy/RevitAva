using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Threading;
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

    private const int GWL_HWNDPARENT = -8;

    private static IntPtr RevitMainWindowHandle => Process.GetCurrentProcess().MainWindowHandle;

    /// <summary>
    /// 以模态方式显示 Avalonia 窗口（借用 WPF DispatcherFrame）
    /// </summary>
    public static void ShowWindow(this Window window)
    {
        var revitHandle = RevitMainWindowHandle;
        var frame = new DispatcherFrame();

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
            // 窗口关闭前先恢复 Revit 窗口并激活焦点
            EnableWindow(revitHandle, true);
            SetForegroundWindow(revitHandle);
        };

        window.Closed += (_, _) =>
        {
            frame.Continue = false;
        };

        EnableWindow(revitHandle, false);
        window.Show();

        Dispatcher.PushFrame(frame);
    }
}
