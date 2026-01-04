using Autodesk.Revit.UI;

namespace RevitAva.Services.Interfaces;

/// <summary>
/// 主题服务接口
/// </summary>
public interface IThemeService
{
    /// <summary>
    /// 初始化主题服务
    /// </summary>
    void Initialize(UIControlledApplication application);

    /// <summary>
    /// 当前是否为深色主题
    /// </summary>
    bool IsDarkTheme { get; }

    /// <summary>
    /// 切换 Revit 主题（深色/浅色）
    /// </summary>
    void ToggleTheme();

    /// <summary>
    /// 释放资源
    /// </summary>
    void Dispose();
}
