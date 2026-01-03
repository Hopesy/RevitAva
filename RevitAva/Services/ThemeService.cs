using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Avalonia;
using Avalonia.Styling;
using Microsoft.Extensions.Logging;
using RevitAva.Services.Interfaces;

namespace RevitAva.Services;

/// <summary>
/// 主题服务实现
/// 监听 Revit 主题变化并自动同步 Avalonia UI 主题
/// </summary>
public class ThemeService : IThemeService
{
    private readonly ILogger<ThemeService> _logger;
    private UIControlledApplication? _application;
    // 内部使用字段，暴露给外部的是只读属性
    private bool _isDarkTheme;
    public bool IsDarkTheme => _isDarkTheme;

    public ThemeService(ILogger<ThemeService> logger) => _logger = logger;
    public void Initialize(UIControlledApplication application)
    {
        _application = application ?? throw new ArgumentNullException(nameof(application));

        try
        {
            // 订阅 Revit 的主题变化事件
            _application.ThemeChanged += OnRevitThemeChanged;
            _logger.LogInformation("已订阅 Revit ThemeChanged 事件");

            // 获取并应用当前 Revit 主题
            var currentTheme = UIThemeManager.CurrentTheme;
            _isDarkTheme = currentTheme == UITheme.Dark;

            ApplyTheme(_isDarkTheme);

            _logger.LogInformation("主题服务初始化完成，当前主题: {Theme}",
                _isDarkTheme ? "深色" : "浅色");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "初始化主题服务时发生错误");
            throw;
        }
    }
    // Revit 主题变化事件处理器
    private void OnRevitThemeChanged(object? sender, ThemeChangedEventArgs e)
    {       
         
            // 获取新的主题
            var newTheme = UIThemeManager.CurrentTheme;
            _isDarkTheme = newTheme == UITheme.Dark;
            // 应用新主题到 Avalonia
            ApplyTheme(_isDarkTheme);
    }
    // 暴露给外部，手动设置主题
    public void SetTheme(bool isDark)  => ApplyTheme(isDark);
    // 应用主题到 Avalonia Application
    private void ApplyTheme(bool isDark)
    {
        try
        {
            // 确保 Avalonia Application 已初始化
            if (global::Avalonia.Application.Current == null)
            {
                _logger.LogWarning("Avalonia.Application.Current 为 null，无法应用主题");
                return;
            }

            // 使用 Avalonia 的线程调度器确保在 UI 线程上执行
            global::Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                try
                {
                    // 设置 Avalonia 的主题变体
                    global::Avalonia.Application.Current.RequestedThemeVariant = isDark
                        ? ThemeVariant.Dark
                        : ThemeVariant.Light;

                    _logger.LogDebug("已将 Avalonia 主题切换为: {Variant}",
                        isDark ? "Dark" : "Light");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "在 UI 线程上应用主题时发生错误");
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "应用主题时发生错误");
        }
    }
    
    // 取消订阅事件（在插件卸载时调用）
    public void Dispose()
    {
        if (_application != null) 
        { 
            _application.ThemeChanged -= OnRevitThemeChanged;
            _logger.LogInformation("已取消订阅 Revit ThemeChanged 事件");
        }

    }
}
