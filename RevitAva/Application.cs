using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RevitAva.Commands;
using System.Reflection;
using Tuna.Revit.Extensions;
using Avalonia;
using Avalonia.Themes.Fluent;
using RevitAva.Services;
using Semi.Avalonia;

namespace RevitAva;
/*
 *  Windows的消息分发基于HWND，不是基于框架
 * 每个窗口有自己的HWND和WndProc
 * 只要有一个消息循环在运行，所有窗口都能工作
 * Revit 的消息循环已经在运行了，只需要"注册"(创建窗口)，系统自动处理其余的
 */
public class Application : IExternalApplication
{
    public Result OnStartup(UIControlledApplication application)
    {
        //创建UI面板，添加按钮
        this.CreateRibbon(application);
        Host.Start();
        var logger = Host.GetService<ILogger<Application>>();
        // 初始化 Avalonia（借用 WPF 消息循环）
        // 第一次使用任何WPF类型时CLR自动加载Application类型,执行静态构造函数,初始化渲染引擎、主题样式等
        // Avalonia跨平台,不能假设环境,必须显式配置平台后端和主题
        AppBuilder.Configure<Avalonia.Application>()
            .UsePlatformDetect()
            .LogToTrace()
            .SetupWithoutStarting();//【关键】初始化Avalonia框架配置但不启动应用程序生命周期
        //.StartWithClassicDesktopLifetime
        // 设置 Fluent 主题
        Avalonia.Application.Current!.Styles.Add(new SemiTheme());
        logger.LogInformation("RevitAva插件启动");
        return Result.Succeeded;
    }

    public Result OnShutdown(UIControlledApplication application)
    {
        var logger = Host.GetService<ILogger<Application>>();
        logger.LogInformation("RevitAva插件关闭");
        Host.Stop();
        return Result.Succeeded;
    }
    private void CreateRibbon(UIControlledApplication application)
    {
        var tab = application.AddRibbonTab("RevitAva");
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
    }
}
