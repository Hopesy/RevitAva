using Autodesk.Revit.UI;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System.IO;
using System.Reflection;
using RevitAva.Services;
using RevitAva.Services.Interfaces;

namespace RevitAva;

public static class Host
{
    private static IHost? _host;

    public static void Start(UIApplication uiApplication)
    {
        var builder = Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
        {
            ContentRootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
        });

        // 配置日志
        builder.Logging.ClearProviders();
        var logDirectory = Path.Combine(builder.Environment.ContentRootPath, "Logs");
        Directory.CreateDirectory(logDirectory);
        var logPath = Path.Combine(logDirectory, "RevitAva.log");
        builder.Logging.AddSerilog(new LoggerConfiguration()
            .WriteTo.File(logPath, rollingInterval: RollingInterval.Day)
            .WriteTo.Console()
            .CreateLogger());

        // 注册 Revit 上下文（单例）
        builder.Services.AddSingleton<IRevitContext>(new RevitContext(uiApplication));
        // 注册 Messenger（单例）
        builder.Services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);
        // 注册服务
        builder.Services.AddSingleton<IThemeService, ThemeService>();
        builder.Services.AddSingleton<ICurveArrayService, CurveArrayService>();

        // 注册 View 和 ViewModel
        builder.Services.AddTransient<Views.CurveArrayView>();
        builder.Services.AddTransient<ViewModels.CurveArrayViewModel>();
        builder.Services.AddTransient<Views.SettingView>();
        builder.Services.AddTransient<ViewModels.SettingViewModel>();

        _host = builder.Build();
        _host.Start();
    }

    public static void Stop()
    {
        _host?.StopAsync().GetAwaiter().GetResult();
    }

    public static T GetService<T>() where T : class => _host!.Services.GetRequiredService<T>();
}
