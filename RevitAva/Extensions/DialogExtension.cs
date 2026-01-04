using System.Diagnostics;
using System.Runtime.InteropServices;
using Avalonia.Controls;

namespace RevitAva.Extensions.Backup;
/*
 * 用最底层的 Win32 API (GetMessage) 截获了线程上的所有消息
 * 裸露的 Win32 GetMessage 替代了 WPF 复杂的 Dispatcher
 * ShowModal是在 IExternalCommand 里调用的，也就是在 主线程 (Thread ID: 101)。
 * while (GetMessage(...)) 循环，依然是在 主线程 (Thread ID: 101) 上一行一行跑的。
 * 界面上点击按钮，触发 Avalonia 的事件，进而调用 Transaction.Start() 时，代码依然运行在 主线程 (Thread ID: 101) 的堆栈深处
 * Revit API一看ID相同，允许开启事务
 */
public static class DialogExtension
{
    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
    
    [DllImport("user32.dll")]
    private static extern bool EnableWindow(IntPtr hWnd, bool bEnable);

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

    /// <summary>
    /// 获取 Revit 主窗口句柄
    /// </summary>
    private static IntPtr RevitMainWindowHandle => Process.GetCurrentProcess().MainWindowHandle;

    /// <summary>
    /// 以模态方式显示 Avalonia 窗口（适用于 Revit 插件环境）
    /// 使用 Win32 消息循环实现阻塞
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

        window.Closed += (_, _) =>
        {
            EnableWindow(revitHandle, true);
            closed = true;
        };

        // 禁用 Revit 主窗口
        EnableWindow(revitHandle, false);
        
        window.Show();
        //【重要】换句话说，虽然是show方法，但是我们自己写的消息循环卡住了，command命令没结束，所以一直有事务权限。
        // window.ShowDialog(window);
        // avalonia的showdialog必须传入window，做不到，为了模拟模态窗口，不得不这样做
        // 手动写了一个新的循环来处理窗口交互和其他操作，卡住Revit主线程的消息循环，直到窗口关闭
        // 原来的 Revit 主循环（WPF Dispatcher）此刻正停在调用 ShowModal() 的那一行
        // 完全无法运行，被迫把“派发消息”的权柄交给了下面写的这个 while
        while (!closed && GetMessage(out var msg, IntPtr.Zero, 0, 0))
        {    
            // GetMessage把消息取出来
            // DispatchMessage粗暴的把消息分发出去
            // 没有经过WPF中间层过滤处理消息，可能会出问题
            TranslateMessage(ref msg);
            DispatchMessage(ref msg);
        }
    }
}
/*
取信：while循环调用GetMessage，拿到了“鼠标点击”这条消息。
派发：你的代码调用 DispatchMessage。
同步等待：注意！DispatchMessage 不是把信扔出去就不管了。 它会找到 Avalonia 的窗口函数 (WndProc) 并直接调用它。
层层调用：Avalonia的WndProc发现是点击，于是调用Avalonia的Button 代码，Button代码调用OnButtonClick。
执行事务：在OnButtonClick里，写了Transaction.Start()。
Revit检查：Revit问：“你是主线程吗？”
看堆栈底部 -> 是线程 101 -> Pass！
Revit不在乎你是通过WPF Dispatcher调用的，还是通过裸while调用的，只要线程ID对，它就干活。
返回：事务提交 -> 函数返回 -> DispatchMessage 执行完毕 -> 回到你的 while 循环去取下一条消息。
结论： 事务不是被“分发”的，事务是 DispatchMessage 派生出来的任务
 */