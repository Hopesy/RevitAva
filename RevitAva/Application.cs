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

namespace RevitAva;

public class Application : IExternalApplication
{
    public Result OnStartup(UIControlledApplication application)
    {
        //创建UI面板，添加按钮
        this.CreateRibbon(application);
        Host.Start();
        var logger = Host.GetService<ILogger<Application>>();
        // 初始化 Avalonia（借用 WPF 消息循环）
        AppBuilder.Configure<Avalonia.Application>()
            .UsePlatformDetect()
            .LogToTrace()
            .SetupWithoutStarting();
        // 设置 Fluent 主题
        Avalonia.Application.Current!.Styles.Add(new FluentTheme());
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
