using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using Avalonia.Controls;

namespace RevitAva.Extensions;
// Show+DispatcherFrame消息循环的方式模拟模态窗口的调用，将线程卡在command中调用窗口的地方
// 保证viewmodel中开启的Transaction是在command内部
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
    // 以模态方式显示 Avalonia 窗口（借用 WPF DispatcherFrame）
    public static void ShowWindow(this Window window)
    {
        var revitHandle = RevitMainWindowHandle;
        // 创建一个 WPF 的 DispatcherFrame,它的作用是卡住当前线程，但允许 UI 事件继续处理
        var frame = new DispatcherFrame();
        window.Opened += (_, _) =>
        {
            var handle = window.TryGetPlatformHandle()?.Handle;
            if (handle.HasValue && revitHandle != IntPtr.Zero)
            {  
                // 设置Revit 为拥有者 (Owner)
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
            // 【关键】告诉 Frame 循环可以结束了
            frame.Continue = false;
        };
        EnableWindow(revitHandle, false);
        // 使用 Show() 而不是 ShowDialog()
        window.Show();
        // 启动消息循环阻塞,防止IExternalCommand结束,同时保持UI的响应（Revit不会卡死）
        // 代码会停在这里，直到 frame.Continue 变为 false (即窗口关闭)
        Dispatcher.PushFrame(frame);
    }
}
