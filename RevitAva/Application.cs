using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;
using Microsoft.Extensions.Logging;
using RevitAva.Commands;
using Tuna.Revit.Extensions;
using Avalonia;
using RevitAva.Extensions;
using RevitAva.Services.Interfaces;
using Semi.Avalonia;

namespace RevitAva;
/* 【1】消息兼容机制
 * 非模态：都依赖于主线程的消息循环
 * Windows的消息分发基于HWND，不是基于框架，每个窗口有自己的HWND和WndProc，只要有一个消息循环在运行，所有窗口都能工作
 * avalonia底层消息类型也是win32，所以主线程的消息循环可以正常处理avalonia的交互
 * Revit主线程本质是挂载着WPF的调度器System.Windows.Threading.Dispatcher,可以处理Revit消息和Win32消息(avalonia能借用的原因)
 * Revit的API、事务系统、空闲事件 (Idling) 都深度绑定在这个特定的调度器上。
 * 模态时：主线程的消息循环暂停处理，内部又开了嵌套消息循环先处理当前窗口的所有交互
 * Avalonia是一个跨平台框架，它有自己的一套调度器Avalonia.Threading.Dispatcher,只关心自己的事件和基础的Win32消息,没法处理Revit消息
 * ShowDialog时候会暂停主线程消息循环，启动窗口的嵌套消息循环，wpf可以顺利处理Revit消息,avalonia却不认识Revit消息没法处理
 */
/*
 * 【2】事务机制
 * 我们没办法调用Dialog,因为要传avalonia的window
 * 只要能将代码卡在command里面，事务就能开启成功
 * 因此采用Show+手写消息循环模拟模态，卡住command中代码在视图显示那一行，这样一直有事务权限
 * 复杂的事务（比如涉及视图严重重绘，或者交互选取）可能出问题，还没测试
 */

public class Application : IExternalApplication
{
    public Result OnStartup(UIControlledApplication application)
    {
        //创建UI面板，添加按钮
        this.CreateRibbon(application);
        // 启动 Host（必须在初始化 Avalonia 之前）
        var uiApp = application.GetUIApplication();
        if (uiApp == null)
        {
            return Result.Failed;
        }
        Host.Start(uiApp);
        var logger = Host.GetService<ILogger<Application>>();
        //【重点】Avalonia不需要自己的消息循环就能运行，因为它蹭了Revit的消息循环来帮它把消息从操作系统里取出来，并分发给自己
        // 第一次使用任何WPF类型时CLR自动加载Application类型,执行静态构造函数,初始化渲染引擎、主题样式等
        // Avalonia跨平台,不能假设环境,必须显式配置平台后端和主题,wpf会自动完后曾
        // 注意：HotAvalonia 会通过 MSBuild 任务自动启用热重载（Debug 模式下）
        AppBuilder.Configure<AvaloniaApp>()
            .UsePlatformDetect()
            .LogToTrace()
            .SetupWithoutStarting(); //【关键】初始化Avalonia框架配置但不启动应用程序生命周期
        //.StartWithClassicDesktopLifetime
        // 添加 SemiTheme 到样式集合
        // Avalonia.Application.Current!.Styles.Add(new SemiTheme());
        // 初始化主题服务（会自动根据 Revit 当前主题设置 Avalonia 主题）
        var themeService = Host.GetService<IThemeService>();
        themeService.Initialize(application);
        logger.LogInformation("RevitAva插件启动");
        return Result.Succeeded;
    }

    public Result OnShutdown(UIControlledApplication application)
    {
        var logger = Host.GetService<ILogger<Application>>();
        logger.LogInformation("RevitAva插件关闭");

        // 释放主题服务资源
        var themeService = Host.GetService<IThemeService>();
        themeService.Dispose();

        // 停止 Host
        Host.Stop();

        return Result.Succeeded;
    }
    private void CreateRibbon(UIControlledApplication application)
    {
        var tab = application.AddRibbonTab("RevitAva");

        // 服务面板
        tab.AddRibbonPanel("服务", panel =>
        {
            panel.AddPushButton<SettingCommand>(button =>
                {
                    button.LargeImage = new BitmapImage(
                        new Uri("pack://application:,,,/RevitAva;component/Resources/Icons/setting.png"));
                    button.ToolTip = "设置MCP服务";
                    button.Title = "设置";
                });
        });

        // 阵列面板
        tab.AddRibbonPanel("阵列", panel =>
        {
            panel.AddPushButton<CurveArrayCommand>(button =>
            {
                button.LargeImage = new BitmapImage(
                    new Uri("pack://application:,,,/RevitAva;component/Resources/Icons/array.png"));
                button.ToolTip = "沿曲线阵列常规模型";
                button.Title = "曲线阵列";
            });
        });
    }
}
