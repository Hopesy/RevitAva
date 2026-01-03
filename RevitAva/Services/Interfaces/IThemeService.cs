using Autodesk.Revit.UI;

namespace RevitAva.Services.Interfaces;

/// <summary>
/// 主题服务接口
/// 负责监听 Revit 主题变化并同步更新 Avalonia UI 主题
/// </summary>
public interface IThemeService
{
    /// <summary>
    /// 初始化主题服务
    /// 订阅 Revit 的 ThemeChanged 事件并设置初始主题
    /// </summary>
    /// <param name="application">UIControlledApplication 实例</param>
    void Initialize(UIControlledApplication application);

    /// <summary>
    /// 获取当前主题是否为深色模式
    /// </summary>
    bool IsDarkTheme { get; }

    /// <summary>
    /// 手动切换主题（用于测试或手动控制）
    /// </summary>
    /// <param name="isDark">是否切换为深色主题</param>
    void SetTheme(bool isDark);

    /// <summary>
    /// 释放资源，取消事件订阅
    /// </summary>
    void Dispose();
}
