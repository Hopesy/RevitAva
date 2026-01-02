using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System.IO;
using System.Reflection;

namespace RevitAva;

public static class Host
{
    private static IHost? host;
    public static void Start()
    {
        //【1】使用默认配置创建主机
        var builder = Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
        {
            // 插件环境下默认的ContentRootPathRevit指向Revit.exe，因此这里需要修改为插件dll目录
            ContentRootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
        });
        // 启用选项模式，注册并验证配置，使用数据注解验证，应用启动时验证
        //builder.Services.AddOptions<SocketSettings>()
        //    .Bind(builder.Configuration.GetSection(SocketSettings.SectionName))
        //    .ValidateDataAnnotations()
        //    .ValidateOnStart();
        //【2】配置日志为Serilog（写入文件和控制台）
        builder.Logging.ClearProviders();
        // 构建完整日志目录路径并确保目录存在
        var logDirectory = Path.Combine(builder.Environment.ContentRootPath, "Logs");
        Directory.CreateDirectory(logDirectory);
        var logPath = Path.Combine(logDirectory, "RevitAva.log");
        builder.Logging.AddSerilog(new LoggerConfiguration()
            .WriteTo.File(logPath, rollingInterval: RollingInterval.Day)
            .WriteTo.Console()
            .CreateLogger());
        //【3】添加服务如View,ViewModel,Service
        //builder.Services.AddTransient<StartUpViewModel>();

        host = builder.Build();
        host.Start();
    }
    public static void Stop()
    {
        //GetAwaiter()：获取 Task 的等待器;GetResult()：阻塞当前线程，直到StopAsync()完成
        host!.StopAsync().GetAwaiter().GetResult();
    }
    public static T GetService<T>() where T : class => host!.Services.GetRequiredService<T>();
    public static IServiceProvider Services
    {
        get => host?.Services ?? throw new InvalidOperationException("Host is null.");
    }
}
