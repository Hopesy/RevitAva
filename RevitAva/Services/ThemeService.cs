using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
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
    private bool _isDarkTheme;

    public bool IsDarkTheme => _isDarkTheme;

    public ThemeService(ILogger<ThemeService> logger) => _logger = logger;

    public void Initialize(UIControlledApplication application)
    {
        _application = application ?? throw new ArgumentNullException(nameof(application));

        // 订阅 Revit 主题变化事件
        _application.ThemeChanged += OnRevitThemeChanged;

        // 同步当前主题
        _isDarkTheme = UIThemeManager.CurrentTheme == UITheme.Dark;
        ApplyAvaloniaTheme(_isDarkTheme);

        _logger.LogInformation("主题服务初始化完成，当前主题: {Theme}", _isDarkTheme ? "深色" : "浅色");
    }

    /// <summary>
    /// 切换 Revit 主题
    /// </summary>
    public void ToggleTheme()
    {
        var newTheme = _isDarkTheme ? UITheme.Light : UITheme.Dark;
        UIThemeManager.CurrentTheme = newTheme;
        _logger.LogInformation("已切换 Revit 主题为: {Theme}", newTheme);
    }

    private void OnRevitThemeChanged(object? sender, ThemeChangedEventArgs e)
    {
        _isDarkTheme = UIThemeManager.CurrentTheme == UITheme.Dark;
        ApplyAvaloniaTheme(_isDarkTheme);
        _logger.LogInformation("Revit 主题已变更为: {Theme}", _isDarkTheme ? "深色" : "浅色");
    }

    private void ApplyAvaloniaTheme(bool isDark)
    {
        if (global::Avalonia.Application.Current == null)
            return;

        global::Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            global::Avalonia.Application.Current.RequestedThemeVariant = isDark
                ? ThemeVariant.Dark
                : ThemeVariant.Light;
        });
    }

    public void Dispose()
    {
        if (_application != null)
        {
            _application.ThemeChanged -= OnRevitThemeChanged;
            _logger.LogInformation("已取消订阅 Revit ThemeChanged 事件");
        }
    }
}
