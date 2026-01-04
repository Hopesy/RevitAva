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

    private const int GWL_HWNDPARENT = -8;

    /// <summary>
    /// 获取 Revit 主窗口句柄
    /// </summary>
    private static IntPtr RevitMainWindowHandle => Process.GetCurrentProcess().MainWindowHandle;

    /// <summary>
    /// 以模态方式显示 Avalonia 窗口（适用于 Revit 插件环境）
    /// </summary>
    public static void ShowModal(this Window window)
    {
        var ownerHandle = RevitMainWindowHandle;

        window.Opened += (_, _) =>
        {
            var handle = window.TryGetPlatformHandle()?.Handle;
            if (handle.HasValue && ownerHandle != IntPtr.Zero)
            {
                SetWindowLong(handle.Value, GWL_HWNDPARENT, ownerHandle);
            }
        };

        // 禁用 Revit 主窗口
        EnableWindow(ownerHandle, false);
        
        window.Closed += (_, _) =>
        {
            // 重新启用 Revit 主窗口
            EnableWindow(ownerHandle, true);
        };

        window.Show();
    }
}
